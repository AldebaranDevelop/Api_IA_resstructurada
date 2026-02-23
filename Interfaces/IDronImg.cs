using Api_IA.Models.Images;
using ApiDrones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDrones.Services.Interface
{
    public interface IDronImg
    {
        Task<List<bool>> CustomVisionAsyncTestMain(List<RequestImages> Image);
        Task<bool> CustomVisionAsyncTestUploadImgToBlob(string image_name, byte[] image, string url, string content_type, int last_inserted_id, bool resize_flag, int img_height);
        Task<IActionResult> GetStore(IConfiguration configuration, string store_id, string aisle_id, string operator_id);
    }
}
