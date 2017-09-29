using System;

namespace AzureFunctionApp
{
    public static class Settings
    {
        public static string Get(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
