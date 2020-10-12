namespace BotBrown.Workers
{
    using BotBrown.ChatCommands;
    using BotBrown;
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Workers.TextToSpeech;
    using BotBrown.Workers.Timers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog;

    public sealed class CommandWorker : IDisposable
    {
        private readonly IEventBus bus;
        private readonly IConfigurationManager configurationManager;
        private readonly IPresenceStore presenceStore;
        private readonly IChatCommandResolver chatCommandResolver;
        private readonly ITextToSpeechProcessor textToSpeechProcessor; // TODO !
        private readonly List<TimerCommand> timerCommands = new List<TimerCommand>();
        private Guid identifier = Guid.NewGuid();

        public CommandWorker(
            IEventBus bus,
            IConfigurationManager configurationManager,
            IPresenceStore presenceStore,
            ITextToSpeechProcessor textToSpeechProcessor,
            IChatCommandResolver chatCommandResolver)
        {
            this.bus = bus;
            this.configurationManager = configurationManager;
            this.presenceStore = presenceStore;
            this.chatCommandResolver = chatCommandResolver;
            this.textToSpeechProcessor = textToSpeechProcessor;
        }

        public async Task<bool> Execute(CancellationToken cancellationToken)
        {
            bus.SubscribeToTopic<MessageReceivedEvent>(identifier);
            bus.SubscribeToTopic<NewFollowerEvent>(identifier);
            bus.SubscribeToTopic<SubGiftEvent>(identifier);
            bus.SubscribeToTopic<NewSubscriberEvent>(identifier);
            bus.SubscribeToTopic<ResubscriberEvent>(identifier);
            bus.SubscribeToTopic<CommunitySubscriptionEvent>(identifier);
            bus.SubscribeToTopic<TwitchChannelJoinedEvent>(identifier);
            bus.SubscribeToTopic<ChatCommandReceivedEvent>(identifier);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (bus.TryConsume(identifier, out MessageReceivedEvent message))
                {
                    ProcessChatMessage(message);
                }

                if (bus.TryConsume(identifier, out NewFollowerEvent newFollower))
                {
                    ProcessNewFollowerEvent(newFollower);
                }

                if (bus.TryConsume(identifier, out SubGiftEvent subGiftEvent))
                {
                    ProcessSubGiftEvent(subGiftEvent);
                }

                if (bus.TryConsume(identifier, out NewSubscriberEvent newSubscriberEvent))
                {
                    ProcessNewSubscriberEvent(newSubscriberEvent);
                }

                if (bus.TryConsume(identifier, out ResubscriberEvent resubscriberEvent))
                {
                    ProcessResubscriberEvent(resubscriberEvent);
                }

                if (bus.TryConsume(identifier, out CommunitySubscriptionEvent communitySubscriptionEvent))
                {
                    ProcessCommunitySubscriptionEvent(communitySubscriptionEvent);
                }

                if (bus.TryConsume(identifier, out TwitchChannelJoinedEvent channelJoinedEvent))
                {
                    ProcessChannelJoinedEvent(channelJoinedEvent);
                }

                if (bus.TryConsume(identifier, out ChatCommandReceivedEvent chatCommandReceivedEvent))
                {
                    ProcessChatCommandReceivedEvent(chatCommandReceivedEvent);
                }

                await Task.Delay(100);
            }

            return true;
        }

        private void ProcessChatCommandReceivedEvent(ChatCommandReceivedEvent @event)
        {
            var simpleTextCommandConfiguration = configurationManager.LoadConfiguration<SimpleTextCommandConfiguration>();
            var chatCommands = chatCommandResolver.ResolveAllChatCommands();

            foreach (var command in chatCommands)
            {
                if (command.CanConsume(@event))
                {
                    var shouldContinue = command.Consume(@event).Result;
                    if (!shouldContinue)
                    {
                        return;
                    }
                }
            }

            if (simpleTextCommandConfiguration.Commands.ContainsKey(@event.CommandText))
            {
                string optionalUser = @event.OptionalUser;
                if (string.IsNullOrEmpty(optionalUser))
                {
                    bus.Publish(new SendChannelMessageRequestedEvent(simpleTextCommandConfiguration.Commands[@event.CommandText], @event.ChannelName));
                    return;
                }

                bus.Publish(new SendChannelMessageRequestedEvent($"{optionalUser}: {simpleTextCommandConfiguration.Commands[@event.CommandText]}", @event.ChannelName));
                return;
            }
        }

        public void Dispose()
        {
        }

        private void ProcessNewFollowerEvent(NewFollowerEvent newFollowerEvent)
        {
            var sentenceConfiguration = configurationManager.LoadConfiguration<SentenceConfiguration>();
            foreach (ChannelUser newFollow in newFollowerEvent.NewFollowers)
            {
                bus.Publish(new TextToSpeechEvent(newFollow, string.Format(sentenceConfiguration.FollowerAlert, newFollow.Username)));
            }
        }

        private void ProcessSubGiftEvent(SubGiftEvent subGiftEvent)
        {
            var sentenceConfiguration = configurationManager.LoadConfiguration<SentenceConfiguration>();
            ChannelUser user = subGiftEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.GiftedSubscriberAlert, user.Username)));
        }

        private void ProcessNewSubscriberEvent(NewSubscriberEvent newSubscriberEvent)
        {
            var sentenceConfiguration = configurationManager.LoadConfiguration<SentenceConfiguration>();
            ChannelUser user = newSubscriberEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.SubscriberAlert, user.Username)));
        }

        private void ProcessResubscriberEvent(ResubscriberEvent resubscriberEvent)
        {
            var sentenceConfiguration = configurationManager.LoadConfiguration<SentenceConfiguration>();
            ChannelUser user = resubscriberEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.ResubscriberAlert, user.Username, resubscriberEvent.NumberOfMonthsSubscribed)));
        }

        private void ProcessCommunitySubscriptionEvent(CommunitySubscriptionEvent communitySubscriptionEvent)
        {
            var sentenceConfiguration = configurationManager.LoadConfiguration<SentenceConfiguration>();
            ChannelUser user = communitySubscriptionEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.SubBombAlert, user.Username, communitySubscriptionEvent.NumberOfSubscriptionsGifted)));
        }

        private void ProcessChannelJoinedEvent(TwitchChannelJoinedEvent channelJoinedEvent)
        {
            var generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>();
            if (string.IsNullOrEmpty(generalConfiguration.BotChannelGreeting))
            {
                return;
            }

            bus.Publish(new SendChannelMessageRequestedEvent(generalConfiguration.BotChannelGreeting, channelJoinedEvent.ChannelName));
        }

        private void ProcessChatMessage(MessageReceivedEvent message)
        {
            if (WhatsTheTime(message))
            {
                return;
            }

            if (ListTimers(message))
            {
                return;
            }

            if (TimerStart(message))
            {
                return;
            }

            if (AddEditOrDeleteSimpleTextCommand(message))
            {
                return;
            }

            if (!RecordGreetingLanguage(message) && !RecordNamechange(message.Message) && !GetLanguages(message))
            {
                GreetIfNecessary(message);
            }

            SayByeIfNecessary(message);
            CheckForSoundCommands(message);

            if (SpeakIfNecessary(message))
            {
                return;
            }
        }

        private void CheckForSoundCommands(MessageReceivedEvent message)
        {
            if (!message.HasEmotesInMessage)
            {
                return;
            }

            var soundConfiguration = configurationManager.LoadConfiguration<CommandConfiguration>();
            foreach (var emoteInMessage in message.EmotesInMessage)
            {
                if (!soundConfiguration.TryGetDefinition(emoteInMessage.Name, out CommandDefinition commandDefinition))
                {
                    continue;
                }

                var command = commandDefinition.CreateCommand();
                if (!command.ShouldExecute)
                {
                    continue;
                }

                bus.Publish(new PlaySoundEvent(command.Filename, command.Volume));
                command.MarkAsExecuted();
            }
        }

        private bool WhatsTheTime(MessageReceivedEvent @event)
        {
            TwitchChatMessage message = @event.Message;
            ChannelUser user = @event.User;

            if (!message.MessageIs("!time"))
            {
                return false;
            }

            if (!message.IsMessageFromModerator && !message.IsMessageFromBroadcaster)
            {
                return false;
            }

            DateTime now = DateTime.Now;
            bus.Publish(new TextToSpeechEvent(user, $"Die Zeitleitung zeigt {now:HH} Uhr {now:mm} an."));
            return true;
        }

        private bool AddEditOrDeleteSimpleTextCommand(MessageReceivedEvent @event)
        {
            TwitchChatMessage message = @event.Message;

            if (!message.IsMessageFromModerator && !message.IsMessageFromBroadcaster)
            {
                return false;
            }

            if (!message.MessageStartsWith("+!") && !message.MessageStartsWith("-!"))
            {
                return false;
            }

            var simpleTextCommandConfiguration = configurationManager.LoadConfiguration<SimpleTextCommandConfiguration>();
            simpleTextCommandConfiguration.ProcessMessage(message.Message);
            return true;
        }

        private bool ListTimers(MessageReceivedEvent @event)
        {
            TwitchChatMessage message = @event.Message;
            ChannelUser user = @event.User;

            // !timer 420 Ex-und-hopp
            if (!message.MessageIs("!timers"))
            {
                return false;
            }

            var activeTimers = timerCommands.Where(x => x.IsRunning).ToList();
            if (!activeTimers.Any())
            {
                bus.Publish(new SendChannelMessageRequestedEvent($"Es sind keine Timer aktiv", @event.Message.ChannelName));
                return true;
            }

            List<string> expiringTimers = new List<string>();
            foreach (TimerCommand timer in activeTimers)
            {
                expiringTimers.Add($"{timer.Name}: {timer.FormattedTimeLeft}");
            }

            string expiringTimerOutput = string.Join(", ", expiringTimers);

            bus.Publish(new SendChannelMessageRequestedEvent($"Folgende Timer sind aktiv -> {expiringTimerOutput}", @event.Message.ChannelName));

            return true;
        }

        private bool TimerStart(MessageReceivedEvent @event)
        {
            TwitchChatMessage message = @event.Message;
            ChannelUser user = @event.User;

            // !timer 420 Ex-und-hopp
            if (!message.MessageStartsWith("!timer "))
            {
                return false;
            }

            string parameterString = message.ReplaceInNormalizedMessage("!timer ", string.Empty);
            string[] parameters = parameterString.Split(' ');
            if (parameters.Length < 2)
            {
                return false;
            }

            if (!int.TryParse(parameters[0], out int timeInSeconds))
            {
                return false;
            }

            if (!message.IsMessageFromBroadcaster && !message.IsMessageFromModerator)
            {
                return false;
            }

            string timerName = string.Join(" ", parameters.Skip(1));

            bus.Publish(new TextToSpeechEvent(user, $"Der Timer {timerName} wurde gestartet."));

            TimeSpan timerLength = TimeSpan.FromSeconds(timeInSeconds);

            DateTime now = DateTime.Now;
            DateTime doneAt = now.Add(timerLength);
            timerCommands.Add(new TimerCommand(timerName, doneAt, new DefaultTimeProvider()));

            Task.Delay(timerLength)
                .ContinueWith(o => PublishTimeEnded(timerName, user));

            return true;
        }

        private void PublishTimeEnded(string timerName, ChannelUser user)
        {
            bus.Publish(new TextToSpeechEvent(user, $"Der Timer {timerName} ist abgelaufen."));

            foreach (var timer in timerCommands.Where(x => x.Name == timerName))
            {
                timer.Done();
            }
        }

        private bool SpeakIfNecessary(MessageReceivedEvent @event)
        {
            TwitchChatMessage chatMessage = @event.Message;
            ChannelUser user = @event.User;

            var twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>();
            if (!chatMessage.IsCustomRewardId(twitchConfiguration.TextToSpeechRewardId))
            {
                return false;
            }

            bus.Publish<TextToSpeechEvent>(new SpeakEvent(user, chatMessage.Message));

            return true;
        }

        private void SayByeIfNecessary(MessageReceivedEvent @event)
        {
            ChannelUser user = @event.User;

            if (!presenceStore.IsSayByeNecessary(user))
            {
                return;
            }

            TwitchChatMessage message = @event.Message;

            var generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>();
            if (message.IsGoodbyeMessage(generalConfiguration.ByePhrases))
            {
                bus.Publish(new TextToSpeechEvent(user, string.Format(generalConfiguration.ByePhrase, user.Username)));
                presenceStore.RemovePresence(user);
            }
        }

        private bool RecordGreetingLanguage(MessageReceivedEvent @event)
        {
            TwitchChatMessage chatMessage = @event.Message;
            ChannelUser user = @event.User;

            if (!chatMessage.MessageStartsWith("!sprache"))
            {
                return false;
            }

            string requestedLanguage = chatMessage.ReplaceInNormalizedMessage("!sprache:", string.Empty);

            if (textToSpeechProcessor.TryGetLanguage(requestedLanguage, out string language)) // TODO !!
            {
                var greetingConfiguration = configurationManager.LoadConfiguration<GreetingConfiguration>();
                greetingConfiguration.AddGreeting(user, language);
                configurationManager.WriteConfiguration(greetingConfiguration);
                return true;
            }

            bus.Publish(new SendChannelMessageRequestedEvent($"Die Sprache {requestedLanguage} spreche ich leider nicht.", @event.Message.ChannelName));
            return false;
        }

        private void GreetIfNecessary(MessageReceivedEvent @event)
        {
            ChannelUser user = @event.User;

            if (presenceStore.IsGreetingNecessary(user))
            {
                var now = DateTimeOffset.Now;
                string? phrase;

                if (now.Hour > 0 && now.Hour < 11)
                {
                    phrase = "Guten Morgen";
                }
                else if (now.Hour >= 11 && now.Hour < 18)
                {
                    phrase = "Hallo";
                }
                else
                {
                    phrase = "Guten Abend";
                }

                bus.Publish(new TextToSpeechEvent(user, $"{phrase} {user.Username}"));
                presenceStore.RecordPresence(user);
            }
        }

        private bool RecordNamechange(TwitchChatMessage chatMessage)
        {
            if (!chatMessage.MessageStartsWith("!correctname:"))
            {
                return false;
            }

            if (!chatMessage.IsMessageFromModerator && !chatMessage.IsMessageFromBroadcaster)
            {
                return false;
            }

            string namechange = chatMessage.ReplaceInNormalizedMessage("!correctname:", string.Empty);

            string[] namechangeParameters = namechange.Split(';');
            if (namechangeParameters.Length != 2)
            {
                return false;
            }

            var usernameConfiguration = configurationManager.LoadConfiguration<UsernameConfiguration>();
            if (!usernameConfiguration.FindUserByRealUsername(namechangeParameters[0], out ChannelUser user))
            {
                return false;
            }

            usernameConfiguration.AddUsername(user.WithResolvedUsername(namechangeParameters[1]));
            return true;
        }

        private bool GetLanguages(MessageReceivedEvent message)
        {
            if (!message.Message.MessageStartsWith("!sprachen"))
            {
                return false;
            }

            string languages = textToSpeechProcessor.TextToSpeechLanguages;
            bus.Publish(new SendWhisperMessageRequestedEvent(message.User, languages));
            return true;
        }
    }
}
