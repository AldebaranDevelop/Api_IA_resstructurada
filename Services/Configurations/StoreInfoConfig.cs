
using ApiDrones.Models.Api_IA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDrones.Configurations;

public class StoreInfoConfig
{
    public StoreInfoConfig() { }

    public async Task<ResponseTasks> OrderFinalTasks(List<TaskStoreBD> tasks, string storeName, string store_id)
    {
        ResponseTasks responseTasks = new ResponseTasks();
        StoreInfo storeInfo = new StoreInfo()
        {
            StoreId = store_id,
            StoreName = storeName,
        };
        responseTasks.StoreInfo = storeInfo;

        List<Operator> Operators = new List<Operator>();
        var groupedTasks = tasks.GroupBy(t => t.operator_name);

        foreach ( var group in groupedTasks)
        {
            Operator operatorTemp = new Operator();
            List<ResponseTask> finalTasks = new List<ResponseTask>();

            foreach (var task in group)
            {
                ResponseTask responseTask = new ResponseTask()
                {
                    task_id = task.task_id,
                    execution_id = task.execution_id,
                    missing_item_name = task.missing_item_name,
                    missing_item_tag = task.missing_item_tag ,
                    item_sku= task.item_sku,
                    missing_since = task.missing_since,
                    task_original_item_url = task.task_original_item_url,
                    camera_input = task.camera_input,
                    task_priority = task.task_priority,
                    unseen_count = task.unseen_count,
                    aisle_name = task.aisle_name,
                    boundingBox = task.boundingBox 
                };

                finalTasks.Add(responseTask);
                
            }
            var operatorId = tasks.GroupBy(t => t.operator_name).
                FirstOrDefault(g => g.Key == group.Key).
                Select(m => m.operator_id).Distinct().ToList()[0];

            operatorTemp.OperatorName = group.Key.ToString();
            operatorTemp.OperatorId = operatorId.ToString();
            operatorTemp.Tasks = finalTasks;
            Operators.Add(operatorTemp);
        }
        responseTasks.Operators = Operators;

        return responseTasks;
    }


}
