namespace BotBrown.Configuration.Factories
{
    public class AzureConfigurationFactory : IConfigurationFileFactory<AzureConfiguration>
    {
        public AzureConfiguration CreateDefaultConfiguration()
        {
            return new AzureConfiguration();
        }
    }
}
