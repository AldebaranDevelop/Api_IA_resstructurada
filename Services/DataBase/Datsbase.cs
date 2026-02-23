
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDrones.infrastructure;

public class DataBaseWork
{
    private readonly string _connectionString;
    public DataBaseWork(IConfiguration configuration)
    {
        string connectionString = configuration.GetValue<string>("ConnectionStrings:ContentPaneCS");

        _connectionString = connectionString;
    }
    public async Task<List<TaskStoreBD>> GetTaskStore(string store_id,string aisle_id, string operator_id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("SP_GET_TASKS", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@store_id", store_id);
                command.Parameters.AddWithValue("@aisle_id", aisle_id);
                command.Parameters.AddWithValue("@operator_id", operator_id);

                connection.Open();

                using(SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if(!reader.HasRows)
                        return new List<TaskStoreBD>();

                    List<TaskStoreBD> tasks = new List<TaskStoreBD>();

                    while (await reader.ReadAsync())
                    {
                        TaskStoreBD task = new TaskStoreBD()
                        {
                            task_id = reader["task_id"] != DBNull.Value ? Convert.ToInt32(reader["task_id"]) : 0,
                            task_completion = reader["task_completion"] != DBNull.Value ? Convert.ToInt32(reader["task_completion"]) : 0,
                            task_priority = reader["task_priority"] != DBNull.Value ? Convert.ToInt32(reader["task_priority"].ToString()) : 0,
                            missing_item_name = reader["task_missing_item_name"] != DBNull.Value ? reader["task_missing_item_name"].ToString() : string.Empty,
                            missing_item_tag = reader["task_missing_item_tag"] != DBNull.Value ? reader["task_missing_item_tag"].ToString() : string.Empty,
                            camera_input = reader["task_missing_item_image_url"] != DBNull.Value ? reader["task_missing_item_image_url"].ToString() : string.Empty,
                            task_original_item_url = reader["task_original_item_url"] != DBNull.Value ? reader["task_original_item_url"].ToString() : string.Empty,
                            item_sku = reader["task_item_sku"] != DBNull.Value ? Convert.ToInt32(reader["task_item_sku"].ToString()) : 0,
                            missing_since = reader["task_last_seen"] != DBNull.Value ? reader["task_last_seen"].ToString() : string.Empty,
                            aisle_name = reader["aisle_name"] != DBNull.Value ? reader["aisle_name"].ToString() : string.Empty,
                            operator_name = reader["operator_name"] != DBNull.Value ? reader["operator_name"].ToString() : string.Empty,
                            store_id = reader["store_id"] != DBNull.Value ? Convert.ToInt32(reader["store_id"].ToString()) : 0,
                            aisle_id = reader["aisle_id"] != DBNull.Value ? Convert.ToInt32(reader["aisle_id"].ToString()) : 0,
                            operator_id = reader["operator_id"] != DBNull.Value ? Convert.ToInt32(reader["operator_id"].ToString()) : 0,
                            execution_id = reader["execution_id"] != DBNull.Value ? Convert.ToInt32(reader["execution_id"].ToString()) : 0,
                            task_creation_date = reader["task_creation_date"] != DBNull.Value ? Convert.ToDateTime(reader["task_creation_date"]) : DateTime.MinValue,
                            task_completion_date = reader["task_completion_date"] != DBNull.Value ? Convert.ToDateTime(reader["task_completion_date"]) : DateTime.MinValue,
                            task_meaningful_id = reader["task_meaningful_id"] != DBNull.Value ? reader["task_meaningful_id"].ToString() : string.Empty,
                            unseen_count = reader["task_occurrences"] != DBNull.Value ? Convert.ToInt32(reader["task_occurrences"].ToString()) : 0,
                            task_reason = reader["task_reason"] != DBNull.Value ? reader["task_reason"].ToString() : string.Empty,
                        };

                        BoundingBox boundingBoxAux = new BoundingBox()
                        {
                            Left = reader["left_x"] != DBNull.Value ? Convert.ToDouble(reader["left_x"]) : 0,
                            Top = reader["top_y"] != DBNull.Value ? Convert.ToDouble(reader["top_y"]) : 0,
                            Width = reader["width_x"] != DBNull.Value ? Convert.ToDouble(reader["width_x"]) : 0,
                            Height = reader["height_y"] != DBNull.Value ? Convert.ToDouble(reader["height_y"]) : 0,
                        };
                        task.boundingBox = boundingBoxAux;

                        tasks.Add(task);
                    }
                    return tasks;
                }
            }
        }
    }

    public async Task<string> GetNameStore(string store_id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("SP_GET_STORE_NAME", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@store_id", store_id);
                connection.Open();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows)
                        return "";

                    string store_name = "";

                    while (await reader.ReadAsync())
                    {
                        store_name = reader["store_name"] != DBNull.Value ? reader["store_name"].ToString() : string.Empty;
                    }

                    return store_name;
                }
            }
        }
    }
}
