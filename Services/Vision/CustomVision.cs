using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDrones.infrastructure
{
    public class ApiCustomVision
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private static CustomVisionPredictionClient _client;

        public CustomVisionPredictionClient GetPredictionClient()
        {
            if (_client != null)
                return _client;

            string predictionKey =
                Environment.GetEnvironmentVariable("VISION_PREDICTION_KEY2");

            string predictionEndpoint =
                Environment.GetEnvironmentVariable("VISION_PREDICTION_ENDPOINT2");

            if (string.IsNullOrEmpty(predictionKey) || string.IsNullOrEmpty(predictionEndpoint))
                throw new InvalidOperationException(
                    "Las credenciales de Azure Custom Vision no están configuradas correctamente.");

            _client = new CustomVisionPredictionClient(
                new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = predictionEndpoint
            };

            return _client;
        }

        public async Task FireAndForgetHttpCallAsync()
        {
            try
            {
                await _httpClient.GetAsync(
                    "https://groceryfirebaseupdater1.azurewebsites.net/api/updateItems");
            }
            catch
            {

            }
        }
    }
}