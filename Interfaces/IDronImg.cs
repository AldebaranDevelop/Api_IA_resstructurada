using Microsoft.AspNetCore.Mvc;
using Api_IA.Models.Images;

namespace Api_IA.Interfaces
{
    public interface IDronImg
    {
        Task<IActionResult> GetStore(IConfiguration configuration, string store_id, string aisle_id, string operator_id);
        Task<List<bool>> CustomVisionAsyncTestMain(List<RequestImages> Image);
        Task<bool> CustomVisionAsyncTestUploadImgToBlob(string image_name, byte[] image, string url, string content_type, int last_inserted_id, bool resize_flag, int img_height);

    }

}
