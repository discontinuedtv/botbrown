using BotBrown.Configuration;
using BotBrown.Events;
using BotBrown.Events.Twitch;
using BotBrown.Models;
using System;
using System.Threading.Tasks;

namespace BotBrown.ChatCommands
{
    public class AddTtsCustomRewardCommand : BaseChatCommand
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IEventBus eventBus;

        public AddTtsCustomRewardCommand(IConfigurationManager configurationManager, IEventBus eventBus)
        {
            this.configurationManager = configurationManager;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "addttsreward" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            if (!chatCommandReceivedEvent.HasCustomRewardId)
            {
                return Task.CompletedTask;
            }

            var twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>();
            twitchConfiguration.TextToSpeechRewardId = chatCommandReceivedEvent.CustomRewardId;
            configurationManager.WriteConfiguration(twitchConfiguration);

            eventBus.Publish(new SendChannelMessageRequestedEvent($"Custom Reward wurde registriert.", chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
