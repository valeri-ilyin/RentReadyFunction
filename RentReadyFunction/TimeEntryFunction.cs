using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RentReady.Server;
using System.Configuration;

namespace RentReadyFunction
{
    public static class TimeEntryFunction
    {
        [FunctionName("TimeEntryFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var processor = new TimeEntryRequestProcessor(log);
            return await processor.ProcessAsync(requestBody);
        }
    }
}
