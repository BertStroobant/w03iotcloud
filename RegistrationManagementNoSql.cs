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
    public static class RegistrationManagementNoSql
    {
        [FunctionName("RegistrationManagementNoSql")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration reg = JsonConvert.DeserializeObject<Registration>(requestBody);

            return new OkObjectResult(reg);
        }
    }
}
