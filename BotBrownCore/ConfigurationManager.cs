namespace BotBrownCore
{
    using Newtonsoft.Json;
    using System;
    using System.IO;

    public sealed class ConfigurationManager : IConfigurationManager
    {
        private string configurationBasePath = Environment.CurrentDirectory;
        private IConfigurationFileFactoryRegistry registry;

        public ConfigurationManager(IConfigurationFileFactoryRegistry registry)
        {
            this.registry = registry;
        }

        public T LoadConfiguration<T>(string filename)
            where T : IConfiguration
        {
            string pathToFile = $"{configurationBasePath}/{filename}";

            if (!File.Exists(pathToFile))
            {
                IConfigurationFileFactory<T> factory = registry.GetFactory<T>();
                T defaultConfiguration = factory.CreateDefaultConfiguration();
                WriteConfiguration<T>(defaultConfiguration, filename);
                return defaultConfiguration;
            }

            using (TextReader reader = new StreamReader($"{configurationBasePath}/{filename}"))
            {
                string serialzedConfiguration = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(serialzedConfiguration);
            }           
        }

        public void WriteConfiguration<T>(T configurationValue, string filename)
            where T : IConfiguration
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