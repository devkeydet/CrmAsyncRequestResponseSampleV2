namespace CrmAsyncRequestResponseSample.Plugins
{
    class WebClientFactory
    {
        public static IWebClient Create()
        {
            return new MyWebClient();
        }
    }
}