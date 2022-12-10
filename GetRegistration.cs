using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using w03iotcloud.models;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace MCT.Function
{
    public static class GetRegistration
    {
        [FunctionName("GetRegistration")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "v1/registration")] HttpRequest req,
            ILogger log)
        {
            List<Registration> registration = new List<Registration>();
            string ConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                   try
           {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    string sql = $"SELECT * FROM Registrations";
                    command.CommandText = sql;
                    

                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        Registration reg = new Registration()
                        {
                            RegistrationId = reader["RegistrationId"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Zipcode = Convert.ToInt32(reader["Zipcode"]),
                            Age = Convert.ToInt32(reader["Age"]),
                            IsFirstTimer = Convert.ToInt32(reader["IsFirstTimer"])
                        };
                        registration.Add(reg);
                    }
                }
           }
           catch (System.Exception ex)
           {
                log.LogError(ex.ToString());
                return new StatusCodeResult(500);
           }
            }
        

            return new OkObjectResult(registration);
        }
    }
}
