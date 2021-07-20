namespace BotBrown.Events
{
    using TwitchLib.Api.Helix.Models.Channels.GetChannelInformation;

    public class UpdateChannelEvent : Event
    {
        public string Game { get; set; }
        public string Title { get; set; }

        public void Update(ChannelInformation channel)
        {
            Game ??= channel.GameName;
            Title ??= channel.Title;
        }
    }
}
