﻿namespace BotBrown
{
    public class BotArguments
    {
        public BotArguments(bool isDebug, bool dontConnectToTwitch, string port, string customConfigurationPath, string customSoundsPath, string logPath)
        {
            IsDebug = isDebug;
            DontConnectToTwitch = dontConnectToTwitch;
            Port = port;
            CustomConfigurationPath = customConfigurationPath;
            CustomSoundsPath = customSoundsPath;
            LogPath = logPath;
        }

        public bool IsDebug { get; }

        public bool DontConnectToTwitch { get; }

        public string Port { get; }

        public bool HasCustomConfigurationPath => !string.IsNullOrEmpty(CustomConfigurationPath);

        public string CustomConfigurationPath { get; }

        public bool HasCustomSoundsPath => !string.IsNullOrEmpty(CustomSoundsPath);

        public string CustomSoundsPath { get; }

        public string LogPath { get; }
    }
}
