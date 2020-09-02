namespace BotBrown.Configuration
{
    public interface IConfigurationFileFactoryRegistry
    {
        void AddFactory<T>(IConfigurationFileFactory<T> factory)
            where T : IConfiguration;

        IConfigurationFileFactory<T> GetFactory<T>()
            where T : IConfiguration;
    }
}