using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//Breaking one of the the golden rules of source control by checking in dlls.  
//However, https://github.com/microsoft/easyrepro assemblies are not on nuget.  
//I've filed an issue: https://github.com/Microsoft/EasyRepro/issues/44.  
//If/when these assemblies are published to nuget, I will pull from nuget and remove from source control.
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using System.Security;
using System.Configuration;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace UIAutomationTests
{
    [TestClass]
    public class CrmAsyncRequestResponseSampleTests
    {

        private readonly SecureString _username = ConfigurationManager.AppSettings["OnlineUsername"].ToSecureString();
        private readonly SecureString _password = ConfigurationManager.AppSettings["OnlinePassword"].ToSecureString();
        private readonly Uri _xrmUri = new Uri(ConfigurationManager.AppSettings["OnlineCrmUrl"].ToString());

        [TestMethod]
        public void TestsCrmAsyncRequestResponseSample()
        {
            using (var xrmBrowser = new XrmBrowser(TestSettings.Options))
            {
                xrmBrowser.LoginPage.Login(_xrmUri, _username, _password);

                // Navigate to the app by selecting the AsyncRequestResponseSample app in the app list
                var id = "TabArrowDivider";
                xrmBrowser.Driver.ClickWhenAvailable(By.Id(id));

                xrmBrowser.ThinkTime(5000);  // not sure why I need think time here, but I get an exception otherwise.
                // Need to see if there is a better way to consistently select the app from the list by app name.  For now using XPath...  
                var xpath = "//*[@id='taskpane-scroll-container']/div[3]/button[1]";
                var appElement = xrmBrowser.Driver.FindElement(By.XPath(xpath));  // Tried ClicWhenAvailable, but it failed sporadically
                appElement.Click();

                // Create a new record and save
                xrmBrowser.ThinkTime(5000);  // not sure why I need think time here, but I get an exception otherwise.
                xrmBrowser.CommandBar.ClickCommand("New");
                var guid = Guid.NewGuid().ToString();
                xrmBrowser.Entity.SetValue("dkdt_name", guid);
                xrmBrowser.CommandBar.ClickCommand("Save");

                // Verify web resource contents after message comes back from Azure
                xrmBrowser.Document.SwitchToContentFrame();
                xrmBrowser.Driver.SwitchTo().Frame("WebResource_CheckForUpdateFromAzureCode");
                var wait = new WebDriverWait(xrmBrowser.Driver, TimeSpan.FromSeconds(35));
                wait.Until(d=>d.FindElement(By.Id("status")).Text.Contains("Azure code updated entity."));
            }
        }
    }
}