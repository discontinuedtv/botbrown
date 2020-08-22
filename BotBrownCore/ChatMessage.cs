namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;

    public class ChatMessage
    {
        private string customRewardId;
        private string normalizedMessage;
        private string channel;
        private string username;
        private ITwitchClientWrapper clientWrapper;

        public bool IsMessageFromModerator { get; internal set; }

        public bool IsMessageFromBroadcaster { get; internal set; }

        public string Message { get; }

        public ChatMessage(ITwitchClientWrapper clientWrapper, string message, bool isMessageFromBroadcaster, bool isMessageFromModerator, string customRewardId, string channel, string username)
        {
            normalizedMessage = message.Trim().ToLower();
            this.clientWrapper = clientWrapper;
            this.customRewardId = customRewardId;
            this.username = username;
            this.channel = channel;
            IsMessageFromBroadcaster = isMessageFromBroadcaster;
            IsMessageFromModerator = isMessageFromModerator;
            Message = message;
        }
        public bool MessageStartsWith(string expectedStartOfString)
        {
            return normalizedMessage.StartsWith(expectedStartOfString);
        }

        internal void SendReplyToChannel(string replyMessage)
        {
            clientWrapper.SendMessage(channel, replyMessage);
        }

        internal string ReplaceInNormalizedMessage(string stringToReplace, string stringToReplaceWith)
        {
            return normalizedMessage.Replace(stringToReplace, stringToReplaceWith).Trim();
        }

        internal void SendReplyToWhisper(string languages)
        {
            clientWrapper.SendWhisper(username, languages);
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
