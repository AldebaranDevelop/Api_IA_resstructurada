using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using System;
using System.Threading.Tasks;
using System.Net.Security;
using System.Drawing;
using System.Text.Json;
using System.Net.Http;
using System.Text;

namespace ApiDrones.infrastructure
{
    public class ApiKraken
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public async Task<string> AsyncResizeImage(string url, int heigth) {
            try
            {
                string krakenUrl = "https://api.kraken.io/v1/url";
                string ApiKey = Environment.GetEnvironmentVariable("KRAKEN_APIKEY");
                string ApiPrivate = Environment.GetEnvironmentVariable("KRAKEN_SECRET");

                var payload = new
                {
                    auth = new
                    {
                        api_key = Environment.GetEnvironmentVariable("KRAKEN_APIKEY"),
                        api_secret = Environment.GetEnvironmentVariable("KRAKEN_SECRET")
                    },
                    url = url,
                    wait = true,
                    resize = new
                    {
                        height = heigth,
                        strategy = "portrait"
                    }
                };
                string payloadJson = JsonSerializer.Serialize(payload);

                var response = await _httpClient.PostAsync(krakenUrl, new StringContent(payloadJson, Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    // Leer el contenido de la respuesta
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Parsear la respuesta JSON
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    // Obtener la URL de la imagen optimizada
                    if (jsonResponse.TryGetProperty("kraked_url", out var krakedUrl))
                    {
                        return krakedUrl.GetString();
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {e.Message}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
            return url;
        }
    }
}
