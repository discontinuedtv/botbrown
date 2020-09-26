namespace BotBrown.Configuration
{
    using BotBrown.Configuration.Factories;

    public class FactConfigurationFactory : IConfigurationFileFactory<FactConfiguration>
    {
        public FactConfiguration CreateDefaultConfiguration()
        {
            return new FactConfiguration();
        }
    }
}
