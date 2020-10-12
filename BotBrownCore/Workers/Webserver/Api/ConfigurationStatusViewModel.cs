namespace BotBrown.Workers.Webserver.Api
{
    using BotBrown.Configuration;
    using Newtonsoft.Json;
    using System.Reflection;

    public class ConfigurationStatusViewModel
    {
        private readonly IConfiguration status;

        public ConfigurationStatusViewModel(IConfiguration status)
        {
            this.status = status;
        }

        [JsonProperty("name")]
        public string Filename => status.GetType().GetCustomAttribute<ConfigurationFileAttribute>().Filename;

        [JsonProperty("isValid")]
        public bool IsValid => status.IsValid();

        [JsonProperty("values")]
        public ConfigurationValueViewModel[] Values
        {
            get
            {
                return new[]
                {
                    new ConfigurationValueViewModel("a", "b", "c")
                };
            }
        }
    }

    public class ConfigurationValueViewModel
    {
        public ConfigurationValueViewModel(string defaultValue, string currentValue, string typename)
        {
            DefaultValue = defaultValue;
            CurrentValue = currentValue;
            TypeName = typename;
        }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; }

        [JsonProperty("currentValue")]
        public string CurrentValue { get; }

        [JsonProperty("typename")]
        public string TypeName { get; }
    }
}
