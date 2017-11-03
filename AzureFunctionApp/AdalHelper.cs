using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Globalization;
using System.Threading.Tasks;

namespace AzureFunctionApp
{
    public class AdalHelper
    {
        public virtual async Task<string> GetBearerTokenAsync()
        {
            var settings = new Settings();
            var aadInstance = settings.Get("ida:AADInstance"); ;
            var tenant = settings.Get("ida:Tenant");
            var clientId = settings.Get("ida:ClientId");
            var authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            var crmInstanceUrl = settings.Get("crmInstanceUrl");
            // NOTE: If using Dynamics 365 (online) December 2016 update or later, consider using S2S instead of username/password:
            // https://msdn.microsoft.com/en-us/library/mt790168.aspx
            // If not, consider storing retrieving sensitive data using Azure KeyVault
            var userCredential = new UserPasswordCredential(
                settings.Get("user"),
                settings.Get("password")
            );
            var authContext = new AuthenticationContext(authority, new FileTokenCache(settings));
            var authResult = await authContext.AcquireTokenAsync(crmInstanceUrl, clientId, userCredential);

                return authResult.AccessToken;
        }
    }
}