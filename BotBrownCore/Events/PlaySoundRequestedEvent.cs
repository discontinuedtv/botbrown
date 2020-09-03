namespace BotBrown.Events
{
    public class PlaySoundRequestedEvent : Event
    {
        public PlaySoundRequestedEvent(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }
    }
}
