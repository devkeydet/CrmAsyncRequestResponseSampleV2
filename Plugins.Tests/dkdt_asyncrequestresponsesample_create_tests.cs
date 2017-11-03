using CrmAsyncRequestResponseSample.Plugins;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace Plugins.Tests
{
    [TestClass]
    public class dkdt_asyncrequestresponsesample_create_tests
    {
        [TestMethod]
        public void TestExecuteWithConfiguration()
        {
            // Arrange
            var fakeServiceProvider = A.Fake<IServiceProvider>();
            var fakePluginExecutionContext = A.Fake<IPluginExecutionContext>();
            var fakeWebClient = A.Fake<IWebClient>();
            A.CallTo(
                () => fakeServiceProvider.GetService(typeof(IPluginExecutionContext))
            ).Returns(fakePluginExecutionContext);
            A.CallTo(
                () => fakePluginExecutionContext.PrimaryEntityId
            ).Returns(Guid.NewGuid());
            A.CallTo(
                () => fakeWebClient.UploadData(A<string>.Ignored, A<string>.Ignored, A<byte[]>.Ignored)
            ).Returns(null);

            // Act
            var plugin = new dkdt_asyncrequestresponsesample_create(
                "",
                "Endpoint=sb://some.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rWrelB5AUL9GfOp0tCCCHeClWngq827UQI/3C5N9ts0=;EntityPath=somequeue"
            );
            plugin.TestableExecute(fakeServiceProvider, fakeWebClient);

            //Assert
            A.CallTo(
                () => fakeWebClient.UploadData(A<string>.Ignored, A<string>.Ignored, A<byte[]>.Ignored)
            ).MustHaveHappened();
        }
    }
}
