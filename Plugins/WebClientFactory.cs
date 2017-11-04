using System.Diagnostics.CodeAnalysis;

namespace CrmAsyncRequestResponseSample.Plugins
{
    [ExcludeFromCodeCoverage]
    class WebClientFactory
    {
        public static IWebClient Create()
        {
            return new MyWebClient();
        }
    }
}