namespace BotBrown.Configuration
{
    using System.IO;

    public class DefaultConfigurationPathProvider : IConfigurationPathProvider
    {
        public string Path
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }
    }
}
