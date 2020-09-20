namespace BotBrown.Events.Twitch
{
    using BotBrown.Configuration;
    using TwitchLib.Client.Enums;

    public class ChatCommandReceivedEvent : UserSpecificEvent
    {
        public ChatCommandReceivedEvent(ChannelUser user, string commandText, string commandArgs, string channelName, string optionalUser, UserType userType)
            : base(user)
        {
            CommandText = commandText;
            ChannelName = channelName;
            OptionalUser = optionalUser;
            UserType = userType;
        }

        public string CommandText { get; }

        public string CommandArgs { get; }

        public string ChannelName { get; }

        public string OptionalUser { get; }
        
        public UserType UserType { get; }
    }
}
