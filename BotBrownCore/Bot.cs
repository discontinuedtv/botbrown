namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;
    using TwitchLib.Communication.Clients;
    using TwitchLib.Communication.Models;
    using BotBrownCore.Configuration;
    using TwitchLib.Api.Services;
    using TwitchLib.Api.Services.Events.FollowerService;
    using TwitchLib.Api;
    using TwitchLib.Api.Core;
    using TwitchLib.Api.Helix.Models.Users;
    using System.Threading.Tasks;

    public sealed class Bot : IDisposable
    {
        // https://github.com/TwitchLib/TwitchLib
        // https://twitchapps.com/tmi/
        // https://twitchtokengenerator.com/

        private readonly IConfigurationFileFactoryRegistry registry;
        private readonly IConfigurationManager configurationManager;

        private readonly TextToSpeechProcessor ttsProcessor = new TextToSpeechProcessor();
        private readonly IBotExecutionContext executionContext;

        private TwitchAPI api;
        private TwitchClient client;
        private FollowerService followerService;

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

        // TODOS fürs übernächste mal:
        // Lautstärke einstellbar -> Recherche
        // Commands extrahieren und erweiterbar machen
        // Konfigurationen in Datenbank speichern
        // Lautstärke einstellbar        
        // Oberfläche für die Konfiguration

        public Bot()
        {
            executionContext = new BotExecutionContext(ttsProcessor);

            registry = new ConfigurationFileFactoryRegistry();
            registry.AddFactory(new CommandConfigurationFileFactory());
            registry.AddFactory(new TwitchConfigurationFileFactory());
            registry.AddFactory(new GreetingConfigurationFileFactory());
            registry.AddFactory(new UsernameConfigurationFileFactory());
            registry.AddFactory(new SentenceConfigurationFileFactory());
            registry.AddFactory(new GeneralConfigurationFileFactory());
            configurationManager = new ConfigurationManager(registry);
            
            logger.Debug("Configuration Manager wurde geladen.");

            RefreshGeneralConfiguration();
            ttsProcessor.Configure(generalConfiguration);
            ttsProcessor.RegisterAvailableLanguages();
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

                InitializeTwitchLibChatClient();
                InitializeTwitchApiClient();
            }
            catch (Exception e)
            {
                logger.Log("Der Bot wurde aufgrund eines Fehlers beendet");
                logger.Error(e);
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

        private void InitializeTwitchApiClient()
        {
            api = new TwitchAPI(null, null, new ApiSettings
            {
                ClientId = twitchConfiguration.ApiClientId,
                AccessToken = twitchConfiguration.ApiAccessToken
            });

            followerService = new FollowerService(api, 10);
            followerService.SetChannelsByName(new List<string> { twitchConfiguration.Username }); // TODO ändern!
            followerService.OnNewFollowersDetected += Api_OnFollowerDetected;
            followerService.Start();
        }

        private void Api_OnFollowerDetected(object sender, OnNewFollowersDetectedArgs e)
        {
            DateTime dateToCheckAgainst = DateTime.UtcNow.AddSeconds(-90);

            foreach (Follow follow in e.NewFollowers.FindAll(x => x.FollowedAt >= dateToCheckAgainst))
            {
                var user = new ChannelUser(follow.FromUserId, follow.FromUserName, follow.FromUserName);

                Speak(user, (username) => string.Format(sentenceConfiguration.FollowerAlert, username));
            }            
        }

        private void InitializeTwitchLibChatClient()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(twitchConfiguration.Username, twitchConfiguration.AccessToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, twitchConfiguration.Channel);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnCommunitySubscription += Client_OnCommunitySubscription;
            client.OnReSubscriber += Client_OnReSubscriber;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnGiftedSubscription += Client_OnGiftedSubscription;

            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            ChannelUser user = new ChannelUser(e.GiftedSubscription.MsgParamRecipientId, e.GiftedSubscription.MsgParamRecipientUserName, e.GiftedSubscription.MsgParamRecipientUserName);
            Speak(user, (username) => string.Format(sentenceConfiguration.GiftedSubscriberAlert, username));
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            ChannelUser user = new ChannelUser(e.Subscriber.UserId, e.Subscriber.DisplayName, e.Subscriber.DisplayName);
            Speak(user, (username) => string.Format(sentenceConfiguration.SubscriberAlert, username));
        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            ChannelUser user = new ChannelUser(e.ReSubscriber.UserId, e.ReSubscriber.DisplayName, e.ReSubscriber.DisplayName);
            Speak(user, (username) => string.Format(sentenceConfiguration.ResubscriberAlert, username, e.ReSubscriber.MsgParamStreakMonths));
        }

        private void Client_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            ChannelUser user = new ChannelUser(e.GiftedSubscription.UserId, e.GiftedSubscription.DisplayName, e.GiftedSubscription.DisplayName);
            Speak(user, (username) => string.Format(sentenceConfiguration.SubBombAlert, username, e.GiftedSubscription.MsgParamMassGiftCount));
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
            // Load commands from configuration

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

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            if (string.IsNullOrEmpty(generalConfiguration.BotChannelGreeting))
            {
                return;
            }

            client.SendMessage(e.Channel, generalConfiguration.BotChannelGreeting);
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }

            string targetUsername = ResolveUsername(new ChannelUser(e.ChatMessage.UserId, e.ChatMessage.DisplayName, e.ChatMessage.DisplayName));

            if (TimerStart(e))
            {
                return;
            }

            if (WhatsTheTime(e))
            {
                return;
            }

            if (!RecordGreetingLanguage(e) && !RecordNamechange(e) && !GetLanguages(e))
            {
                GreetIfNecessary(e);
            }

            SayByeIfNecessary(e);

            ICommand foundCommand = FindCommandInMessage(e.ChatMessage);
            if (foundCommand != null)
            {
                foundCommand.Execute(executionContext);
                return;
            }

            if (SpeakIfNecessary(e))
            {
                return;
            }
        }

        private bool TimerStart(OnMessageReceivedArgs e)
        {
            // !timer 420 Ex-und-hopp
            if (!e.ChatMessage.Message.Trim().ToLower().StartsWith("!timer"))
            {
                return false;
            }

            string parameterString = e.ChatMessage.Message.Trim().ToLower().Replace("!timer", string.Empty).Trim();
            string[] parameters = parameterString.Split(' ');
            if (parameters.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(parameters[0], out int timeInSeconds))
            {
                return false;
            }

            if (!e.ChatMessage.IsBroadcaster)
            {
                return false;
            }

            Task.Delay(TimeSpan.FromSeconds(timeInSeconds))
                .ContinueWith(o => Speak(e.ChatMessage, (username) => $"Der Timer {parameters[1]} ist abgelaufen."));

            return true;
        }

        private bool WhatsTheTime(OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.Trim().ToLower().StartsWith("!time"))
            {
                return false;
            }

            if (!e.ChatMessage.IsModerator && !e.ChatMessage.IsBroadcaster)
            {
                return false;
            }

            DateTime now = DateTime.Now;

            Speak(e.ChatMessage, (username) => $"Die Zeitleitung zeigt {now:HH} Uhr {now:mm} an.");

            return true;
        }

        private bool GetLanguages(OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.StartsWith("!sprachen", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string languages = ttsProcessor.TextToSpeechLanguages;
            client.SendWhisper(e.ChatMessage.Username, languages);
            return true;
        }

        private bool SpeakIfNecessary(OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.CustomRewardId == null)
            {
                return false;
            }

            if (!e.ChatMessage.CustomRewardId.Equals(twitchConfiguration.TextToSpeechRewardId, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            Speak(e.ChatMessage, (username) => $"{username} sagt: {e.ChatMessage.Message}");
            return true;
        }

        private bool RecordGreetingLanguage(OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.Trim().ToLower().StartsWith("!sprache:"))
            {
                return false;
            }

            string requestedLanguage = e.ChatMessage.Message.Trim().ToLower().Replace("!sprache:", string.Empty);

            if (ttsProcessor.TryGetLanguage(requestedLanguage, out string language))
            {
                greetingConfiguration.AddGreeting(e.ChatMessage.UserId, language);
                configurationManager.WriteConfiguration(greetingConfiguration, ConfigurationFileConstants.Greetings);
                return true;
            }

            client.SendMessage(e.ChatMessage.Channel, $"Die Sprache {requestedLanguage} spreche ich leider nicht.");
            return false;
        }

        private bool RecordNamechange(OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.Trim().ToLower().StartsWith("!correctname:"))
            {
                return false;
            }

            if (!e.ChatMessage.IsModerator)
            {
                return false;
            }

            string namechange = e.ChatMessage.Message.Trim().ToLower().Replace("!correctname:", string.Empty);

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

        private void GreetIfNecessary(OnMessageReceivedArgs e)
        {
            if (presenceStore.IsGreetingNecessary(e.ChatMessage.UserId))
            {
                Speak(e.ChatMessage, (username) => $"Hallo {username}");
                presenceStore.RecordPresence(e.ChatMessage.UserId);
            }
        }

        private void SayByeIfNecessary(OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.Contains("@") && presenceStore.IsSayByeNecessary(e.ChatMessage.UserId))
            {
                foreach (string word in e.ChatMessage.Message.Split(' '))
                {
                    if (generalConfiguration.ByePhrases.Contains(word.ToLower()))
                    {
                        Speak(e.ChatMessage, (username) => string.Format(generalConfiguration.ByePhrase, username));
                        presenceStore.RemovePresence(e.ChatMessage.UserId);
                        break;
                    }
                }
            }
        }

        public void Speak(ChatMessage message, Func<string, string> messageAction)
        {
            string targetUsername = ResolveUsername(new ChannelUser(message.UserId, message.DisplayName, message.DisplayName));
            string languageForProcessing = greetingConfiguration.RetrieveDesiredLanguage(message.UserId);
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

        private ICommand FindCommandInMessage(ChatMessage chatMessage)
        {
            foreach (string command in soundsPerCommand.Keys)
            {
                if (chatMessage.Message.Contains(command))
                {
                    return soundsPerCommand[command];
                }
            }

            return null;
        }

        public void Dispose()
        {
            foreach (var command in soundsPerCommand)
            {
                command.Value.Dispose();
            }

            client.Disconnect();
            followerService.Stop();
        }
    }
}
