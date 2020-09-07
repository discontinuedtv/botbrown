namespace BotBrown
{
    public class BotArguments
    {
        public BotArguments(bool isDebug, bool dontConnectToTwitch)
        {
            IsDebug = isDebug;
            DontConnectToTwitch = dontConnectToTwitch;
        }

        public bool IsDebug { get; }

        public bool DontConnectToTwitch { get; }
    }
}
