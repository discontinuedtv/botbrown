namespace BotBrown.ChatCommands
{
    using System.Collections.Generic;

    public class ChatCommandResolver : IChatCommandResolver
    {
        private readonly IList<IChatCommand> chatCommands;

        public ChatCommandResolver(IList<IChatCommand> chatCommands)
        {
            this.chatCommands = chatCommands;
        }

        public IEnumerable<IChatCommand> ResolveAllChatCommands()
        {
            return chatCommands;
        }
    }
}
