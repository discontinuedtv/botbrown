namespace BotBrown.Configuration
{
    using BotBrown.Configuration.Factories;

    public class DeathCounterConfigurationFactory : IConfigurationFileFactory<DeathCounterConfiguration>
    {
        public DeathCounterConfiguration CreateDefaultConfiguration()
        {
            return new DeathCounterConfiguration();
        }
    }
}
