namespace BotBrown.Events.Twitch
{
    using BotBrown.Configuration;

    public class ChatCommandReceivedEvent : UserSpecificEvent
    {
        public ChatCommandReceivedEvent(ChannelUser user, string commandText, string channelName, string optionalUser)
            : base(user)
        {
            CommandText = commandText;
            ChannelName = channelName;
            OptionalUser = optionalUser;
        }

        public string CommandText { get; }

        public string ChannelName { get; }

        public string OptionalUser { get; }
    }
}
