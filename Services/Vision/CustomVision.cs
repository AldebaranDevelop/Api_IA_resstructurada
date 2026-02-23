using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDrones.infrastructure
{
    public class ApiCustomVision
    {
        public CustomVisionPredictionClient GetPredictionClient()
        {
            try
            {
                string predictionKey = Environment.GetEnvironmentVariable("VISION_PREDICTION_KEY2");
                string predictionEndpoint = Environment.GetEnvironmentVariable("VISION_PREDICTION_ENDPOINT2");

                if (string.IsNullOrEmpty(predictionKey) || string.IsNullOrEmpty(predictionEndpoint))
                    throw new InvalidOperationException("Las credenciales de Azure Custom Vision no están configuradas correctamente.");

                var client = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
                {
                    Endpoint = predictionEndpoint
                };

                return client;
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        public async Task FireAndForgetHttpCallAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync("https://groceryfirebaseupdater1.azurewebsites.net/api/updateItems");
                    Console.WriteLine($"HTTP Status Code: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during HTTP call: {ex.Message}");
                }
            }
        }
    }
}
