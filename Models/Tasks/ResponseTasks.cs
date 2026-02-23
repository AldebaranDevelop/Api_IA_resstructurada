using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Newtonsoft.Json;

namespace ApiDrones.Models.Api_IA
{
    public class ResponseTask
    {
        public int task_id { get; set; }
        public int execution_id { get; set; }
        public string missing_item_name { get; set; }
        public string missing_item_tag { get; set; }
        public int item_sku { get; set; }
        public string missing_since { get; set; }
        public string task_original_item_url { get; set; }
        public string camera_input { get; set; }
        public int task_priority { get; set; }
        public int unseen_count { get; set; }
        public string aisle_name { get; set; }
        public BoundingBox boundingBox { get; set; }
    }
    public class ResponseTasks
    {
        [JsonProperty("store_info")]
        public StoreInfo StoreInfo { get; set; }

        [JsonProperty("operators")]
        public List<Operator> Operators { get; set; }
    }
}