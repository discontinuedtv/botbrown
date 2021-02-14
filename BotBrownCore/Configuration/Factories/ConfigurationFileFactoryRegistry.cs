namespace BotBrown.Configuration.Factories
{
    using Castle.MicroKernel;
    using System;
    using System.Collections.Concurrent;

    public class ConfigurationFileFactoryRegistry : IConfigurationFileFactoryRegistry
    {
        private readonly IKernel kernel;

        private readonly ConcurrentDictionary<Type, object> factories = new ConcurrentDictionary<Type, object>();

        public ConfigurationFileFactoryRegistry(IKernel kernel)
        {
            this.kernel = kernel;
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

                factories.TryAdd(typeof(T), factory);
            }

            return (IConfigurationFileFactory<T>)factory;
        }
    }
}