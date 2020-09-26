namespace BotBrown
{
    public class BotArguments
    {
        public BotArguments(bool isDebug, bool dontConnectToTwitch, string port)
        {
            IsDebug = isDebug;
            DontConnectToTwitch = dontConnectToTwitch;
            Port = port;
        }

        public bool IsDebug { get; }

        public bool DontConnectToTwitch { get; }

        public string Port { get; }
    }
}
