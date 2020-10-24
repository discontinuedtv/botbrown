namespace BotBrown.Configuration
{
    using BotBrown.Configuration.Factories;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.IO;
    using Serilog;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    public sealed class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfigurationPathProvider configurationPathProvider;
        private readonly IConfigurationFileFactoryRegistry registry;
        private readonly ILogger logger;
        private readonly ConcurrentDictionary<Type, IConfiguration> configurations;

        private string ConfigurationBasePath => configurationPathProvider.Path;

        public ConfigurationManager(
            IConfigurationFileFactoryRegistry registry,
            ILogger logger,
            IConfigurationPathProvider configurationPathProvider)
        {
            configurations = new ConcurrentDictionary<Type, IConfiguration>();
            this.registry = registry;
            this.logger = logger.ForContext<ConfigurationManager>();
            this.configurationPathProvider = configurationPathProvider;
        }

        public void ResetCacheFor(string filename)
        {
            filename = Path.GetFileName(filename);
            Type typeToRemoveFromCache = null;
            foreach (var configValuePair in configurations)
            {
                var attributes = Attribute.GetCustomAttributes(configValuePair.Value.GetType());
                var attribute = (ConfigurationFileAttribute)attributes.First(x => x.GetType() == typeof(ConfigurationFileAttribute));
                if (attribute.Filename == filename)
                {
                    typeToRemoveFromCache = configValuePair.Key;
                    break;
                }
            }

            if (typeToRemoveFromCache != null)
            {
                logger.Information($"Configuration file '{filename}' changed. Clearing cache.");
                configurations.TryRemove(typeToRemoveFromCache, out var _);
            }
        }

        public T LoadConfiguration<T>()
            where T : IConfiguration
        {
            Type key = typeof(T);
            if (configurations.ContainsKey(key))
            {
                return (T)configurations[key];
            }

            var filename = key.GetCustomAttribute<ConfigurationFileAttribute>().Filename;
            string pathToFile = $"{ConfigurationBasePath}/{filename}";

            if (!File.Exists(pathToFile))
            {
                IConfigurationFileFactory<T> factory = registry.GetFactory<T>();
                T defaultConfiguration = factory.CreateDefaultConfiguration();
                WriteConfiguration(defaultConfiguration);

                configurations.TryAdd(key, defaultConfiguration);

                if (defaultConfiguration is IChangeableConfiguration changeableConfiguration)
                    changeableConfiguration.PropertyChanged += ConfigurationChanged;

                return defaultConfiguration;
            }

            using (TextReader reader = new StreamReader($"{ConfigurationBasePath}/{filename}"))
            {
                string serialzedConfiguration = reader.ReadToEnd();
                var configuration = JsonConvert.DeserializeObject<T>(serialzedConfiguration);
                configurations.TryAdd(key, configuration);

                if (configuration is IChangeableConfiguration changeableConfiguration)
                    changeableConfiguration.PropertyChanged += ConfigurationChanged;

                return configuration;
            }
        }

        private void ConfigurationChanged(object sender, PropertyChangedEventArgs e)
        {
            WriteConfiguration(sender as IConfiguration);
        }

        public void WriteConfiguration(IConfiguration configurationValue)
        {
            var filename = configurationValue.GetType().GetCustomAttribute<ConfigurationFileAttribute>().Filename;

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            using StreamWriter sw = new StreamWriter($"{ConfigurationBasePath}/{filename}");
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, configurationValue);
        }

        public IEnumerable<IConfiguration> CheckConfigurationStatus()
        {
            ResolveConfigurationTypes();

            var status = new List<IConfiguration>();

            foreach (KeyValuePair<Type, IConfiguration> configuration in configurations)
            {
                status.Add(configuration.Value);
            }

            return status;
        }

        private void ResolveConfigurationTypes()
        {
            Type[] types = GetType().Assembly.GetTypes().Where(x => x.GetCustomAttribute<ConfigurationFileAttribute>() != null).ToArray();
            foreach (Type type in types)
            {
                if (!configurations.TryGetValue(type, out IConfiguration _))
                {
                    MethodInfo method = GetType().GetMethod(nameof(LoadConfiguration));
                    MethodInfo genericMethod = method.MakeGenericMethod(GetType());
                    genericMethod.Invoke(null, null);
                }
            }
        }
    }
}