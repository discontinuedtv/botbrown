namespace BotBrown.Configuration
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    public sealed class ConfigurationManager : IConfigurationManager
    {
        private string configurationBasePath = Environment.CurrentDirectory;
        private IConfigurationFileFactoryRegistry registry;
        private Dictionary<Type, (IConfiguration, string)> configurations;

        public ConfigurationManager(IConfigurationFileFactoryRegistry registry)
        {
            configurations = new Dictionary<Type, (IConfiguration, string)>();
            this.registry = registry;
        }

        public void ReloadConfig(string filename)
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
    }
}