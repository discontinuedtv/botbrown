using BotBrown.Configuration.Factories;

namespace BotBrown.Configuration
{
    public class DeathCounterConfigurationFactory : IConfigurationFileFactory<DeathCounterConfiguration>
    {
        public DeathCounterConfiguration CreateDefaultConfiguration()
        {
            return new DeathCounterConfiguration();
        }
    }
}
