namespace BotBrown.ChatCommands
{
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class BaseChatCommand : IChatCommand
    {
        private DateTimeOffset lastUsed;

        public bool CanConsume(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            return Commands.Contains(chatCommandReceivedEvent.CommandText) &&
                chatCommandReceivedEvent.IsForUserType(ElligableUserType) &&
                lastUsed.Add(Cooldown) < DateTimeOffset.Now;
        }

        public async Task<bool> Consume(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            lastUsed = DateTimeOffset.Now;
            await ConsumeCommandSpecific(chatCommandReceivedEvent);
            return ShouldContinue;
        }

        public abstract Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent);

        public abstract UserType ElligableUserType { get; }

        public abstract string[] Commands { get; }

        public abstract TimeSpan Cooldown { get; }

        public abstract bool ShouldContinue { get; }
    }
}
