﻿namespace BotBrown.Configuration
{
    using Newtonsoft.Json;
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.Twitch)]
    public class TwitchConfiguration : IConfiguration
    {
        public string Username { get; set; }

        public string ApiClientId { get; set; }

        public string ApiAccessToken { get; set; }

        public string ApiRefreshToken { get; set; }

        public string AccessToken { get; set; }

        public string Channel { get; set; }

        public string TextToSpeechRewardId { get; set; }

        public string BroadcasterUserId { get; set; }

        [JsonIgnore]
        public string Filename => ConfigurationFileConstants.Twitch;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(AccessToken) &&
                !string.IsNullOrWhiteSpace(Channel) &&
                !string.IsNullOrWhiteSpace(BroadcasterUserId);
        }
    }
}