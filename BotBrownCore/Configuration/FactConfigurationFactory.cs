namespace BotBrown.Configuration
{
    public class FactConfigurationFactory : IConfigurationFileFactory<FactConfiguration>
    {
        public FactConfiguration CreateDefaultConfiguration()
        {
            return new FactConfiguration();
        }
    }
}
