namespace BotBrown.Configuration
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    class ConfigurationWatcher : IConfigurationWatcher
    {
        private const int ConfigurationWatchWaitTime = 5000;
        private IConfigurationManager configurationManager;

        public ConfigurationWatcher(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public Task<bool> StartWatch(CancellationToken cancellationToken)
        {
            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = Directory.GetCurrentDirectory();
                watcher.Filter = "*.json";
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.IncludeSubdirectories = true;
                watcher.Changed += HandleFileChanged;
                watcher.EnableRaisingEvents = true;

                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(ConfigurationWatchWaitTime);
                }
            }

            return new Task<bool>(() => true);
        }

        private void HandleFileChanged(object sender, FileSystemEventArgs e)
        {
            configurationManager.ResetCacheFor(e.FullPath);
        }
    }
}
