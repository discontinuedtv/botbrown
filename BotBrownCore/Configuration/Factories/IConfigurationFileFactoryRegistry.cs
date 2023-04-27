namespace BotBrown.Configuration.Factories
{
    public interface IConfigurationFileFactoryRegistry
    {
        IConfigurationFileFactory<T> GetFactory<T>()
            where T : IConfiguration;
    }
}