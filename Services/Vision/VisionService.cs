using Api_IA.Models.Images;
using ApiDrones.Configurations;
using ApiDrones.infrastructure;
using ApiDrones.Models;
using ApiDrones.Models.Api_IA;
using ApiDrones.Models.StoreInfo;
using ApiDrones.Repositories.Interface;
using ApiDrones.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiDrones.Services
{
    public class DronImages : IDronImg
    {
        private static readonly ApiKraken _kraken = new ApiKraken();

        private static readonly string _blobBase = Environment.GetEnvironmentVariable("BLOB_URL_BASE");

        private readonly IExecutionsRepository _executionsRepository;

        //SINGLETONS (mejora de performance, en vez de rear un cliente por imagen)
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        private readonly CustomVisionPredictionClient _predictionClient;

        public DronImages(IExecutionsRepository executionsRepository)
        {
            _executionsRepository = executionsRepository;

            // mismo comportamiento, solo reutilizado
            _predictionClient = new ApiCustomVision().GetPredictionClient();
        }

        //Optimizacón sin cambios en metodo
        public async Task<List<bool>> CustomVisionAsyncTestMain(List<RequestImages> Images)
        {
            var tasks = new List<Task<bool>>();

            int LastInsertedId = await _executionsRepository.AsyncTestPostmanInsertLogBD();

            if (LastInsertedId != 0)
            {
                //limitar paralelismo (evita saturación/red lenta)
                var semaphore = new SemaphoreSlim(4);

                foreach (var image in Images)
                {
                    tasks.Add(Task.Run(async () =>
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
                                img_height
                            );
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                var results = await Task.WhenAll(tasks);
                return results.ToList();
            }

            return null;
        }

        //Misma optimización sin cambios en el metodo, mayor rendimiento interno
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
                int blob_status = await AsyncTestHttpUploadToBucket(image, url, content_type);

                if (blob_status == 201)
                {
                    //Sin Task.Run.... más rápido
                    int insertedImageId =
                        await _executionsRepository.AsyncTestPostmanInsertImageBd(
                            last_inserted_id,
                            image_name,
                            "subidaendpoint");

                    if (insertedImageId != 0)
                    {
                        string project_id = "1be6360f-e231-4f63-9821-e25dc7a1dfcf";
                        string publishIterationName = "Iteration13";

                        string temp_url = url;

                        if (resize_flag)
                            url = await _kraken.AsyncResizeImage(temp_url, (int)(img_height * 0.8));

                        var results = await _predictionClient.DetectImageUrlAsync(
                            Guid.Parse(project_id),
                            publishIterationName,
                            new ImageUrl(url));

                        if (results != null)
                        {
                            await _executionsRepository.CustomVisionAsyncTestInsertPredictionsToDb(
                                results,
                                image_name,
                                last_inserted_id,
                                url);

                            await _executionsRepository.UpdateImage(insertedImageId);

                            return true;
                        }

                        await _executionsRepository.UpdateImage(insertedImageId);
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        //Mejora Clve: HttpClient reutilizado
        public async Task<int> AsyncTestHttpUploadToBucket(byte[] image, string url, string content_type)
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

        //StoreInfo sin cambios funcionales, necesito hacer pruebas pero no tengo acceso a la DB
        public async Task<IActionResult> GetStore(
            IConfiguration configuration,
            string store_id,
            string aisle_id,
            string operator_id)
        {
            try
            {
                DataBaseWork dataBaseWork = new DataBaseWork(configuration);

                List<TaskStoreBD> tasks =
                    await dataBaseWork.GetTaskStore(store_id, aisle_id, operator_id);

                if (tasks == null || !tasks.Any())
                    return new NoContentResult();

                string storeName = await dataBaseWork.GetNameStore(store_id);

                StoreInfoConfig storeInfoConfig = new StoreInfoConfig();

                ResponseTasks responseStoreTasks =
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