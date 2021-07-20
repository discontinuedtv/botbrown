namespace BotBrown.Events.Twitch
{
    public class RaidReceivedEvent : Event
    {
        private string displayName;
        private string msgParamViewerCount;

        public RaidReceivedEvent(string displayName, string msgParamViewerCount)
        {
            this.displayName = displayName;
            this.msgParamViewerCount = msgParamViewerCount;
        }
    }
}
