using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

public class TaskStoreBD
    {
        public int task_id { get; set; }
        public int task_completion { get; set; }
        public int task_priority { get; set; }
        public string missing_item_name { get; set;}
        public string missing_item_tag { get; set; }
        public string camera_input { get; set; }
        public string task_original_item_url { get; set; }
        public int item_sku { get; set; }
        public string missing_since { get; set; }
        public string aisle_name { get; set; }
        public string operator_name { get; set; }
        public int store_id { get; set; }
        public int aisle_id { get; set; }
        public int operator_id { get; set; }
        public int execution_id { get; set; }
        public DateTime task_creation_date { get; set; }
        public DateTime task_completion_date { get;set; }
        public string task_meaningful_id { get; set; }
        public int unseen_count { get; set; }
        public BoundingBox boundingBox { get; set; }
        public string task_reason {  get; set; }

    }