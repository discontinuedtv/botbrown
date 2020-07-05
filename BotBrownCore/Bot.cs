namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;
    using TwitchLib.Communication.Clients;
    using TwitchLib.Communication.Models;

    public sealed class Bot : IDisposable
    {
        private Dictionary<string, Command> soundsPerCommand = new Dictionary<string, Command>();
        private TwitchClient client;

        public Bot()
        {
            soundsPerCommand.Add("diehaaHype", new Command("HYPAHYPA", 60, "hypahypa.wav", 20));
            soundsPerCommand.Add("scoddiNice", new Command("NICE", 60, "clickNice.wav", 60));
            soundsPerCommand.Add("mopserBRAINDED", new Command("Hä?", 20, "haeh.wav", 40));
            soundsPerCommand.Add("fosuakLost", new Command("STOP IT", 20, "stopit.wav", 40));

            // https://github.com/TwitchLib/TwitchLib
            // https://twitchapps.com/tmi/

            BotConfiguration configuration = ReadBotConfiguration();

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

        public class Configuration : ConfigurationBuilder
        {

        }

        private BotConfiguration ReadBotConfiguration()
        {
            //var config = new Configuration().AddJsonFile("blubb.json", false, true).Build();

            string username = ConfigurationManager.AppSettings.Get("username");
            string accessToken = ConfigurationManager.AppSettings.Get("accessToken");
            string channel = ConfigurationManager.AppSettings.Get("channel");

            return new BotConfiguration(username, accessToken, channel); 
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
            Command foundCommand = FindCommandInMessage(e.ChatMessage);
            if (foundCommand != null)
            {
                foundCommand.Execute();
            }
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
