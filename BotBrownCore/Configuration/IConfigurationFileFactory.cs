namespace BotBrownCore.Configuration
{
    public interface IConfigurationFileFactory<T>
        where T : IConfiguration
    {
        T CreateDefaultConfiguration();
    }
}
