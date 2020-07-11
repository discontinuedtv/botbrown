namespace BotBrownCore
{
    public interface IConfigurationFileFactory<T>
        where T : IConfiguration
    {
        T CreateDefaultConfiguration();
    }
}
