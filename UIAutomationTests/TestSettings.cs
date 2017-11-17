using Microsoft.Dynamics365.UIAutomation.Browser;

namespace UIAutomationTests
{
    public static class TestSettings
    {
        public static BrowserOptions Options = new BrowserOptions
        {
#if DEBUG
            BrowserType = BrowserType.Chrome,
#endif
#if RELEASE
            BrowserType = BrowserType.PhantomJs,
#endif
            PrivateMode = true,
            FireEvents = true
        };
    }
}