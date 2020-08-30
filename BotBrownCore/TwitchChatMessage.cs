namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;

    public class TwitchChatMessage
    {
        private string customRewardId;
        private string normalizedMessage;

        public bool IsMessageFromModerator { get; internal set; }

        public bool IsMessageFromBroadcaster { get; internal set; }

        public string Message { get; }

        public string ChannelName { get; }

        public TwitchChatMessage(string message, bool isMessageFromBroadcaster, bool isMessageFromModerator, string customRewardId, string channel)
        {
            normalizedMessage = message.ToLower();
            this.customRewardId = customRewardId;
            ChannelName = channel;
            IsMessageFromBroadcaster = isMessageFromBroadcaster;
            IsMessageFromModerator = isMessageFromModerator;
            Message = message;
        }
        public bool MessageStartsWith(string expectedStartOfString)
        {
            return normalizedMessage.StartsWith(expectedStartOfString);
        }

        public bool MessageIs(string expectedMessage)
        {
            return normalizedMessage == expectedMessage.ToLower();
        }

        internal string ReplaceInNormalizedMessage(string stringToReplace, string stringToReplaceWith)
        {
            return normalizedMessage.Replace(stringToReplace, stringToReplaceWith).Trim();
        }

        internal bool IsCustomRewardId(string expectedRewardId)
        {
            return customRewardId != null && customRewardId.Equals(expectedRewardId, StringComparison.OrdinalIgnoreCase);
        }

        internal IEnumerable<string> Split(char charToSplitBy)
        {
            return Message.Split(charToSplitBy);
        }

        internal bool IsGoodbyeMessage(HashSet<string> byePhrases)
        {
            if (Message.Contains("@"))
            {
                return false;
            }

            foreach (string word in Message.Split(' '))
            {
                if (byePhrases.Contains(word.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public string MessageContainsAnyCommand(IEnumerable<string> commandsToScanFor)
        {
            foreach (string command in commandsToScanFor)
            {
                if (Message.Contains(command))
                {
                    return command;
                }
            }

            return null;
        }
    }
}
