using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace AzureFunctionApp
{
    internal static class AdalHelper
    {
        private static readonly string _aadInstance = Settings.Get("ida:AADInstance");
        private static readonly string _tenant = Settings.Get("ida:Tenant");
        private static readonly string _clientId = Settings.Get("ida:ClientId");
        private static readonly string _authority = string.Format(CultureInfo.InvariantCulture, _aadInstance, _tenant);
        private static readonly string _crmInstanceUrl = Settings.Get("crmInstanceUrl");
        // NOTE: If using Dynamics 365 (online) December 2016 update or later, consider using S2S instead of username/password:
        // https://msdn.microsoft.com/en-us/library/mt790168.aspx
        // If not, consider storing retrieving sensitive data using Azure KeyVault
        private static readonly UserPasswordCredential _userCredential = new UserPasswordCredential(
            Settings.Get("user"),
            Settings.Get("password")
        );
        private static AuthenticationContext _authContext = new AuthenticationContext(_authority, new FileTokenCache());

        public async static Task<string> GetBearerTokenAsync()
        {
            var authResult = await _authContext.AcquireTokenAsync(_crmInstanceUrl, _clientId, _userCredential);

            return authResult.AccessToken;
        }
    }
}
