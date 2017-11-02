using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrmAsyncRequestResponseSample.Plugins;
using Microsoft.Crm.Sdk.Fakes;
using Microsoft.Xrm.Sdk;
using System.Net.Fakes;
using System.Net;

namespace Plugins.Tests
{
    [TestClass]
    public class dkdt_asyncrequestresponsesample_create_tests
    {
        [TestMethod]
        public void TestExecuteWithConfiguration()
        {
            // Arrange
            using (var pipeline = new PluginPipeline(
                FakeMessageNames.Create,
                FakeStages.PostOperation,
                new Entity("dkdt_asyncrequestresponsesample")))
            {
                ShimWebClient.AllInstances.UploadDataStringStringByteArray = (WebClient wc, string a, string b, byte[] c) =>
                {
                    return new byte[0];
                };

                //Act
                var plugin = new dkdt_asyncrequestresponsesample_create("", "Endpoint=sb://some.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rWrelB5AUL9GfOp0tCCCHeClWngq827UQI/3C5N9ts0=;EntityPath=somequeue");
                pipeline.Execute(plugin);

                // Assert
                // Nothing to asssert in this case since we aren't manipulating anything in the context.  If the code executes without exception, it's a succesful test.
            }
        }
    }
}
