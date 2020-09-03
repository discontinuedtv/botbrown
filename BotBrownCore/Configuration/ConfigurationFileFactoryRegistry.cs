namespace BotBrown.Configuration
{
    using Castle.MicroKernel;
    using System;
    using System.Collections.Generic;

    public class ConfigurationFileFactoryRegistry : IConfigurationFileFactoryRegistry
    {
        private readonly IKernel kernel;

        private readonly IDictionary<Type, object> factories = new Dictionary<Type, object>();

        public ConfigurationFileFactoryRegistry(IKernel kernel)
        {
            this.kernel = kernel;
        }

        // TODO Cleanup
        public void AddFactory<T>(IConfigurationFileFactory<T> factory) where T : IConfiguration
        {
            if (factories.TryGetValue(typeof(T), out _))
            {
                throw new InvalidOperationException($"Es gibt bereits eine Factory für den Typ {typeof(T)}");
            }

            factories.Add(typeof(T), factory);
        }

        public IConfigurationFileFactory<T> GetFactory<T>() where T : IConfiguration
        {
            if (!factories.TryGetValue(typeof(T), out object factory))
            {
                factory = kernel.Resolve<IConfigurationFileFactory<T>>();
                if (factory == null)
                {
                    throw new InvalidOperationException($"Es gibt keine Factory für den Typ {typeof(T)}");
                }

                factories.Add(typeof(T), factory);
            }

            return (IConfigurationFileFactory<T>)factory;
        }
    }
}