using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

public class ProcessedImages2
    {
        [Key]
        [JsonProperty("processed_image_id")]
        public int Processed_image_id { get; set; }

        [JsonProperty("image_url")]
        public string Image_url { get; set; }

        [JsonProperty("image_name")]
        public string Image_name { get; set; }

        [JsonProperty("execution_id")]
        public int Execution_id { get; set; }
    }