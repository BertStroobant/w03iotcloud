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
using Azure.Data.Tables;

namespace MCT.Function
{
    public static class RegistrationManagementNoSql
    {
        [FunctionName("RegistrationManagementNoSql")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v2/registrations")] HttpRequest req,
            ILogger log)
        {
            try 
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Registration reg = JsonConvert.DeserializeObject<Registration>(requestBody);
                string accountName = Environment.GetEnvironmentVariable("AccountName");
                string storageAccountKey = Environment.GetEnvironmentVariable("StorageAccountKey");
                string storageUri = Environment.GetEnvironmentVariable("StorageUri");
                string partitionKey = "registrations";
                string rowKey = Guid.NewGuid().ToString();
                var tableClient = new TableClient
                (new Uri(storageUri),
                    "registrations",
                    new TableSharedKeyCredential(accountName, storageAccountKey));
                await tableClient.CreateIfNotExistsAsync();
                var entity = new TableEntity(partitionKey, rowKey)
                {
                    {"Age", reg.Age},
                    {"FirstName", reg.FirstName},
                    {"LastName", reg.LastName},
                    {"Email", reg.Email},
                    {"Zipcode", reg.Zipcode},
                    {"IsFirstTimer", reg.IsFirstTimer}
                };
                tableClient.AddEntity(entity);
                return new OkObjectResult(reg);
                    
                
            }
            catch (System.Exception ex)
            {
                log.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }

            

            
        }
    }
}
