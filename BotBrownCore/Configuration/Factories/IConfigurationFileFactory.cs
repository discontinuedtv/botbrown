namespace BotBrown.Configuration.Factories
{
    public interface IConfigurationFileFactory<T>
        where T : IConfiguration
    {
        T CreateDefaultConfiguration();
    }
}
