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
        private GreetingConfiguration greetingConfiguration;
        private IDictionary<string, string> availableLanguages = new Dictionary<string, string>();
        private IDictionary<string, string> usernameCache = new Dictionary<string, string>();

        // TODOS fürs nächste mal:
        // Sprachen dynamisch ermitteln
        // TTS abschaltbar
        // Lautstärke einstellbar
        // Verabschiedungen erkennen

        // TODOS fürs übernächste mal:
        // Oberfläche für die Konfiguration

        public Bot()
        {
            registry = new ConfigurationFileFactoryRegistry();
            registry.AddFactory(new CommandConfigurationFileFactory());
            registry.AddFactory(new TwitchConfigurationFileFactory());
            registry.AddFactory(new GreetingConfigurationFileFactory());
            configurationManager = new ConfigurationManager(registry);

            logger.Debug("Configuration Manager wurde geladen.");

            synth.SetOutputToDefaultAudioDevice();

            availableLanguages.Add("deutsch", SyntheticLanguages.Deutsch);
            availableLanguages.Add("chinesisch", SyntheticLanguages.Chinesisch);
            availableLanguages.Add("polnisch", SyntheticLanguages.Polnisch);
            availableLanguages.Add("spanisch", SyntheticLanguages.Spanisch);
            availableLanguages.Add("russisch", SyntheticLanguages.Russisch);
            availableLanguages.Add("französisch", SyntheticLanguages.Französisch);
            availableLanguages.Add("amerikanisch", SyntheticLanguages.Amerikanisch);
        }

        public void Execute()
        {
            try
            {
                RefreshCommands();
                RefreshGreetings();

                TwitchConfiguration configuration = LoadTwitchConfiguration();
                logger.Log("Twitch Konfiguration wurden geladen.");

                if (!configuration.IsValid())
                {
                    logger.Log("Die Twitch Konfiguration ist nich valide");
                    return;
                }

                ConnectionCredentials credentials = new ConnectionCredentials(configuration.Username, configuration.AccessToken);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient customClient = new WebSocketClient(clientOptions);
                client = new TwitchClient(customClient);
                client.Initialize(credentials, configuration.Channel);

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

        private void RefreshGreetings()
        {
            greetingConfiguration = configurationManager.LoadConfiguration<GreetingConfiguration>(ConfigurationFileConstants.Greetings);
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
            if (!RecordGreetingLanguage(e))
            {
                GreetIfNecessary(e);
            }

            SpeakIfNecessary(e);

            Command foundCommand = FindCommandInMessage(e.ChatMessage);
            if (foundCommand != null)
            {
                foundCommand.Execute();
            }
        }

        private void SpeakIfNecessary(OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.CustomRewardId != "c15394f1-571b-4484-8d1c-b1b2384bae7a")
            {
                return;
            }

            Speak(e, (username) => $"{username} sagt: {e.ChatMessage.Message}");
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
                synth.SelectVoice(SyntheticLanguages.Deutsch);
            }

            synth.Speak(messageAction(targetUsername));
        }

        private string GetUsername(ChatMessage chatMessage)
        {
            if (usernameCache.TryGetValue(chatMessage.UserId, out string cachedUsername))
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
                else
                {
                    sb.Append(c);
                }
            }

            string targetUsername = sb.ToString();
            usernameCache[chatMessage.UserId] = targetUsername;
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
