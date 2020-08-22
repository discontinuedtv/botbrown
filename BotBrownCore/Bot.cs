namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using BotBrownCore.Configuration;
    using System.Threading.Tasks;
    using System.Linq;
    using BotBrownCore.Events;

    public sealed class Bot : IDisposable
    {
        // https://github.com/TwitchLib/TwitchLib
        // https://twitchapps.com/tmi/
        // https://twitchtokengenerator.com/

        private readonly IConfigurationManager configurationManager;

        private readonly TextToSpeechProcessor ttsProcessor = new TextToSpeechProcessor();
        private readonly IBotExecutionContext executionContext;

        private ITwitchApiWrapper apiWrapper;
        private ITwitchClientWrapper clientWrapper;

        private Dictionary<string, SoundCommand> soundsPerCommand = new Dictionary<string, SoundCommand>();
        private readonly ILogger logger = new ConsoleLogger();

        private TwitchConfiguration twitchConfiguration;
        private GreetingConfiguration greetingConfiguration;
        private UsernameConfiguration usernameConfiguration;
        private SentenceConfiguration sentenceConfiguration;
        private GeneralConfiguration generalConfiguration;

        private PresenceStore presenceStore = new PresenceStore();

        // TODOS fürs nächste mal:
        // Subscriber, Resubscriber vorlesen lassen --> testen
        // Aufräumen des Codes
        // Konfigurationen in Datenbank speichern <- Wirklich? Oder lieber Plugin das reagiert?

        // TODOS fürs übernächste mal:
        // Lautstärke einstellbar -> Recherche
        // Commands extrahieren und erweiterbar machen
        // Lautstärke einstellbar        
        // Oberfläche für die Konfiguration

        public Bot(ITwitchApiWrapper apiWrapper, ITwitchClientWrapper clientWrapper, IConfigurationManager configurationManager)
        {
            this.apiWrapper = apiWrapper;
            this.clientWrapper = clientWrapper;
            this.configurationManager = configurationManager;

            executionContext = new BotExecutionContext(ttsProcessor);
           
            RefreshGeneralConfiguration();
            ttsProcessor.Configure(generalConfiguration);
            ttsProcessor.RegisterAvailableLanguages();

            apiWrapper.Subscribe(new Subscriber<NewFollowerEvent>(AlertForNewFollowers));

            clientWrapper.Subscribe(new Subscriber<SubGiftEvent>((e) => Speak(e.User, (username) => string.Format(sentenceConfiguration.GiftedSubscriberAlert, username))));
            clientWrapper.Subscribe(new Subscriber<NewSubscriberEvent>((e) => Speak(e.User, (username) => string.Format(sentenceConfiguration.SubscriberAlert, username))));
            clientWrapper.Subscribe(new Subscriber<ResubscriberEvent>((e) => Speak(e.User, (username) => string.Format(sentenceConfiguration.ResubscriberAlert, username, e.NumberOfMonthsSubscribed))));
            clientWrapper.Subscribe(new Subscriber<CommunitySubscriptionEvent>((e) => Speak(e.User, (username) => string.Format(sentenceConfiguration.SubBombAlert, username, e.NumberOfSubscriptionsGifted))));
            clientWrapper.Subscribe(new Subscriber<MessageReceivedEvent>(OnNewMessage));
            clientWrapper.Subscribe(new Subscriber<ChannelJoinedEvent>(OnChannelJoined));
        }

        public void Execute()
        {
            try
            {
                RefreshCommands();
                RefreshGreetings();
                RefreshUsernames();
                RefreshSentences();

                twitchConfiguration = LoadTwitchConfiguration();
                logger.Log("Twitch Konfiguration wurden geladen.");

                if (!twitchConfiguration.IsValid())
                {
                    logger.Log("Die Twitch Konfiguration ist nich valide");
                    return;
                }

                clientWrapper.ConnectToTwitch(twitchConfiguration);
                apiWrapper.ConnectToTwitch(twitchConfiguration);
            }
            catch (Exception e)
            {
                logger.Log("Der Bot wurde aufgrund eines Fehlers beendet");
                logger.Error(e);
            }
        }

        private void OnChannelJoined(ChannelJoinedEvent @event)
        {
            if (string.IsNullOrEmpty(generalConfiguration.BotChannelGreeting))
            {
                return;
            }

            clientWrapper.SendMessage(@event.ChannelName, generalConfiguration.BotChannelGreeting);
        }

        private void OnNewMessage(MessageReceivedEvent @event)
        {
            string targetUsername = ResolveUsername(@event.User); // ?

            if (TimerStart(@event))
            {
                return;
            }

            if (WhatsTheTime(@event))
            {
                return;
            }

            if (!RecordGreetingLanguage(@event) && !RecordNamechange(@event.Message) && !GetLanguages(@event.Message))
            {
                GreetIfNecessary(@event);
            }

            SayByeIfNecessary(@event);

            string commandName = @event.Message.MessageContainsAnyCommand(soundsPerCommand.Keys);
            if (commandName != null)
            {
                soundsPerCommand[commandName].Execute(executionContext);
                return;
            }

            if (SpeakIfNecessary(@event))
            {
                return;
            }
        }

        private void AlertForNewFollowers(NewFollowerEvent @event)
        {
            foreach (ChannelUser newFollow in @event.NewFollowers)
            {
                Speak(newFollow, (username) => string.Format(sentenceConfiguration.FollowerAlert, username));
            }
        }

        private void RefreshGeneralConfiguration()
        {
            generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>(ConfigurationFileConstants.General);
        }

        private void RefreshSentences()
        {
            sentenceConfiguration = configurationManager.LoadConfiguration<SentenceConfiguration>(ConfigurationFileConstants.Sentences);
        }

        private void RefreshGreetings()
        {
            greetingConfiguration = configurationManager.LoadConfiguration<GreetingConfiguration>(ConfigurationFileConstants.Greetings);
        }

        private void RefreshUsernames()
        {
            usernameConfiguration = configurationManager.LoadConfiguration<UsernameConfiguration>(ConfigurationFileConstants.Usernames);
        }

        public void RefreshCommands()
        {
            soundsPerCommand.Clear();
            CommandConfiguration commandConfiguration = LoadCommandConfiguration();
            foreach (CommandDefinition commandDefinition in commandConfiguration.CommandsDefinitions)
            {
                SoundCommand command = commandDefinition.CreateCommand();
                soundsPerCommand.Add(command.Shortcut, command);

                logger.Log($"Kommando {command.Shortcut} hinzugefügt.");
            }

            logger.Log("Kommandos wurden geladen.");
        }

        private CommandConfiguration LoadCommandConfiguration()
        {
            return configurationManager.LoadConfiguration<CommandConfiguration>(ConfigurationFileConstants.Commands);
        }

        private TwitchConfiguration LoadTwitchConfiguration()
        {
            return configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);
        }

        private bool TimerStart(MessageReceivedEvent @event)
        {
            ChatMessage message = @event.Message;
            ChannelUser user = @event.User;

            // !timer 420 Ex-und-hopp
            if (!message.MessageStartsWith("!timer"))
            {
                return false;
            }

            string parameterString = message.ReplaceInNormalizedMessage("!timer", string.Empty);
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

            Speak(@event.User, (username) => $"Der Timer {timerName} wurde gestartet.");

            Task.Delay(TimeSpan.FromSeconds(timeInSeconds))
                .ContinueWith(o => Speak(user, (username) => $"Der Timer {timerName} ist abgelaufen."));

            return true;
        }

        private bool WhatsTheTime(MessageReceivedEvent @event)
        {
            ChatMessage message = @event.Message;
            ChannelUser user = @event.User;

            if (!message.MessageStartsWith("!time"))
            {
                return false;
            }

            if (!message.IsMessageFromModerator && !message.IsMessageFromBroadcaster)
            {
                return false;
            }

            DateTime now = DateTime.Now;

            Speak(user, (username) => $"Die Zeitleitung zeigt {now:HH} Uhr {now:mm} an.");

            return true;
        }

        private bool GetLanguages(ChatMessage chatMessage)
        {
            if (!chatMessage.MessageStartsWith("!sprachen"))
            {
                return false;
            }

            string languages = ttsProcessor.TextToSpeechLanguages;
            chatMessage.SendReplyToWhisper(languages);
            return true;
        }

        private bool SpeakIfNecessary(MessageReceivedEvent @event)
        {
            ChatMessage chatMessage = @event.Message;
            ChannelUser user = @event.User;

            if (!chatMessage.IsCustomRewardId(twitchConfiguration.TextToSpeechRewardId))
            {
                return false;
            }

            Speak(user, (username) => $"{username} sagt: {chatMessage.Message}");
            return true;
        }

        private bool RecordGreetingLanguage(MessageReceivedEvent @event)
        {
            ChatMessage chatMessage = @event.Message;
            ChannelUser user = @event.User;

            if (!chatMessage.MessageStartsWith("!sprache"))
            {
                return false;
            }

            string requestedLanguage = chatMessage.ReplaceInNormalizedMessage("!sprache:", string.Empty);

            if (ttsProcessor.TryGetLanguage(requestedLanguage, out string language))
            {
                greetingConfiguration.AddGreeting(user, language);
                configurationManager.WriteConfiguration(greetingConfiguration, ConfigurationFileConstants.Greetings);
                return true;
            }

            chatMessage.SendReplyToChannel($"Die Sprache {requestedLanguage} spreche ich leider nicht.");           
            return false;
        }

        private bool RecordNamechange(ChatMessage chatMessage)
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

        private void GreetIfNecessary(MessageReceivedEvent @event)
        {
            ChannelUser user = @event.User;

            if (presenceStore.IsGreetingNecessary(user))
            {
                Speak(user, (username) => $"Hallo {username}");
                presenceStore.RecordPresence(user);
            }
        }

        private void SayByeIfNecessary(MessageReceivedEvent @event)
        {
            ChannelUser user = @event.User;

            if (!presenceStore.IsSayByeNecessary(user))
            {
                return;
            }

            ChatMessage message = @event.Message;

            if (message.IsGoodbyeMessage(generalConfiguration.ByePhrases))
            {
                Speak(user, (username) => string.Format(generalConfiguration.ByePhrase, username));
                presenceStore.RemovePresence(user);
            }
        }

        public void Speak(MessageReceivedEvent message, Func<string, string> messageAction)
        {
            ChannelUser user = message.User;
            string targetUsername = ResolveUsername(user);
            string languageForProcessing = greetingConfiguration.RetrieveDesiredLanguage(user.UserId);
            ttsProcessor.Speak(targetUsername, languageForProcessing, messageAction);
        }

        public void Speak(ChannelUser user, Func<string, string> messageAction)
        {
            string targetUsername = ResolveUsername(user);
            string languageForProcessing = greetingConfiguration.RetrieveDesiredLanguage(user.UserId);
            ttsProcessor.Speak(targetUsername, languageForProcessing, messageAction);
        }

        private string ResolveUsername(ChannelUser user)
        {
            if (usernameConfiguration.TryGetValue(user.UserId, out string cachedUsername))
            {
                return cachedUsername;
            }

            string username = user.Username.Replace("_", " ");
            var sb = new StringBuilder();

            foreach (char c in username)
            {
                if (char.IsUpper(c))
                {
                    sb.Append($" {c}");
                }
                else if (char.IsNumber(c))
                {
                    continue;
                }
                else
                {
                    sb.Append(c);
                }
            }

            string targetUsername = sb.ToString();
            usernameConfiguration.AddUsername(user.UserId, user.RealUsername, targetUsername);
            user.Username = targetUsername;
            configurationManager.WriteConfiguration(usernameConfiguration, ConfigurationFileConstants.Usernames);
            return targetUsername;
        }

        public void Dispose()
        {
            foreach (var command in soundsPerCommand)
            {
                command.Value.Dispose();
            }

            clientWrapper.Stop();
            apiWrapper.Stop();
        }
    }
}
