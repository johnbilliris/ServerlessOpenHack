using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Team1
{
    public static class GetRatingsViaDocumentClient
    {
        [FunctionName("GetRatingsViaDocumentClient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ratingsdata",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger GetRatings function processed a request.");
 
            string searchTerm = req.Query["userId"];
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return (ActionResult)new NotFoundResult();
            }


            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("ratingsdata", "ratings");

            log.LogInformation($"Searching for: {searchTerm}");

            IDocumentQuery<RatingClass> query = client.CreateDocumentQuery<RatingClass>(collectionUri)
                .Where(p => p.userId.Equals(searchTerm))
                .AsDocumentQuery();

            if (query == null)
            {
                log.LogInformation($"Rating items not found");
                return new BadRequestResult();
            }
            else
            {
                log.LogInformation($"Found Ratings item");
                List<RatingClass> results = new List<RatingClass>();
                while (query.HasMoreResults)
                {
                    foreach (RatingClass result in await query.ExecuteNextAsync())
                    {
                        results.Add(result);
                    }
                }

                return new OkObjectResult(results);
            }
        }
    }
}
