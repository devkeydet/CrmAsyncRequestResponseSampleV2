using System;

namespace AzureFunctionApp
{
    public class Settings
    {
        public virtual string Get(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
