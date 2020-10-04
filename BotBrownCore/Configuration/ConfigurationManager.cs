namespace BotBrown.Configuration
{
    using BotBrown.Configuration.Factories;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Serilog;
    using System.Collections.Generic;

    public sealed class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfigurationPathProvider configurationPathProvider;
        private readonly IConfigurationFileFactoryRegistry registry;
        private readonly ILogger logger;
        private readonly ConcurrentDictionary<Type, (IConfiguration, string)> configurations;

        private string ConfigurationBasePath => configurationPathProvider.Path;

        public ConfigurationManager(IConfigurationFileFactoryRegistry registry, ILogger logger, IConfigurationPathProvider configurationPathProvider)
        {
            configurations = new ConcurrentDictionary<Type, (IConfiguration, string)>();
            this.registry = registry;
            this.logger = logger.ForContext<ConfigurationManager>();
            this.configurationPathProvider = configurationPathProvider;
        }

        public void ResetCacheFor(string filename)
        {
            filename = Path.GetFileName(filename);
            Type typeToRemoveFromCache = null;
            foreach (var config in configurations)
            {
                if (config.Value.Item2 == filename)
                {
                    typeToRemoveFromCache = config.Key;
                    break;
                }
            }

            if (typeToRemoveFromCache != null)
            {
                logger.Information($"Configuration file '{filename}' changed. Clearing cache.");
                configurations.TryRemove(typeToRemoveFromCache, out var _);
            }
        }

        public T LoadConfiguration<T>(string filename)
            where T : IConfiguration
        {
            if (configurations.ContainsKey(typeof(T)))
            {
                return (T)configurations[typeof(T)].Item1;
            }

            string pathToFile = $"{ConfigurationBasePath}/{filename}";

            if (!File.Exists(pathToFile))
            {
                IConfigurationFileFactory<T> factory = registry.GetFactory<T>();
                T defaultConfiguration = factory.CreateDefaultConfiguration();
                WriteConfiguration(defaultConfiguration, filename);

                configurations.TryAdd(typeof(T), (defaultConfiguration, filename));

                if(defaultConfiguration is IChangeableConfiguration changeableConfiguration)
                    changeableConfiguration.PropertyChanged += ConfigurationChanged;

                return defaultConfiguration;
            }

            using (TextReader reader = new StreamReader($"{ConfigurationBasePath}/{filename}"))
            {
                string serialzedConfiguration = reader.ReadToEnd();
                var configuration = JsonConvert.DeserializeObject<T>(serialzedConfiguration);
                configurations.TryAdd(typeof(T), (configuration, filename));

                if (configuration is IChangeableConfiguration changeableConfiguration)
                    changeableConfiguration.PropertyChanged += ConfigurationChanged;

                return configuration;
            }
        }

        private void ConfigurationChanged(object sender, PropertyChangedEventArgs e)
        {
            var entry = configurations[sender.GetType()].Item2;
            WriteConfiguration(sender as IConfiguration, entry);
        }

        public void WriteConfiguration(IConfiguration configurationValue, string filename)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            using (StreamWriter sw = new StreamWriter($"{ConfigurationBasePath}/{filename}"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, configurationValue);
            }
        }

        public IEnumerable<IConfiguration> CheckConfigurationStatus()
        {
            ResolveConfigurationTypes();

            var status = new List<IConfiguration>();

            foreach (KeyValuePair<Type, (IConfiguration, string)> configuration in configurations)
            {
                status.Add(configuration.Value.Item1);
            }

            return status;
        }

        private void ResolveConfigurationTypes()
        {
            foreach (Type type in GetType().Assembly.GetTypes())
            {
                var asd = type.GetCustomAttributes(typeof(ConfigurationFileAttribute), true).Cast<ConfigurationFileAttribute>().ToArray();
                if (asd.Length > 0)
                {
                    if (!configurations.TryGetValue(type, out (IConfiguration, string) _))
                    {
                        MethodInfo method = GetType().GetMethod(nameof(LoadConfiguration));

                        MethodInfo genericMethod = method.MakeGenericMethod(type);
                        genericMethod.Invoke(this, new object[] { asd[0].Filename });
                    }
                }
            }
        }
    }
}