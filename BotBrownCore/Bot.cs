namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using BotBrownCore.Configuration;
    using System.Threading.Tasks;
    using System.Linq;
    using BotBrownCore.Events;

    public sealed class Bot : IDisposable
    {
        public void Dispose()
        {
            
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

        public void Execute()
        {
        }
    }
}
