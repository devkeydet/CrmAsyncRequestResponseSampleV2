using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.QualityTools.Testing.Fakes;
using AzureFunctionApp.Fakes;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Fakes;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RestSharp.Fakes;
using RestSharp;
using System.Net;
using System.Linq;

namespace AzureFunctionApp.Tests
{
    [TestClass]
    public class QueueListenerTests
    {
        [TestMethod]
        public async Task TestRunAsync()
        {
            //Arrange
            var traceWriter = new TraceWriterStub(TraceLevel.Info);

            // Beint lazy and using Fakes
            // TODO: Consider refactoring to not require Fakes
            using (ShimsContext.Create())
            {
                //    ShimServiceBusTriggerAttribute.ConstructorStringAccessRights = (ServiceBusTriggerAttribute attr, string a, AccessRights ar) =>
                //    {
                //        //Do Nothing
                //    };

                //    ShimServiceBusTriggerAttribute.AllInstances.ConnectionSetString = (ServiceBusTriggerAttribute attr, string c) =>
                //    {
                //       //Do Nothing
                //    };

                //    ShimServiceBusTriggerAttribute.AllInstances.AccessSetAccessRights = (ServiceBusTriggerAttribute attr, AccessRights c) =>
                //    {
                //        //Do Nothing
                //    };
                ShimSettings.GetString = (string name) =>
                {
                    switch (name)
                    {
                        case "crmInstanceUrl": return "https://some.crm.dynamics.com";
                        case "crmWebApiVersion": return "8.2";
                    }
                    return "";
                };

                ShimAuthenticationContext.ConstructorStringTokenCache = (AuthenticationContext a, string authority, TokenCache tc) =>
                {
                    //Do Nothing
                };

                ShimAdalHelper.GetBearerTokenAsync = () =>
                {
                    return Task.FromResult("fake_token");
                };

                ShimRestClient.AllInstances.ExecuteTaskAsyncIRestRequest = (RestClient r, IRestRequest rr) =>
                {
                    var response = new RestResponse
                    {
                        StatusCode = HttpStatusCode.NoContent
                    };
                    return Task.FromResult(response as IRestResponse);
                };

                //Act
                await QueueListener.RunAsync("somevalue", traceWriter);

                //Assert
                var traceEvent = traceWriter.Traces.Where(t=>t.Message.Contains("Successfully processed")).First();
                Assert.IsNotNull(traceEvent);
            }
        }
    }
}
