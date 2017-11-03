using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace CrmAsyncRequestResponseSample.Plugins
{
    // ReSharper disable once InconsistentNaming
    [CrmPluginRegistration("Create",
    "dkdt_asyncrequestresponsesample", StageEnum.PostOperation, ExecutionModeEnum.Synchronous,
    "","dkdt_asyncrequestresponsesample create", 1,
    IsolationModeEnum.Sandbox
    ,Description = "dkdt_asyncrequestresponsesample create"
    ,Id = "0d392320-e6b5-e611-80f3-c4346bac2908"
    )]
    public class dkdt_asyncrequestresponsesample_create : IPlugin
    {
        private readonly string _baseAddress;
        private readonly string _token;
        private readonly string _sasKeyName;
        private readonly string _sasKeyValue;
        private readonly string _queueName;
        private bool _hasConfiguration;

        // ReSharper disable once UnusedParameter.Local
        public dkdt_asyncrequestresponsesample_create(string unsecureConfig, string secureConfig)
        {
            // TODO: Debug parsing
            // Parse connection string
            if (secureConfig != String.Empty)
            {
                var keyValueArray = secureConfig.Split(';');
                _baseAddress = keyValueArray[0].Split(new[] { '=' }, 2)[1].Replace("sb", "https");
                _sasKeyName = keyValueArray[1].Split(new[] { '=' }, 2)[1];
                _sasKeyValue = keyValueArray[2].Split(new[] { '=' }, 2)[1];
                _queueName = keyValueArray[3].Split(new[] { '=' }, 2)[1];

                _token = BuildSasToken();

                _hasConfiguration = true;
            }
            else
            {
                _hasConfiguration = false;
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var webClient = WebClientFactory.Create();
            TestableExecute(serviceProvider, webClient);
        }

        public void TestableExecute(IServiceProvider serviceProvider, IWebClient webClient)
        {
            if (_hasConfiguration)
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                try
                {
                    // Instead of using the built in CRM integration with Azure Service Bus (ASB),
                    // we are going to put a message in a queue directly using the ASB REST API.
                    // Our queue processing code doesn't need everything in the IPluginExecutionContext.
                    // Therefore, we can increase performance by sending *only* the data our queue processing
                    // code requires.  We gain performance in serialization/deserialization of
                    // queue message *and* speed over the wire since the data payload will be much smaller.
                    SendMessageToQueue($"{context.PrimaryEntityId}", webClient);
                }
                catch (Exception e)
                {
                    throw new InvalidPluginExecutionException(e.Message);
                }
            }
        }

        // Code below adapted from https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-brokered-tutorial-rest
        private string BuildSasToken()
        {
            var fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + 3600);
            var stringToSign = WebUtility.UrlEncode(_baseAddress) + "\n" + expiry;
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_sasKeyValue));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            return String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                WebUtility.UrlEncode(_baseAddress), WebUtility.UrlEncode(signature), expiry, _sasKeyName);
        }

        private void SendMessageToQueue(string body, IWebClient webClient)
        {
            var fullAddress = _baseAddress + _queueName + "/messages" + "?timeout=60&api-version=2013-08 ";
            webClient.Headers.Add(HttpRequestHeader.Authorization, _token);
            webClient.UploadData(fullAddress, "POST", Encoding.UTF8.GetBytes(body));
        }
    }
}