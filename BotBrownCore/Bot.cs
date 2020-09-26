namespace BotBrown
{
    using System;
    using System.IO;
    using System.Threading;
    using BotBrown.ChatCommands;
    using BotBrown.Configuration;
    using BotBrown.DI;
    using BotBrown.Workers;
    using BotBrown.Configuration.Factories;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using Serilog;

    public sealed class Bot : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly WindsorContainer container = new WindsorContainer();
        private IWorkerHost workerHost;

        public void Execute(BotArguments botArguments)
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

            workerHost = container.Resolve<IWorkerHost>();
            workerHost.Execute(cancellationTokenSource.Token, botArguments);
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

        public void PublishTestTTSMessage(string message)
        {
            workerHost.PublishTTSMessage(message);
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            container.Dispose();
        }

        // https://github.com/TwitchLib/TwitchLib
        // https://twitchapps.com/tmi/
        // https://twitchtokengenerator.com/

        // TODO Konfigurationsdateien cachen

        // TODOS fürs nächste mal:
        // Subscriber, Resubscriber vorlesen lassen --> testen
        // Aufräumen des Codes
        // Konfigurationen in Datenbank speichern <- Wirklich? Oder lieber Plugin das reagiert?

        // TODOS fürs übernächste mal:
        // Lautstärke einstellbar -> Recherche
        // Commands extrahieren und erweiterbar machen
        // Lautstärke einstellbar        
        // Oberfläche für die Konfiguration
    }
}
