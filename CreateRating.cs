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
            
            var user = await ValidateUserIdAsync(bodyRating.UserId);

            string responseMessage = $"User: {user.FullName}";
            /*string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
                */

            return new OkObjectResult(responseMessage);
        }

        private static async Task<User> ValidateUserIdAsync(string userId)
        {
            var httpClient = new HttpClient();
            var stringUser = await httpClient.GetStringAsync($"https://serverlessohuser.trafficmanager.net/api/GetUser?userId={userId}");

            var user = JsonConvert.DeserializeObject<User>(stringUser);
            return user;

        }
    }
}
