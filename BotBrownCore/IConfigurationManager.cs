namespace BotBrownCore
{
    internal interface IConfigurationManager
    {
        T LoadConfiguration<T>(string filename)
            where T : IConfiguration;

        void WriteConfiguration<T>(T configurationValue, string filename)
            where T : IConfiguration;
    }
}