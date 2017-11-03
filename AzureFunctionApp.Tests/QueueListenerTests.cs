using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using FakeItEasy;
using RestSharp;
using System.Net;

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

            var fakeSettings = A.Fake<Settings>();
            A.CallTo(() => fakeSettings.Get("crmInstanceUrl"))
            .Returns("https://some.crm.dynamics.com");
            A.CallTo(() => fakeSettings.Get("crmWebApiVersion"))
            .Returns("8.2");

            var fakeRestClient = A.Fake<RestClient>();
            var fakeResponse = GetFakeResponse();
            A.CallTo(() => fakeRestClient.ExecuteTaskAsync(A<IRestRequest>.Ignored))
            .Returns(Task.FromResult(fakeResponse));

            var fakeAdalHelper = A.Fake<AdalHelper>();
            A.CallTo(() => fakeAdalHelper.GetBearerTokenAsync())
            .Returns("fake_token");

            //Act
            await QueueListener.TestableRunAsync("somevalue", traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper);

            //Assert
            var traceEvent = traceWriter.Traces.Where(t => t.Message.Contains("Successfully processed")).FirstOrDefault();
            Assert.IsNotNull(traceEvent);
        }

        private IRestResponse GetFakeResponse()
        {
            var response = new RestResponse
            {
                StatusCode = HttpStatusCode.NoContent
            };
            return response as IRestResponse;
        }

    }
}
