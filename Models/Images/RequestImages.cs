using Newtonsoft.Json;

namespace Api_IA.Models.Images
{
    public class RequestImages
    {
        [JsonProperty("image_name")]
        public string Image_name { get; set; }

        [JsonProperty("image_url")]
        public string Image_url { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}
