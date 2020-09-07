﻿namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Workers.TextToSpeech;
    using BotBrown.Workers.Timers;
    using BotBrownCore.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class CommandWorker : IDisposable
    {
        private readonly IEventBus bus;
        private readonly IConfigurationManager configurationManager;
        private readonly IPresenceStore presenceStore;
        private readonly ILogger logger;
        private readonly ITextToSpeechProcessor textToSpeechProcessor; // TODO !

        private Dictionary<string, SoundCommand> soundsPerCommand = new Dictionary<string, SoundCommand>();
        private List<TimerCommand> timerCommands = new List<TimerCommand>();

        private Guid identifier = Guid.NewGuid();

        private SentenceConfiguration sentenceConfiguration;
        private GeneralConfiguration generalConfiguration;
        private TwitchConfiguration twitchConfiguration;
        private GreetingConfiguration greetingConfiguration;
        private UsernameConfiguration usernameConfiguration;

        public CommandWorker(IEventBus bus, IConfigurationManager configurationManager, IPresenceStore presenceStore, ITextToSpeechProcessor textToSpeechProcessor, ILogger logger)
        {
            this.bus = bus;
            this.configurationManager = configurationManager;
            this.presenceStore = presenceStore;
            this.logger = logger;
            this.textToSpeechProcessor = textToSpeechProcessor;
        }

        public async Task<bool> Execute(CancellationToken cancellationToken)
        {
            sentenceConfiguration = configurationManager.LoadConfiguration<SentenceConfiguration>(ConfigurationFileConstants.Sentences);
            generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>(ConfigurationFileConstants.General);
            twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);
            greetingConfiguration = configurationManager.LoadConfiguration<GreetingConfiguration>(ConfigurationFileConstants.Greetings);
            usernameConfiguration = configurationManager.LoadConfiguration<UsernameConfiguration>(ConfigurationFileConstants.Usernames);

            RefreshCommands();

            bus.SubscribeToTopic<MessageReceivedEvent>(identifier);
            bus.SubscribeToTopic<NewFollowerEvent>(identifier);
            bus.SubscribeToTopic<SubGiftEvent>(identifier);
            bus.SubscribeToTopic<NewSubscriberEvent>(identifier);
            bus.SubscribeToTopic<ResubscriberEvent>(identifier);
            bus.SubscribeToTopic<CommunitySubscriptionEvent>(identifier);
            bus.SubscribeToTopic<TwitchChannelJoinedEvent>(identifier);
            bus.SubscribeToTopic<PlaySoundRequestedEvent>(identifier);
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

                if (bus.TryConsume(identifier, out PlaySoundRequestedEvent playSoundRequestedEvent))
                {
                    ProcessPlaySoundRequestedEvent(playSoundRequestedEvent);
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
            ChannelUser user = @event.User;
            string channelName = @event.ChannelName;

            if (@event.CommandText == "re")
            {
                // TODO: Vielleicht lieber Zurück aus der Zukunft Daten? 1955, 1985, 2015

                Random rand = new Random();
                int year = rand.Next(1400, 2180);

                bus.Publish(new SendChannelMessageRequestedEvent($"{user.RealUsername} ist zurück von der Zeitreise aus dem Jahr {year}", channelName));
                return;
            }
        }

        public void Dispose()
        {
            foreach (var command in soundsPerCommand)
            {
                command.Value.Dispose();
            }
        }

        private void RefreshCommands()
        {
            soundsPerCommand.Clear();
            CommandConfiguration commandConfiguration = configurationManager.LoadConfiguration<CommandConfiguration>(ConfigurationFileConstants.Commands);
            AudioConfiguration audioConfiguration = configurationManager.LoadConfiguration<AudioConfiguration>(ConfigurationFileConstants.Audio);
            
            foreach (CommandDefinition commandDefinition in commandConfiguration.CommandsDefinitions)
            {
                SoundCommand command = commandDefinition.CreateCommand(audioConfiguration);
                soundsPerCommand.Add(command.Shortcut, command);

                logger.Log($"Kommando {command.Shortcut} hinzugefügt.");
            }

            logger.Log("Kommandos wurden geladen.");
        }

        private void ProcessNewFollowerEvent(NewFollowerEvent newFollowerEvent)
        {
            foreach (ChannelUser newFollow in newFollowerEvent.NewFollowers)
            {
                bus.Publish(new TextToSpeechEvent(newFollow, string.Format(sentenceConfiguration.FollowerAlert, newFollow.Username)));
            }
        }

        private void ProcessSubGiftEvent(SubGiftEvent subGiftEvent)
        {
            ChannelUser user = subGiftEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.GiftedSubscriberAlert, user.Username)));
        }

        private void ProcessNewSubscriberEvent(NewSubscriberEvent newSubscriberEvent)
        {
            ChannelUser user = newSubscriberEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.SubscriberAlert, user.Username)));
        }

        private void ProcessResubscriberEvent(ResubscriberEvent resubscriberEvent)
        {
            ChannelUser user = resubscriberEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.ResubscriberAlert, user.Username, resubscriberEvent.NumberOfMonthsSubscribed)));
        }

        private void ProcessCommunitySubscriptionEvent(CommunitySubscriptionEvent communitySubscriptionEvent)
        {
            ChannelUser user = communitySubscriptionEvent.User;
            bus.Publish(new TextToSpeechEvent(user, string.Format(sentenceConfiguration.SubBombAlert, user.Username, communitySubscriptionEvent.NumberOfSubscriptionsGifted)));
        }

        private void ProcessChannelJoinedEvent(TwitchChannelJoinedEvent channelJoinedEvent)
        {
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

            if (!RecordGreetingLanguage(message) && !RecordNamechange(message.Message) && !GetLanguages(message))
            {
                GreetIfNecessary(message);
            }

            SayByeIfNecessary(message);

            string commandName = message.Message.MessageContainsAnyCommand(soundsPerCommand.Keys);
            if (commandName != null)
            {
                soundsPerCommand[commandName].Execute();
                return;
            }

            if (SpeakIfNecessary(message))
            {
                return;
            }
        }

        private void ProcessPlaySoundRequestedEvent(PlaySoundRequestedEvent @event)
        {
            if (soundsPerCommand.TryGetValue(@event.CommandName, out SoundCommand command))
            {
                command.Execute();
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
                greetingConfiguration.AddGreeting(user, language);
                configurationManager.WriteConfiguration(greetingConfiguration, ConfigurationFileConstants.Greetings);
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
                bus.Publish(new TextToSpeechEvent(user, $"Hallo {user.Username}"));
                presenceStore.RecordPresence(user);
            }
        }

        private bool RecordNamechange(TwitchChatMessage chatMessage)
        {
            if (!chatMessage.MessageStartsWith("!correctname:"))
            {
                return false;
            }

            if (!chatMessage.IsMessageFromModerator)
            {
                return false;
            }

            string namechange = chatMessage.ReplaceInNormalizedMessage("!correctname:", string.Empty);

            string[] namechangeParameters = namechange.Split(';');
            if (namechangeParameters.Length != 2)
            {
                return false;
            }

            if (!usernameConfiguration.FindUserByRealUsername(namechangeParameters[0], out ChannelUser user))
            {
                return false;
            }

            user.Username = namechangeParameters[1];
            configurationManager.WriteConfiguration(usernameConfiguration, ConfigurationFileConstants.Usernames);
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
