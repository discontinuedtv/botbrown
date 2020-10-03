namespace BotBrown.Workers.Webserver.Api
{
    using BotBrown.Configuration;
    using Newtonsoft.Json;

    public class ConfigurationStatusViewModel
    {
        private ConfigurationStatus status;

        public ConfigurationStatusViewModel(ConfigurationStatus status)
        {
            this.status = status;
        }

        [JsonProperty("name")]
        public string Filename => status.Filename;

        [JsonProperty("isValid")]
        public bool IsValid => status.IsValid;
    }
}