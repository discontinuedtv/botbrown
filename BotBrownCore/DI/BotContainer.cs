namespace BotBrown.DI
{
    using BotBrown.ChatCommands;
    using BotBrown.Configuration;
    using BotBrown.Workers;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using Serilog;
    using System;
    using System.IO;

    public interface IBotContainer
    {
        T Resolve<T>();

        object Resolve(Type serviceType);

        bool HasComponent(Type serviceType);
    }

    public class BotContainer : IBotContainer, IDisposable
    {
        private readonly WindsorContainer container = new WindsorContainer();

        public BotContainer(BotArguments botArguments)
        {
            try
            {
                container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
                container.Register(Classes.FromThisAssembly().InNamespace("BotBrown", true)
                    .Unless(type => type == typeof(LoggingInterceptor))
                    .WithServiceAllInterfaces()
                    .Configure(x => x.Interceptors<LoggingInterceptor>()));
                container.Register(Component.For<LoggingInterceptor>().ImplementedBy<LoggingInterceptor>().IsDefault());

                container.Register(Classes.FromThisAssembly().BasedOn(typeof(IConfigurationFileFactory<>)).WithService.Base().Configure(x => x.Interceptors<LoggingInterceptor>()));
                container.Register(Classes.FromThisAssembly().BasedOn(typeof(IChatCommand)).WithService.Base().Configure(x => x.Interceptors<LoggingInterceptor>()));
                RegisterConfigurationPathProvider(botArguments);
                RegisterSoundPathProvider(botArguments);
                RegisterLogger(botArguments);
            }
            catch (Exception)
            {
                throw new Exception("bla");
            }
        }

        public void Install(IWindsorInstaller installer)
        {
            container.Install(installer);
        }

        public void Dispose()
        {
            container.Dispose();
        }

        private void RegisterLogger(BotArguments botArguments)
        {
            var configuration = new LoggerConfiguration().MinimumLevel.Verbose()
                 .WriteTo.RollingFile(
                     Path.Combine(botArguments.LogPath, "botbrown-{Date}.log"),
                     botArguments.IsDebug ? Serilog.Events.LogEventLevel.Debug : Serilog.Events.LogEventLevel.Verbose,
                     "({Timestamp:HH:mm:ss.fff}|{Level:u3}|{SourceContext}) {Message}{NewLine}{Exception}"
                 ).WriteTo.Console(
                     botArguments.IsDebug ? Serilog.Events.LogEventLevel.Debug : Serilog.Events.LogEventLevel.Verbose,
                     "({Timestamp:HH:mm:ss.fff}|{Level:u3}|{SourceContext}) {Message}{NewLine}{Exception}");

            Log.Logger = configuration.CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

            container.Register(Component.For<ILogger>().LifestyleSingleton().Instance(Log.Logger));
        }

        private void RegisterConfigurationPathProvider(BotArguments botArguments)
        {
            if (botArguments.HasCustomConfigurationPath)
            {
                CustomConfigurationPathProvider custom = new CustomConfigurationPathProvider(botArguments.CustomConfigurationPath);
                container.Register(Component.For<IConfigurationPathProvider>().Instance(custom).Named("_CustomConfiguration").IsDefault().Interceptors<LoggingInterceptor>());
            }
            else
            {
                container.Register(Component.For<IConfigurationPathProvider>().ImplementedBy<DefaultConfigurationPathProvider>().Named("_CustomConfiguration").IsDefault().Interceptors<LoggingInterceptor>());
            }
        }

        private void RegisterSoundPathProvider(BotArguments botArguments)
        {
            if (botArguments.HasCustomSoundsPath)
            {
                CustomSoundPathProvider custom = new CustomSoundPathProvider(botArguments.CustomSoundsPath);
                container.Register(Component.For<ISoundPathProvider>().Instance(custom).Named("_CustomSound").IsDefault().Interceptors<LoggingInterceptor>());
            }
            else
            {
                container.Register(Component.For<ISoundPathProvider>().ImplementedBy<DefaultSoundPathProvider>().Named("_CustomSound").IsDefault().Interceptors<LoggingInterceptor>());
            }
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public object Resolve(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        public bool HasComponent(Type serviceType)
        {
            return container.Kernel.HasComponent(serviceType);
        }
    }
}
