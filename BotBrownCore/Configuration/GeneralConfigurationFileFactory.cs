namespace BotBrownCore.Configuration
{
    internal class GeneralConfigurationFileFactory : IConfigurationFileFactory<GeneralConfiguration>
    {
        public GeneralConfiguration CreateDefaultConfiguration()
        {
            return new GeneralConfiguration
            {
                ActivateTextToSpeech = false
            };
        }
    }
}