using Api_IA.Interfaces;
using ApiDrones.infrastructure;
using ApiDrones.Models;
using ApiDrones.Repositories.Interface;
using ApiDrones.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task<IActionResult> CustomVisionPrediction(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
                return new BadRequestObjectResult("Missing body");

            var request = JsonConvert.DeserializeObject<RequestBody>(body);

            if (request?.Images == null || request.Images.Count == 0)
                return new BadRequestObjectResult("Missing image array");

            var results = await _dronImg.CustomVisionAsyncTestMain(request.Images);

            if (results?.Any() != true)
                return new BadRequestObjectResult("No predictions");

            var items = await _ExecutionsRepository.GetResponseItems();

            var foundProducts = items.Select(x => x.Name).ToList();

            var response = new
            {
                status = "OK",
                found_products = foundProducts
            };

            _ = Task.Run(() =>
            {
                var fire = new ApiCustomVision();
                return fire.FireAndForgetHttpCallAsync();
            });

            return new OkObjectResult(response);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult("Invalid JSON");
        }
        catch (Exception)
        {
            return new StatusCodeResult(400);
        }
    }

    [FunctionName("StoreInfo")]
    public async Task<IActionResult> StoreInfo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        string storeIdStr = req.Query["store_id"];
        string productIdStr = req.Query["aisle_id"];
        string operatorIdStr = req.Query["operator_id"];

        if (string.IsNullOrEmpty(storeIdStr)) return new BadRequestObjectResult("Missing parameter 'store_id'");

        return await _dronImg.GetStore(_configuration, storeIdStr, productIdStr, operatorIdStr);
    }
}