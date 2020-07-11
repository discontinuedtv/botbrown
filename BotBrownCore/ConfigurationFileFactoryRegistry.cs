namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;

    internal class ConfigurationFileFactoryRegistry : IConfigurationFileFactoryRegistry
    {
        private readonly IDictionary<Type, object> factories = new Dictionary<Type, object>();

        public void AddFactory<T>(IConfigurationFileFactory<T> factory) where T : IConfiguration
        {
            if (factories.TryGetValue(typeof(T), out object tmpFactory))
            {
                throw new InvalidOperationException($"Es gibt bereits eine Factory für den Typ {typeof(T)}");
            }

            factories.Add(typeof(T), factory);
        }

        public IConfigurationFileFactory<T> GetFactory<T>() where T : IConfiguration
        {
            if (!factories.TryGetValue(typeof(T), out object factory))
            {
                throw new InvalidOperationException($"Es gibt keine Factory für den Typ {typeof(T)}");
            }

            return (IConfigurationFileFactory<T>)factory;
        }
    }
}