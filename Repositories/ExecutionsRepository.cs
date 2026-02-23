using Api_IA.Models.Items;
using ApiDrones.Models;
using ApiDrones.Repositories.Interface;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiDrones.Repositories
{
    public class ExecutionsRepository : IExecutionsRepository
    {
        private readonly DbContextConfi _dbContextConfi;

        public ExecutionsRepository(DbContextConfi dbContextConfi)
        {
            _dbContextConfi = dbContextConfi;
        }
        public async Task<int> AsyncTestPostmanInsertLogBD()
        {
            Executions execution = new Executions()
            {
                Execution_date = DateTime.Now,
                Execution_routine = "0",
                Store = "1",
                Drone = "0",
                Routine_id = 1,
                Processed = 0
            };

            try
            {
                await _dbContextConfi.Executions.AddAsync(execution);
                await _dbContextConfi.SaveChangesAsync();
                return execution.Execution_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error en AsyncTestPostmanInsertLogBD: " + ex);
                return 0;
            }
        }
        public async Task<int> AsyncTestPostmanInsertImageBd(int last_row_id, string image_name, string image_dir)
        {
            Images imagen = new Images()
            {
                Image_storage = image_dir,
                Image_name = image_name,
                Execution_id = last_row_id,
                Processed = 0,
                Image_url = $"{Environment.GetEnvironmentVariable("BLOB_URL_BASE")}{image_dir}/{image_name}"
            };

            try
            {
                await _dbContextConfi.Images.AddAsync(imagen);
                await _dbContextConfi.SaveChangesAsync();
                return imagen.Image_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error en AsyncTestPostmanInsertImageBd: " + ex);
                return 0;
            }
        }
        public async Task CustomVisionAsyncTestInsertPredictionsToDb(
            ImagePrediction predictions,
            string image_name,
            int execution_id,
            string image_url)
        {
            ProcessedImages2 processedImages2 = new ProcessedImages2()
            {
                Image_url = image_url,
                Image_name = image_name,
                Execution_id = execution_id
            };

            try
            {
                await _dbContextConfi.ProcessedImages2.AddAsync(processedImages2);
                await _dbContextConfi.SaveChangesAsync();

                int Last_insert_id = processedImages2.Processed_image_id;

                var list = new List<Predictions2>();

                foreach (var prediction in predictions.Predictions)
                {
                    if (prediction.TagName != "[Auto-Generated] Other Products")
                    {
                        Predictions2 predictions2 = new Predictions2()
                        {
                            Tag = prediction.TagName,
                            Probability = prediction.Probability * 100,
                            Left_x = prediction.BoundingBox.Left,
                            Top_y = prediction.BoundingBox.Top,
                            Height_y = prediction.BoundingBox.Height,
                            Width_x = prediction.BoundingBox.Width,
                            Processed_image_id = Last_insert_id,
                            Execution_id = execution_id
                        };

                        list.Add(predictions2);
                    }
                }
                if (list.Count > 0)
                {
                    await _dbContextConfi.Predictions2.AddRangeAsync(list);
                    await _dbContextConfi.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error en CustomVisionAsyncTestInsertPredictionsToDb: " + ex);
            }
        }
        public async Task UpdateImage(int image_id)
        {
            try
            {
                await _dbContextConfi.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE Images SET prediction_ready = 1, processed = 1 WHERE Image_id = {image_id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error en UpdateImage: " + ex);
                throw;
            }
        }
        public async Task<List<ResponseItems>> GetResponseItems()
        {
            try
            {
                return await _dbContextConfi
                    .Set<ResponseItems>()
                    .FromSqlInterpolated($"EXEC dbo.EdzonVista")
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error en GetResponseItems: FromSqlInterpolated" + ex);
                throw;
            }
        }
    }
}