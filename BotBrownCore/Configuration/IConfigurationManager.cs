namespace BotBrown.Configuration
{
    public interface IConfigurationManager
    {
        void ReloadConfig(string filename);

        T LoadConfiguration<T>(string filename)
            where T : IConfiguration;

        void WriteConfiguration(IConfiguration configurationValue, string filename);
    }
}