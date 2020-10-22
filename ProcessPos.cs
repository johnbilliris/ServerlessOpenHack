using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Team1.Models;

namespace Team1
{
    public static class ProcessPos
    {
        [FunctionName("ProcessPos")]
        public static void Run(
            [EventHubTrigger("icrapi", Connection = "EventHubConnection")] EventData[] events,
            [CosmosDB(
                databaseName: "ratingsdata",
                collectionName: "orders",
                ConnectionStringSetting = "CosmosDBConnection")] out dynamic cosmosDoc,
             ILogger log)
        {
            var exceptions = new List<Exception>();
            cosmosDoc = null;

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var salesOrder = JsonConvert.DeserializeObject<SalesOrder>(messageBody);
                    salesOrder.id = Guid.NewGuid().ToString();
                    cosmosDoc = salesOrder;
                    log.LogInformation($"sales order id: {salesOrder.id}, sales number: {salesOrder.header.salesNumber}");
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
