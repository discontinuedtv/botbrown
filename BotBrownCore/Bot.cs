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
        private readonly IWorkerHost workerHost;

        public Bot()
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.Register(Classes.FromThisAssembly().InNamespace("BotBrown", true).WithServiceAllInterfaces());
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(IConfigurationFileFactory<>)).WithService.Base());
            workerHost = container.Resolve<IWorkerHost>();
        }

        public void Execute(bool dontConnectToTwitch)
        {
            workerHost.Execute(cancellationTokenSource.Token, dontConnectToTwitch);
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
