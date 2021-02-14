namespace BotBrown.Configuration
{
    public class CustomConfigurationPathProvider : IConfigurationPathProvider
    {      
        public CustomConfigurationPathProvider(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
