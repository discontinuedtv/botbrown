namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;
    using System.Speech.Synthesis;
    using System.Text;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;
    using TwitchLib.Communication.Clients;
    using TwitchLib.Communication.Models;
    using System.Collections.ObjectModel;
    using BotBrownCore.Configuration;

    public sealed class Bot : IDisposable
    {
        // https://github.com/TwitchLib/TwitchLib
        // https://twitchapps.com/tmi/

        private readonly IConfigurationFileFactoryRegistry registry;
        private readonly IConfigurationManager configurationManager;
        private readonly SpeechSynthesizer synth = new SpeechSynthesizer();

        private TwitchClient client;
        private Dictionary<string, Command> soundsPerCommand = new Dictionary<string, Command>();
        private readonly ILogger logger = new ConsoleLogger();

        private HashSet<string> greetedPeople = new HashSet<string>();
        private TwitchConfiguration twitchConfiguration;
        private GreetingConfiguration greetingConfiguration;
        private UsernameConfiguration usernameConfiguration;
        private string sprachen;

        private IDictionary<string, string> availableLanguages = new Dictionary<string, string>();

        // TODOS fürs nächste mal:
        // Lautstärke einstellbar
        // Verabschiedungen erkennen
        // TTS abschaltbar
        // Sprachen Whisper fixen

        // TODOS fürs übernächste mal:
        // Time Befehl
        // Timer Befehl
        // Oberfläche für die Konfiguration

        public Bot()
        {
            registry = new ConfigurationFileFactoryRegistry();
            registry.AddFactory(new CommandConfigurationFileFactory());
            registry.AddFactory(new TwitchConfigurationFileFactory());
            registry.AddFactory(new GreetingConfigurationFileFactory());
            registry.AddFactory(new UsernameConfigurationFileFactory());
            configurationManager = new ConfigurationManager(registry);

            logger.Debug("Configuration Manager wurde geladen.");

            synth.SetOutputToDefaultAudioDevice();

            RegisterAvailableLanguages();
        }

        public void Execute()
        {
            try
            {
                RefreshCommands();
                RefreshGreetings();
                RefreshUsernames();

                twitchConfiguration = LoadTwitchConfiguration();
                logger.Log("Twitch Konfiguration wurden geladen.");

                if (!twitchConfiguration.IsValid())
                {
                    logger.Log("Die Twitch Konfiguration ist nich valide");
                    return;
                }

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
                //client.OnWhisperReceived += Client_OnWhisperReceived;
                //client.OnNewSubscriber += Client_OnNewSubscriber;

                client.OnConnected += Client_OnConnected;

                client.Connect();
            }
            catch (Exception e)
            {
                logger.Log("Der Bot wurde aufgrund eines Fehlers beendet");
                logger.Error(e);
            }
        }

        private void RegisterAvailableLanguages()
        {
            ReadOnlyCollection<InstalledVoice> voices = synth.GetInstalledVoices();

            foreach (InstalledVoice voice in voices)
            {
                string[] languageName = voice.VoiceInfo.Culture.DisplayName.ToLower().Split(' ');
                availableLanguages.Add(languageName[0], voice.VoiceInfo.Name);               
            }
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
                Command command = commandDefinition.CreateCommand();
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
            client.SendMessage(e.Channel, "Hallo Leute, Bot Brown ist wieder da!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!RecordGreetingLanguage(e) && !RecordNamechange(e) && !GetLanguages(e))
            {
                GreetIfNecessary(e);
            }

            Command foundCommand = FindCommandInMessage(e.ChatMessage);
            if (foundCommand != null)
            {
                foundCommand.Execute();
                return;
            }

            if (SpeakIfNecessary(e))
            {
                return;
            }
        }

        private bool GetLanguages(OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.StartsWith("!sprachen", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrEmpty(sprachen))
            {
                InitializeSprachen();
            }

            client.SendWhisper(e.ChatMessage.Username, sprachen);
            return true;
        }

        private void InitializeSprachen()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Dies sind die verfügbaren Sprachen:");

            foreach (var item in availableLanguages)
            {
                sb.AppendLine(item.Key);
            }

            sprachen = sb.ToString();
        }

        private bool SpeakIfNecessary(OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.CustomRewardId.Equals(twitchConfiguration.TextToSpeechRewardId, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            Speak(e, (username) => $"{username} sagt: {e.ChatMessage.Message}");
            return true;
        }

        private bool RecordGreetingLanguage(OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.Trim().ToLower().StartsWith("!sprache:"))
            {
                return false;
            }

            string sprache = e.ChatMessage.Message.Trim().ToLower().Replace("!sprache:", string.Empty);

            if (availableLanguages.TryGetValue(sprache, out string language))
            {
                greetingConfiguration.AddGreeting(e.ChatMessage.UserId, language);
                configurationManager.WriteConfiguration(greetingConfiguration, ConfigurationFileConstants.Greetings);
                return true;
            }

            client.SendMessage(e.ChatMessage.Channel, $"Die Sprache {sprache} spreche ich leider nicht.");
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

            if (!usernameConfiguration.FindUserByRealUsername(namechangeParameters[0], out User user))
            {
                return false;
            }

            user.Username = namechangeParameters[1];
            configurationManager.WriteConfiguration(usernameConfiguration, ConfigurationFileConstants.Usernames);
            return true;
        }

        private void GreetIfNecessary(OnMessageReceivedArgs e)
        {
            if (!greetedPeople.Contains(e.ChatMessage.UserId))
            {
                Speak(e, (username) => $"Hallo {username}");
                greetedPeople.Add(e.ChatMessage.UserId);
            }
        }

        private void Speak(OnMessageReceivedArgs e, Func<string, string> messageAction)
        {
            string targetUsername = GetUsername(e.ChatMessage);

            if (greetingConfiguration.TryGetValue(e.ChatMessage.UserId, out string language))
            {
                synth.SelectVoice(language);
            }
            else
            {
                synth.SelectVoice("Microsoft Hedda Desktop");
            }

            synth.Speak(messageAction(targetUsername));
        }

        private string GetUsername(ChatMessage chatMessage)
        {
            if (usernameConfiguration.TryGetValue(chatMessage.UserId, out string cachedUsername))
            {
                return cachedUsername;
            }

            string username = chatMessage.DisplayName.Replace("_", " ");
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
            usernameConfiguration.AddUsername(chatMessage.UserId, chatMessage.DisplayName, targetUsername);
            configurationManager.WriteConfiguration(usernameConfiguration, ConfigurationFileConstants.Usernames);
            return targetUsername;
        }

        private Command FindCommandInMessage(ChatMessage chatMessage)
        {
            foreach (var command in soundsPerCommand.Keys)
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
        }

        /*
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "i_am_steve_oh")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }
        */

        /*
        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
        */
    }
}
