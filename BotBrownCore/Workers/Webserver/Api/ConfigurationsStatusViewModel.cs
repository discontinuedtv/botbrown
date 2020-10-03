namespace BotBrown.Workers.Webserver.Api
{
    using Newtonsoft.Json;

    public class ConfigurationsStatusViewModel
    {
        [JsonProperty("configurations")]
        public ConfigurationStatusViewModel[] Configurations { get; set; }
    }
}
