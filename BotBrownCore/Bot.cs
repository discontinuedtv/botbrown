namespace BotBrown
{
    using System;
    using System.Threading;
    using BotBrown.DI;
    using BotBrown.Workers;

    public sealed class Bot : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private IWorkerHost workerHost;

        public void Execute(BotArguments botArguments, IBotContainer botContainer)
        {
            workerHost = botContainer.Resolve<IWorkerHost>();
            workerHost.Execute(cancellationTokenSource.Token, botArguments);
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
