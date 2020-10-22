using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Team1.Models;

namespace Team1
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetRatings/{userId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "ratingsdata",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery ="SELECT * FROM c WHERE c.userId={userId} ORDER BY c._ts DESC")] IEnumerable<RatingClass> ratings,
            ILogger log,
            string userId)
        {
            log.LogInformation("GetRating HTTP trigger function processed a request");

            if (ratings == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(ratings);
        }
    }
}
