using Api_IA.Models.Items;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace Api_IA.Interfaces
{
    public interface IExecutionsRepository
    {
        Task<int> AsyncTestPostmanInsertLogBD();
        Task<int> AsyncTestPostmanInsertImageBd(int last_row_id, string image_name, string image_dir);
        Task CustomVisionAsyncTestInsertPredictionsToDb(ImagePrediction predictions, string image_name, int execution_id, string image_url);
        Task UpdateImage(int image_id);
        Task<List<ResponseItems>> GetResponseItems();
    }
}
