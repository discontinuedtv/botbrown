namespace BotBrown.Configuration
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog;

    class ConfigurationWatcher : IConfigurationWatcher
    {
        private const int ConfigurationWatchWaitTime = 5000;
        private readonly IConfigurationManager configurationManager;
        private readonly IConfigurationPathProvider configurationPathProvider;
        private readonly ILogger logger;

        public ConfigurationWatcher(IConfigurationManager configurationManager, IConfigurationPathProvider configurationPathProvider, ILogger logger)
        {
            this.configurationManager = configurationManager;
            this.configurationPathProvider = configurationPathProvider;
            this.logger = logger.ForContext<ConfigurationWatcher>();
        }

        public async Task<bool> StartWatch(CancellationToken cancellationToken)
        {
            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = configurationPathProvider.Path;
                watcher.Filter = "*.json";
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.IncludeSubdirectories = true;
                watcher.Changed += HandleFileChanged;
                watcher.EnableRaisingEvents = true;

                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(ConfigurationWatchWaitTime);
                }
            }

            return true;
        }

        private void HandleFileChanged(object sender, FileSystemEventArgs fileSystemEventArguments)
        {
            try
            {
                configurationManager.ResetCacheFor(fileSystemEventArguments.FullPath);
            }
            catch (Exception e)
            {
                logger.Error("Bei der Vearbeitung einer Konfigurationsänderung ist ein Fehler aufgetreten: {e}", e);
            }
        }
    }
}
