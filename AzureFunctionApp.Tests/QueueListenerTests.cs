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
        public (TraceWriterStub, Settings, RestClient, AdalHelper) Arrange(IRestResponse fakeResponse)
        {
            var traceWriter = new TraceWriterStub(TraceLevel.Info);

            var fakeSettings = A.Fake<Settings>();
            A.CallTo(() => fakeSettings.Get("crmInstanceUrl"))
                .Returns("https://some.crm.dynamics.com");
            A.CallTo(() => fakeSettings.Get("crmWebApiVersion"))
                .Returns("8.2");

            var fakeRestClient = A.Fake<RestClient>();
            A.CallTo(() => fakeRestClient.ExecuteTaskAsync(A<IRestRequest>.Ignored))
                .Returns(Task.FromResult(fakeResponse));

            var fakeAdalHelper = A.Fake<AdalHelper>();
            A.CallTo(() => fakeAdalHelper.GetBearerTokenAsync())
                .Returns("fake_token");

            return (traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper);
        }

        private static IRestResponse GetFakeResponse(HttpStatusCode httpStatusCode)
        {
            var response = new RestResponse
            {
                StatusCode = httpStatusCode
            };
            return response;
        }

        [TestMethod]
        public async Task TestRunAsyncStatusCodeNoContent()
        {
            //Arrange
            var fakeResponse = GetFakeResponse(HttpStatusCode.NoContent);
            var (traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper) = Arrange(fakeResponse);

            //Act
            await QueueListener.TestableRunAsync("somevalue", traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper);

            //Assert
            var traceEvent = traceWriter.Traces.FirstOrDefault(t => t.Message.Contains("Successfully processed"));
            Assert.IsNotNull(traceEvent);
        }

        [TestMethod]
        public async Task TestRunAsyncStatusCodeNotFound()
        {
            //Arrange
            var fakeResponse = GetFakeResponse(HttpStatusCode.NotFound);
            var (traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper) = Arrange(fakeResponse);

            //Act
            await QueueListener.TestableRunAsync("somevalue", traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper);

            //Assert
            var traceEvent = traceWriter.Traces.FirstOrDefault(t => t.Message.Contains("Successfully processed"));
            Assert.IsNotNull(traceEvent);
        }

        [TestMethod]
        public async Task TestRunAsyncStatusCodeOther()
        {
            //Arrange
            var fakeResponse = GetFakeResponse(HttpStatusCode.Forbidden);
            var (traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper) = Arrange(fakeResponse);

            //Act
            await QueueListener.TestableRunAsync("somevalue", traceWriter, fakeSettings, fakeRestClient, fakeAdalHelper);

            //Assert
            var traceEvent = traceWriter.Traces.FirstOrDefault(t => t.Message.Contains("Something went wrong"));
            Assert.IsNotNull(traceEvent);
        }
    }
}
