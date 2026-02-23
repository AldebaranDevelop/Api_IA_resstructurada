using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDrones.Models
{
    public class Predictions2
    {
        [Key]
        [JsonProperty("prediction_id")]
        public int Prediction_id { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("probability")]
        public double Probability { get; set; }

        [JsonProperty("left_x")]
        public double Left_x { get; set; }

        [JsonProperty("top_y")]
        public double Top_y { get; set; }

        [JsonProperty("height_y")]
        public double Height_y { get; set; }

        [JsonProperty("width_x")]
        public double Width_x { get; set; }

        [JsonProperty("processed_image_id")]
        public int Processed_image_id { get; set; }

        [JsonProperty("execution_id")]
        public int Execution_id { get; set; }

    }
}
