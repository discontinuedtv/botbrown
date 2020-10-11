namespace BotBrown
{
    using System;
    using Avalonia;
    using BotBrown.DI;
    using Castle.Windsor;
    using Serilog;

    public class BotBrownLocator : IAvaloniaDependencyResolver
    {
        private IBotContainer container;
        private IAvaloniaDependencyResolver previousResolver;

        public BotBrownLocator(IBotContainer container, IAvaloniaDependencyResolver previousResolver)
        {
            this.container = container;
            this.previousResolver = previousResolver;
        }

        public object GetService(Type serviceType)
        {
            if (container.HasComponent(serviceType))
            {
                return container.Resolve(serviceType);
            }

            var resolved = previousResolver.GetService(serviceType);

            Log.Logger.Error($"{serviceType}");
            
            return resolved;
        }
    }
}