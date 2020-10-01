namespace BotBrown
{
    using System;
    using System.Threading;
    using BotBrown.Configuration;
    using BotBrown.Workers;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;

    public sealed class Bot : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly WindsorContainer container = new WindsorContainer();
        private IWorkerHost workerHost;

        public void Execute(BotArguments botArguments)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.Register(Classes.FromThisAssembly().InNamespace("BotBrown", true).WithServiceAllInterfaces());
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(IConfigurationFileFactory<>)).WithService.Base());

            RegisterConfigurationPathProvider(botArguments);
            RegisterSoundPathProvider(botArguments);

            workerHost = container.Resolve<IWorkerHost>();

            workerHost.Execute(cancellationTokenSource.Token, botArguments);
        }

        private void RegisterConfigurationPathProvider(BotArguments botArguments)
        {
            if (botArguments.HasCustomConfigurationPath)
            {
                CustomConfigurationPathProvider custom = new CustomConfigurationPathProvider(botArguments.CustomConfigurationPath);
                container.Register(Component.For<IConfigurationPathProvider>().Instance(custom).Named("_CustomConfiguration").IsDefault());
            }
            else
            {
                container.Register(Component.For<IConfigurationPathProvider>().ImplementedBy<DefaultConfigurationPathProvider>().Named("_CustomConfiguration").IsDefault());
            }
        }

        private void RegisterSoundPathProvider(BotArguments botArguments)
        {
            if (botArguments.HasCustomSoundsPath)
            {
                CustomSoundPathProvider custom = new CustomSoundPathProvider(botArguments.CustomSoundsPath);
                container.Register(Component.For<ISoundPathProvider>().Instance(custom).Named("_CustomSound").IsDefault());
            }
            else
            {
                container.Register(Component.For<ISoundPathProvider>().ImplementedBy<DefaultSoundPathProvider>().Named("_CustomSound").IsDefault());
            }
        }

        public void PublishTestTTSMessage(string message)
        {
            workerHost.PublishTTSMessage(message);
        }

        public void PublishSoundCommand(string message)
        {
            workerHost.PublishSoundCommand(message);
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
