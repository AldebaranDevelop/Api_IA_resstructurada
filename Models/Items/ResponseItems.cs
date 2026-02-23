using Newtonsoft.Json;

namespace Api_IA.Models.Items
{
    public class ResponseItems
    {

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image_url")]
        public string Image_url { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
