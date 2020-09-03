namespace BotBrown.Configuration
{
    public interface IConfigurationManager
    {
        T LoadConfiguration<T>(string filename)
            where T : IConfiguration;

        void WriteConfiguration(IConfiguration configurationValue, string filename);
    }
}