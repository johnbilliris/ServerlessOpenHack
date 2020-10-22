using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Team1.Models;

namespace Team1
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ratingsdata",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{Query.id}",
                PartitionKey = "{Query.userId}")] RatingClass rating,
            ILogger log)
        {
            log.LogInformation("GetRating HTTP trigger function processed a request");

            if (rating == null)
            {
                log.LogInformation($"Rating item not found");
                return new BadRequestResult();
            }
            else
            {
                log.LogInformation($"Found Rating item, Description={rating.id}");
                return new OkObjectResult(rating);
            }
        }
    }
}
