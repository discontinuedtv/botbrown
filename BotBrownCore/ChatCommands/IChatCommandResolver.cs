namespace BotBrown.ChatCommands
{
    using System.Collections.Generic;

    public interface IChatCommandResolver
    {
        IEnumerable<IChatCommand> ResolveAllChatCommands();
    }
}