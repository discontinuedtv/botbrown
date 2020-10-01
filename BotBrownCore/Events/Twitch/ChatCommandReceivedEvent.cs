namespace BotBrown.Events.Twitch
{
    using BotBrown.Configuration;
    using BotBrown.Models;

    public class ChatCommandReceivedEvent : UserSpecificEvent
    {
        private readonly UserType userType;

        public ChatCommandReceivedEvent(ChannelUser user, string commandText, string commandArgs, string channelName, string optionalUser, UserType userType)
            : base(user)
        {
            CommandText = commandText;
            ChannelName = channelName;
            OptionalUser = optionalUser;
            this.userType = userType;
            CommandArgs = commandArgs;
        }

        public string CommandText { get; }

        public string CommandArgs { get; }

        public string ChannelName { get; }

        public string OptionalUser { get; }
        
        public bool IsForUserType(UserType typeToCheck)
        {
            return userType.IsType(typeToCheck);
        }
    }
}
