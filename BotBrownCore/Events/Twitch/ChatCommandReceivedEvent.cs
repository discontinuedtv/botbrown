namespace BotBrown.Events.Twitch
{
    using BotBrown.Configuration;

    public class ChatCommandReceivedEvent : UserSpecificEvent
    {
        public ChatCommandReceivedEvent(ChannelUser user, string commandText, string channelName)
            : base(user)
        {
            CommandText = commandText;
            ChannelName = channelName;
        }

        public string CommandText { get; }

        public string ChannelName { get; }
    }
}
