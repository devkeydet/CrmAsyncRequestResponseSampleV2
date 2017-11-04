using CrmAsyncRequestResponseSample.Plugins;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace Plugins.Tests
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class dkdt_asyncrequestresponsesample_create_tests
    {
        // NOTE:    Using FakeItEasy (http://fakeiteasy.readthedocs.io) for these tests because this plugin is not involved.
        //          However, you might want to look at one of the following:
        //          Fake Xrm Easy (https://dynamicsvalue.com/get-started/overview)
        //          spkl.fakes (https://github.com/scottdurow/SparkleXrm/wiki/spkl.fakes)
        //          for advanced plugin unit testing.
        // ReSharper disable once InconsistentNaming
        private const string _constSecureConfig = "Endpoint=sb://some.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rWrelB5AUL9GfOp0tCCCHeClWngq827UQI/3C5N9ts0=;EntityPath=somequeue";

        [TestMethod]
        public void TestExecuteWithException()
        {
            // Arrange
            var fakeServiceProvider = A.Fake<IServiceProvider>();
            var fakeWebClient = A.Fake<IWebClient>();

            // Act
            var plugin = new dkdt_asyncrequestresponsesample_create("", _constSecureConfig);

            Assert.ThrowsException<InvalidPluginExecutionException>(() =>
                plugin.TestableExecute(fakeServiceProvider, fakeWebClient)
            );
        }

        [TestMethod]
        public void TestExecuteWithSecureConfiguration()
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
            var plugin = new dkdt_asyncrequestresponsesample_create("", _constSecureConfig);
            plugin.TestableExecute(fakeServiceProvider, fakeWebClient);

            //Assert
            A.CallTo(
                () => fakeWebClient.UploadData(A<string>.Ignored, A<string>.Ignored, A<byte[]>.Ignored)
            ).MustHaveHappened();
        }

        [TestMethod]
        public void TestExecuteWithoutSecureConfiguration()
        {
            // Arrange
            var fakeServiceProvider = A.Fake<IServiceProvider>();
            var fakeWebClient = A.Fake<IWebClient>();

            // Act
            var plugin = new dkdt_asyncrequestresponsesample_create(
                "",
                ""
            );

            Assert.ThrowsException<InvalidPluginExecutionException>(()=>
                plugin.TestableExecute(fakeServiceProvider, fakeWebClient),
                "No Secure Configuration"
            );
        }
    }
}
