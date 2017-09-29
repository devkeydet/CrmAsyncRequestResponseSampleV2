using System.IO;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureFunctionApp
{
    internal class FileTokenCache : TokenCache
    {
        private static readonly object _fileLock = new object();
        private readonly string _cacheFilePath;

        // Initializes the cache against a local file.
        // If the file is already present, it loads its content in the ADAL cache
        public FileTokenCache()
        {
            _cacheFilePath = Settings.Get("HOME") + @"\TokenCache.dat";
            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;
            lock (_fileLock)
            {
                Deserialize(File.Exists(_cacheFilePath)
                    ? File.ReadAllBytes(_cacheFilePath)
                    : null);
            }
        }

        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            File.Delete(_cacheFilePath);
        }

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (_fileLock)
            {
                Deserialize(File.Exists(_cacheFilePath)
                    ? File.ReadAllBytes(_cacheFilePath)
                    : null);
            }
        }

        // Triggered right after ADAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (!HasStateChanged) return;
            lock (_fileLock)
            {
                // reflect changes in the persistent store
                File.WriteAllBytes(_cacheFilePath,
                    Serialize());
                // once the write operation took place, restore the HasStateChanged bit to false
                HasStateChanged = false;
            }
        }
    }
}