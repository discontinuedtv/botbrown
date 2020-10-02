using System.Threading.Tasks;
using TwitchLib.Api.V5.Models.Channels;

namespace BotBrown.Events
{
    public class UpdateChannelEvent : Event
    {
        public string Game { get; set; }
        public string Title { get; set; }

        public void Update(Channel channel)
        {
            Game ??= channel.Game;
            Title ??= channel.Status;
        }
    }
}
