namespace BotBrown.ChatCommands
{
    using BotBrown.Events.Twitch;
    using System.Threading.Tasks;

    public interface IChatCommand
    {
        bool CanConsume(ChatCommandReceivedEvent chatCommandReceivedEvent);

        Task<bool> Consume(ChatCommandReceivedEvent chatCommandReceivedEvent);
    }
}
