using ApiDrones.Models.Api_IA;
using Newtonsoft.Json;

public class Operator
{
    [JsonProperty("operator_name")]
    public string OperatorName {get; set;}

    [JsonProperty("operator_id")]
    public string OperatorId {get; set;}
    [JsonProperty("Tasks")]
    public List<ResponseTask> Tasks {get; set;}
}