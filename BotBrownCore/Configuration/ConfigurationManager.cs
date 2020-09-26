namespace BotBrown.Configuration
{
    using BotBrown.Configuration.Factories;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public sealed class ConfigurationManager : IConfigurationManager
    {
        private readonly string configurationBasePath = Environment.CurrentDirectory;
        private readonly IConfigurationFileFactoryRegistry registry;
        private readonly ILogger logger;
        private readonly Dictionary<Type, (IConfiguration, string)> configurations;

        public ConfigurationManager(IConfigurationFileFactoryRegistry registry, ILogger logger)
        {
            configurations = new Dictionary<Type, (IConfiguration, string)>();
            this.registry = registry;
            this.logger = logger;
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
                logger.Log($"Configuration file '{filename}' changed. Clearing cache.");
                configurations.Remove(typeToRemoveFromCache);
            }
        }

        public T LoadConfiguration<T>(string filename)
            where T : IConfiguration
        {
            if (configurations.ContainsKey(typeof(T)))
            {
                return (T)configurations[typeof(T)].Item1;
            }

            string pathToFile = $"{configurationBasePath}/{filename}";

            if (!File.Exists(pathToFile))
            {
                IConfigurationFileFactory<T> factory = registry.GetFactory<T>();
                T defaultConfiguration = factory.CreateDefaultConfiguration();
                WriteConfiguration(defaultConfiguration, filename);

                configurations.Add(typeof(T), (defaultConfiguration, filename));

                defaultConfiguration.PropertyChanged += ConfigurationChanged;

                return defaultConfiguration;
            }

            using (TextReader reader = new StreamReader($"{configurationBasePath}/{filename}"))
            {
                string serialzedConfiguration = reader.ReadToEnd();
                var configuration = JsonConvert.DeserializeObject<T>(serialzedConfiguration);
                configurations.Add(typeof(T), (configuration, filename));

                configuration.PropertyChanged += ConfigurationChanged;

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

            using (StreamWriter sw = new StreamWriter($"{configurationBasePath}/{filename}"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, configurationValue);
            }
        }

        public IEnumerable<ConfigurationStatus> CheckConfigurationStatus()
        {
            ResolveConfigurationTypes();

            var status = new List<ConfigurationStatus>();

            foreach (KeyValuePair<Type, (IConfiguration, string)> configuration in configurations)
            {
                status.Add(new ConfigurationStatus(configuration.Value.Item2, configuration.Value.Item1.IsValid()));
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

                        MethodInfo genericMethod = method.MakeGenericMethod(GetType());
                        genericMethod.Invoke(null, new object[] { asd[0].Filename });
                    }
                }
            }
        }
    }
}