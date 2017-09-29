using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFunctionApp
{
    public static class QueueListener
    {
        private static readonly string _crmInstanceUrl = Settings.Get("crmInstanceUrl");
        private static readonly object _crmWebApiVersion = Settings.Get("crmWebApiVersion");
        private static readonly Uri _baseUri = new Uri($"{_crmInstanceUrl}/api/data/v{_crmWebApiVersion}");

        [FunctionName("QueueListener")]
        public static async Task RunAsync([ServiceBusTrigger("crmqueue", AccessRights.Listen, Connection = "sbconn")]string myQueueItem, TraceWriter log)
        {
            var primaryEntityId = myQueueItem;
            log.Info($"Begin processing: {primaryEntityId}");

            // Pretend more data was passed in the message and there is more processing needed here
            // or perhaps we need to call a web service, etc.
            Thread.Sleep(2000);

            var restClient = new RestClient(_baseUri);
            var token = await AdalHelper.GetBearerTokenAsync();
            restClient.AddDefaultHeader("Authorization", $"Bearer {token}");

            var entity = $"dkdt_asyncrequestresponsesamples({primaryEntityId})";

            var request = new RestRequest(entity, Method.PATCH)
            {
                RequestFormat = DataFormat.Json
            };

            restClient.AddDefaultHeader("If-Match", "*"); //Ensure PATCH fails if the entity is already deleted.

            request.AddBody(new
            {
                dkdt_updatefromazurecodecomplete = true,
                dkdt_responsefromazurecode = "WHATEVER DATA YOU WANT TO SEND BACK"
            });

            var response = await restClient.ExecuteTaskAsync(request);

            // If the update was successfulr or the entity has already been deleted, then
            // call CompleteAsync() to delete the message from the queu
            if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
            {
                log.Info($"Successfully processed: {primaryEntityId}");
            }
            else
            {
                log.Info($"Something went wrong processing: {primaryEntityId}");
                log.Info($"StatusCode: {response.StatusCode}");
                log.Info("Content:");
                log.Info(response.Content);
                log.Info("");
            }
        }
    }
}
