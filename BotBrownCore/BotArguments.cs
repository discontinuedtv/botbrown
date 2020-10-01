namespace BotBrown
{
    public class BotArguments
    {
        public BotArguments(bool isDebug, bool dontConnectToTwitch, string customConfigurationPath, string customSoundsPath)
        {
            IsDebug = isDebug;
            DontConnectToTwitch = dontConnectToTwitch;
            CustomConfigurationPath = customConfigurationPath;
            CustomSoundsPath = customSoundsPath;
        }

        public bool IsDebug { get; }

        public bool DontConnectToTwitch { get; }

        public bool HasCustomConfigurationPath => !string.IsNullOrEmpty(CustomConfigurationPath);

        public string CustomConfigurationPath { get; }

        public bool HasCustomSoundsPath => !string.IsNullOrEmpty(CustomSoundsPath);

        public string CustomSoundsPath { get; }
    }
}
