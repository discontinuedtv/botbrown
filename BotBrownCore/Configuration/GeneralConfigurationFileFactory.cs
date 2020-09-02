using System.Collections.Generic;

namespace BotBrown.Configuration
{
    public class GeneralConfigurationFileFactory : IConfigurationFileFactory<GeneralConfiguration>
    {
        public GeneralConfiguration CreateDefaultConfiguration()
        {
            return new GeneralConfiguration
            {
                ActivateTextToSpeech = false,
                BotChannelGreeting = "Bot Brown ist zurück aus der Zukunft!",
                ByePhrase = "Auf wiedersehen {0}. Es war schön mit dir.",
                ByePhrases = new HashSet<string>
                {
                    "bye",
                    "ciao",
                    "tschüss",
                    "tschö",
                    "auf wiedersehen"
                }
            };
        }
    }
}