using Newtonsoft.Json;

namespace BotBrown.Configuration
{
    public class TwitchConfiguration : IConfiguration
    {
        [JsonIgnore]
        public const string ApiClientId = "bv8ex3iuo52pc1ob3ti9lq698t45ey";

        [JsonIgnore]
        public const string ClientSecret = "c3ab8aa609ea11e793ae92361f002671";

        public string Username { get; set; }

        //public string ApiAccessToken { get; set; }

        //public string ApiRefreshToken { get; set; }

        public string AccessToken { get; set; }

        public string Channel { get; set; }

        public string TextToSpeechRewardId { get; set; }

        public string BroadcasterUserId { get; set; }

        internal bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(AccessToken) &&
                !string.IsNullOrWhiteSpace(Channel) &&
                !string.IsNullOrWhiteSpace(BroadcasterUserId);
        }
    }
}