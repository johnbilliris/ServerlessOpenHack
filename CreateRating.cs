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
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rating bodyRating = JsonConvert.DeserializeObject<Rating>(requestBody);
            
            if(bodyRating is null)
                return new BadRequestResult();
            
            try{
                var user = await ValidateUserIdAsync(bodyRating.UserId);
                log.LogInformation($"User validated: {user.FullName}");
            } catch(Exception)
            {
                log.LogInformation("Couldn't validate user");
                return new BadRequestResult();
            }

            try{
                var product = await ValidateProductIdAsync(bodyRating.ProductId);
                log.LogInformation($"Product validated: {product.ProductName}");
            } catch(Exception)
            {
                log.LogInformation("Couldn't validate product");
                return new BadRequestResult();
            }

            string responseMessage = $"completed";
            /*string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
                */

            return new OkObjectResult(responseMessage);
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
