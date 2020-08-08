namespace BotBrownCore
{
    public interface ICommand
    {
        void Execute(IBotExecutionContext context);
    }
}