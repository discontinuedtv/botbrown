using Newtonsoft.Json;

namespace BotbrownWPF.Views
{
    public class TwitchApiResponse<T>
    {
        [JsonProperty("data")]
        public T[] Data { get; set; }
    }
}