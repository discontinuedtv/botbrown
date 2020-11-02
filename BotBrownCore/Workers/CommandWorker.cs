namespace BotBrown.Workers
{
    using BotBrown.ChatCommands;
    using BotBrown;
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog;

    public sealed class CommandWorker : IDisposable
    {
        private readonly IEventBus bus;
        private readonly IConfigurationManager configurationManager;
        private readonly IPresenceStore presenceStore;
        private readonly IChatCommandResolver chatCommandResolver;
        private readonly ILogger logger;
        private Guid identifier = Guid.NewGuid();

        public CommandWorker(
            IEventBus bus,
            IConfigurationManager configurationManager,
            IPresenceStore presenceStore,
            IChatCommandResolver chatCommandResolver,
            ILogger logger)
        {
            this.bus = bus;
            this.configurationManager = configurationManager;
            this.presenceStore = presenceStore;
            this.chatCommandResolver = chatCommandResolver;
            this.logger = logger.ForContext<CommandWorker>();
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
                try
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

                    await Task.Delay(100, cancellationToken);
                }
                catch (Exception e)
                {
                    logger.Error("Bei der Verarbeitung eines Kommandos ist ein Fehler aufgetreten: {e}", e);
                }
            }

            return true;
        }

        private void ProcessChatCommandReceivedEvent(ChatCommandReceivedEvent @event)
        {
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

            var simpleTextCommandConfiguration = configurationManager.LoadConfiguration<SimpleTextCommandConfiguration>();
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
            GreetIfNecessary(message);
            SayByeIfNecessary(message);
            CheckForSoundCommands(message);
            SpeakIfNecessary(message);
            CheerForBirthday(message);
        }

        private void CheckForSoundCommands(MessageReceivedEvent message)
        {
            if (!message.HasEmotesInMessage)
            {
                return;
            }

            var soundConfiguration = configurationManager.LoadConfiguration<SoundCommandConfiguration>();
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

        private void CheerForBirthday(MessageReceivedEvent message)
        {
            var birthdayConfiguration = configurationManager.LoadConfiguration<BirthdaysConfiguration>();
            DateTime now = DateTime.Now;
            if (birthdayConfiguration.ContainsBirthdayForDate(now))
            {
                var changed = false;
                var birthdays = birthdayConfiguration.GetBirthdays(now);
                foreach (var birthday in birthdays)
                {
                    if (birthday.UserId == message.User.UserId && !birthday.Gratulated.Contains(now.Year))
                    {
                        changed = true;
                        birthday.Gratulated.Add(now.Year);
                        bus.Publish(new TextToSpeechEvent(message.User, $"Alles Gute zu deinem Geburtstag {message.User.Username}! Genieße deinen Geburstag in unserem Stream. Mach es dir gemütlich."));
                        break;
                    }
                }

                if (changed)
                {
                    birthdayConfiguration.MarkChanged();
                }
            }
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
    }
}
