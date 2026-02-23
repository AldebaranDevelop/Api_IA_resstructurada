using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDrones.Models
{
    public class Images
    {
        [Key]
        [JsonProperty("image_id")]
        public int Image_id { get; set; }

        [JsonProperty("image_storage")]
        public string Image_storage { get; set; }

        [JsonProperty("image_name")]
        public string Image_name { get; set; }

        [JsonProperty("execution_id")]
        public int Execution_id { get; set; }

        [JsonProperty("processed")]
        public short Processed { get; set; }

        [JsonProperty("image_url")]
        public string Image_url { get; set; }

        [JsonProperty("sql_created_time")]
        public DateTime Sql_created_time { get; set; }

        [JsonProperty("prediction_ready")]
        public bool Prediction_ready { get; set; }
    }
}
