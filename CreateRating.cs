using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Team1
{
    public static class CreateRating
    {
        private static readonly HttpClient client = new HttpClient();

        [FunctionName("CreateRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ratingsdata",
                collectionName: "ratingsV2",
                ConnectionStringSetting = "CosmosDBConnection")] out dynamic cosmosDoc,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            RatingClass bodyRating = JsonConvert.DeserializeObject<RatingClass>(requestBody);
            
            cosmosDoc = null;

            if(bodyRating is null)
                return new BadRequestResult();
            
            try{
                Task.Run(() => ValidateUserIdAsync(bodyRating.userId));
                log.LogInformation($"User validated");
            } catch(Exception)
            {
                log.LogInformation("Couldn't validate user");
                return new BadRequestResult();
            }

            try{
                Task.Run(() => ValidateProductIdAsync(bodyRating.productId));
                log.LogInformation($"Product validated");
            } catch(Exception)
            {
                log.LogInformation("Couldn't validate product");
                return new BadRequestResult();
            }

            bodyRating.id = Guid.NewGuid().ToString();
            bodyRating.timeStamp = DateTime.UtcNow;

            if(bodyRating.rating < 0 || bodyRating.rating > 5)
                return new BadRequestResult();
            
            cosmosDoc = bodyRating;
            log.LogInformation($"Saving rating to CosmosDB: {cosmosDoc}");

            return new OkObjectResult(bodyRating);
        }

        private static async Task<User> ValidateUserIdAsync(string userId)
        {
            var stringUser = await client.GetStringAsync($"https://serverlessohuser.trafficmanager.net/api/GetUser?userId={userId}");

            var user = JsonConvert.DeserializeObject<User>(stringUser);
            return user;
        }

        private static async Task<Product> ValidateProductIdAsync(string productId)
        {
            var stringProduct = await client.GetStringAsync($"https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={productId}");
            var product = JsonConvert.DeserializeObject<Product>(stringProduct);
            return product;
        }
    }
}
