using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDrones.Models
{
    public class Executions
    {
        [Key]
        [JsonProperty("execution_id")]
        public int Execution_id { get; set; }

        [JsonProperty("execution_date")]
        public DateTime Execution_date { get; set; }

        [JsonProperty("execution_routine")]
        public string Execution_routine { get; set; }

        [JsonProperty("store")]
        public string Store { get; set; }

        [JsonProperty("drone")]
        public string Drone { get; set; }

        [JsonProperty("routine_id")]
        public int Routine_id { get; set; }

        [JsonProperty("processed")]
        public short Processed { get; set; }

        [JsonProperty("sql_created_time")]
        public DateTime? Sql_created_time { get; set; }

        [JsonProperty("prediction_ready")]
        public bool Prediction_ready { get; set; }

    }
}
