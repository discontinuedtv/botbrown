namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    class ConfigurationWatcher
    {
        private const int ConfigurationWatchWaitTime = 5000;
        private IEventBus bus;
        private IConfigurationManager configurationManager;
        private ILogger logger;
        private Dictionary<string, DateTime> configFiles = new Dictionary<string, DateTime>();

        public ConfigurationWatcher(IEventBus bus, IConfigurationManager configurationManager, ILogger logger)
        {
            this.bus = bus;
            this.configurationManager = configurationManager;
            this.logger = logger;
        }

        public Task<bool> StartWatch(CancellationToken cancellationToken)
        {
            foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                if (!file.EndsWith(".json"))
                {
                    continue;
                }

                configFiles.Add(file, File.GetLastWriteTime(file));
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
                {
                    if (!file.EndsWith(".json"))
                    {
                        continue;
                    }

                    var lastWriteTime = File.GetLastWriteTime(file);
                    if (configFiles.ContainsKey(file) && configFiles[file] < lastWriteTime)
                    {
                        logger.Log("Removing " + file + " from cache. Reload configuration file.");
                        configFiles[file] = lastWriteTime;
                        configurationManager.ReloadConfig(file);
                    }
                }

                Thread.Sleep(ConfigurationWatchWaitTime);
            }

            return new Task<bool>(() => true);
        }
    }
}
