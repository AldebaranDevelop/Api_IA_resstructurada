using Newtonsoft.Json;

public class StoreInfo
    {
        [JsonProperty("StoreId")]
        public string StoreId { get; set; }

        [JsonProperty("StoreName")]
        public string StoreName { get; set; }
    }