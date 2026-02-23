

using Api_IA.Interfaces;
using Api_IA.Models.Images;
using Api_IA.Models.Items;
using ApiDrones.infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ApiDrones.Controllers;
public class Function_App
{
    private readonly IConfiguration _configuration;
    private readonly IDronImg _dronImg;
    private readonly IExecutionsRepository _ExecutionsRepository;

    public Function_App(IConfiguration configuration, IDronImg dronImg, IExecutionsRepository ExecutionsRepository)
    {
        _configuration = configuration;
        _dronImg = dronImg;
        _ExecutionsRepository = ExecutionsRepository;
    }

    [FunctionName("customVisionPrediction")]
    public async Task<IActionResult> customVisionPrediction([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req)
    {
        string data = await new StreamReader(req.Body).ReadToEndAsync();
            
        if (string.IsNullOrEmpty(data))
            return new BadRequestObjectResult("Missing Body");

        try
        {
            RequestBody requestBody = JsonConvert.DeserializeObject<RequestBody>(data);

            List<RequestImages> imageArray = requestBody?.Images;

            if(imageArray == null)
                return new BadRequestObjectResult("Missing image array from body");

            List<bool> isCorrect = await _dronImg.CustomVisionAsyncTestMain(imageArray);

           if (isCorrect != null && isCorrect.Any())
            {
                List<ResponseItems> Response = await _ExecutionsRepository.GetResponseItems();
                List<string> found_products = new List<string>();
                foreach (var item in Response) {
                    found_products.Add(item.Name);
                }
                var jsonObject = new
                {
                    status = "OK",
                    found_products = found_products
                };
                var jsonResponse = JsonConvert.SerializeObject(jsonObject);

                Console.WriteLine("Llegamos a la parte del fuego y dispara");
                var FireAndForget = new ApiCustomVision();
                await FireAndForget.FireAndForgetHttpCallAsync();
                Console.WriteLine("Salimos de la parte del fuego y dispara");
                return new OkObjectResult(new { jsonObject, status_code = 200, mimetype = "application/json"});
            }

        }
        catch (Exception )
        {
            return new BadRequestObjectResult(JsonConvert.SerializeObject(new { status = "Invalid JSON", status_code = 400, mimetype = "application/json" }));
                
        }
        return new BadRequestObjectResult(JsonConvert.SerializeObject(new { status="OK", status_code = 400, mimetype = "application/json"}));

    }

    [FunctionName("StoreInfo")]
    public async Task<IActionResult> StoreInfo(
        [HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = null)] HttpRequest req)
    {
        string storeIdStr = req.Query["store_id"];
        string productIdStr = req.Query["aisle_id"];
        string operatorIdStr = req.Query["operator_id"];

        if (string.IsNullOrEmpty(storeIdStr) ) return new BadRequestObjectResult("Missing parameter 'store_id'");

        return await _dronImg.GetStore(_configuration, storeIdStr, productIdStr, operatorIdStr);
    }
}
