namespace BotBrown.Configuration
{
    public class SentenceConfigurationFileFactory : IConfigurationFileFactory<SentenceConfiguration>
    {
        public SentenceConfiguration CreateDefaultConfiguration()
        {
            return new SentenceConfiguration
            {
                FollowerAlert = "Willkommen an Bord, {0}!",
                SubscriberAlert = "Vielen Dank für deine Unterstützung, {0}!",
                ResubscriberAlert = "Vielen Dank für deine anhaltende Unterstützung, {0}!. Du bist schon unglaubliche {1} Monate dabei.",
                GiftedSubscriberAlert = "{0} hat ein Geschenkabo erhalten. Schön dich dabei zu haben!",
                SubBombAlert = "{0} haut {1} Abos raus! Danke dir vielmals!"
            };
        }
    }
}