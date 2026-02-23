using Api_IA.Models.Images;
using Newtonsoft.Json;

internal class RequestBody
    {
        [JsonProperty("RequestImages")]
        public List<RequestImages> Images { get; set; }
    }