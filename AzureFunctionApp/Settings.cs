using System;
using System.Diagnostics.CodeAnalysis;

namespace AzureFunctionApp
{
    [ExcludeFromCodeCoverage]
    public class Settings
    {
        public virtual string Get(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
