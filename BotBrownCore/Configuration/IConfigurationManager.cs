using System.Collections.Generic;

namespace BotBrown.Configuration
{
    public interface IConfigurationManager
    {
        void ResetCacheFor(string filename);

        T LoadConfiguration<T>(string filename)
            where T : IConfiguration;

        void WriteConfiguration(IConfiguration configurationValue, string filename);
        
        IEnumerable<ConfigurationStatus> CheckConfigurationStatus();
    }
}