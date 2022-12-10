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
using Azure.Identity;

namespace MCT.Function
{
    public static class AddRegistration
    {
        [FunctionName("AddRegistration")]
        public static async Task<IActionResult> addRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/registrations")] HttpRequest req,
            ILogger log)
        {
            var credential = new DefaultAzureCredential();
            var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[]{"https://database.windows.net/.default"}));
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration data = JsonConvert.DeserializeObject<Registration>(requestBody);
            List<Registration> reg = new List<Registration>();
            string ConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using (SqlConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString))
            {
                try
                {
                    connection.AccessToken = token.Token;
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        Guid obj = Guid.NewGuid();
                        string sql = $"INSERT INTO Registrations (RegistrationId, FirstName, LastName, Email, Zipcode, Age, IsFirstTimer) VALUES('{obj}','{data.FirstName}','{data.LastName}','{data.Email}',{data.Zipcode},{data.Age},{data.IsFirstTimer})";
                        command.CommandText = sql;

                        int rowCount = await command.ExecuteNonQueryAsync();
                        System.Console.WriteLine(rowCount);
                    }
                }

                catch (System.Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new StatusCodeResult(500);
                }
            }


            return new OkObjectResult("Toevoegen geslaagd");
        }
    }
}
