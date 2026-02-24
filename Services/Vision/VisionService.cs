using Api_IA.Models.Images;
using ApiDrones.Configurations;
using ApiDrones.infrastructure;
using ApiDrones.Models;
using ApiDrones.Models.Api_IA;
using ApiDrones.Repositories.Interface;
using ApiDrones.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiDrones.Services
{
    public class DronImages : IDronImg
    {
        private static readonly SemaphoreSlim _dbLock = new SemaphoreSlim(1, 1);

        private static readonly ApiKraken _kraken = new ApiKraken();

        private static readonly string _blobBase =
            Environment.GetEnvironmentVariable("BLOB_URL_BASE");

        private readonly IExecutionsRepository _executionsRepository;

        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        private readonly CustomVisionPredictionClient _predictionClient;

        public DronImages(IExecutionsRepository executionsRepository)
        {
            _executionsRepository = executionsRepository;
            _predictionClient = new ApiCustomVision().GetPredictionClient();
        }

        public async Task<List<bool>> CustomVisionAsyncTestMain(List<RequestImages> Images)
        {
            var tasks = new List<Task<bool>>();

            await _dbLock.WaitAsync();
            int LastInsertedId;
            try
            {
                LastInsertedId =
                    await _executionsRepository.AsyncTestPostmanInsertLogBD();
            }
            finally
            {
                _dbLock.Release();
            }

            if (LastInsertedId == 0)
                return null;

            var semaphore = new SemaphoreSlim(4);

            foreach (var image in Images)
            {
                tasks.Add(ProcessImageAsync(image, semaphore, LastInsertedId));
            }

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private async Task<bool> ProcessImageAsync(
            RequestImages image,
            SemaphoreSlim semaphore,
            int LastInsertedId)
        {
            await semaphore.WaitAsync();

            try
            {
                byte[] img = Convert.FromBase64String(image.Image_url);

                bool resize_flag = img.Length > 4194304;
                int img_height = 0;

                if (resize_flag)
                {
                    using var imageObj = Image.Load(img);
                    img_height = imageObj.Height;
                }

                string url =
                    $"{_blobBase}subidaendpoint/{image.Image_name}";

                return await CustomVisionAsyncTestUploadImgToBlob(
                    image.Image_name,
                    img,
                    url,
                    image.Format,
                    LastInsertedId,
                    resize_flag,
                    img_height);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<bool> CustomVisionAsyncTestUploadImgToBlob(
            string image_name,
            byte[] image,
            string url,
            string content_type,
            int last_inserted_id,
            bool resize_flag,
            int img_height)
        {
            try
            {
                int blob_status =
                    await AsyncTestHttpUploadToBucket(image, url, content_type);

                if (blob_status < 200 || blob_status >= 300)
                    return false;

                int insertedImageId;

                await _dbLock.WaitAsync();
                try
                {
                    insertedImageId =
                        await _executionsRepository.AsyncTestPostmanInsertImageBd(
                            last_inserted_id,
                            image_name,
                            "subidaendpoint");
                }
                finally
                {
                    _dbLock.Release();
                }

                if (insertedImageId == 0)
                    return false;

                string project_id = "1be6360f-e231-4f63-9821-e25dc7a1dfcf";
                string publishIterationName = "Iteration13";

                string temp_url = url;

                if (resize_flag)
                    url = await _kraken.AsyncResizeImage(temp_url, (int)(img_height * 0.8));

                var results = await _predictionClient.DetectImageUrlAsync(
                    Guid.Parse(project_id),
                    publishIterationName,
                    new ImageUrl(url));

                await _dbLock.WaitAsync();
                try
                {
                    if (results != null)
                    {
                        await _executionsRepository.CustomVisionAsyncTestInsertPredictionsToDb(
                            results,
                            image_name,
                            last_inserted_id,
                            url);
                    }

                    await _executionsRepository.UpdateImage(insertedImageId);
                }
                finally
                {
                    _dbLock.Release();
                }

                return results != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> AsyncTestHttpUploadToBucket(
            byte[] image,
            string url,
            string content_type)
        {
            string blob_url =
                $"{url}?{Environment.GetEnvironmentVariable("BLOB_UPLOAD_ENDPOINT_KEY")}";

            var request = new HttpRequestMessage(HttpMethod.Put, blob_url)
            {
                Content = new ByteArrayContent(image)
            };

            request.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);

            request.Headers.Add("x-ms-blob-type", "BlockBlob");

            var response = await _httpClient.SendAsync(request);

            return (int)response.StatusCode;
        }

        public async Task<IActionResult> GetStore(
            IConfiguration configuration,
            string store_id,
            string aisle_id,
            string operator_id)
        {
            try
            {
                DataBaseWork dataBaseWork = new DataBaseWork(configuration);

                var tasks =
                    await dataBaseWork.GetTaskStore(store_id, aisle_id, operator_id);

                if (tasks == null || !tasks.Any())
                    return new NoContentResult();

                string storeName =
                    await dataBaseWork.GetNameStore(store_id);

                StoreInfoConfig storeInfoConfig = new StoreInfoConfig();

                var responseStoreTasks =
                    await storeInfoConfig.OrderFinalTasks(tasks, storeName, store_id);

                return new OkObjectResult(responseStoreTasks);
            }
            catch (Exception ex)
            {
                return new ConflictObjectResult(ex.Message);
            }
        }
    }
}