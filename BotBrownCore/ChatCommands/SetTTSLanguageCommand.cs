namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers.TextToSpeech;
    using System;
    using System.Threading.Tasks;

    public class SetTTSLanguageCommand : BaseChatCommand
    {
        private readonly ITextToSpeechProcessor textToSpeechProcessor;
        private readonly IConfigurationManager configurationManager;
        private readonly IEventBus eventBus;

        public SetTTSLanguageCommand(ITextToSpeechProcessor textToSpeechProcessor, IConfigurationManager configurationManager, IEventBus eventBus)
        {
            this.textToSpeechProcessor = textToSpeechProcessor;
            this.configurationManager = configurationManager;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.All;

        public override string[] Commands => new[] { "sprache" };

        public override TimeSpan Cooldown => TimeSpan.FromSeconds(10);

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            if (textToSpeechProcessor.TryGetLanguage(chatCommandReceivedEvent.CommandArgs, out string language)) // TODO !!
            {
                var greetingConfiguration = configurationManager.LoadConfiguration<GreetingConfiguration>();
                greetingConfiguration.AddGreeting(chatCommandReceivedEvent.User, language);
                configurationManager.WriteConfiguration(greetingConfiguration);
                return Task.CompletedTask;
            }

            eventBus.Publish(new SendChannelMessageRequestedEvent($"Die Sprache {chatCommandReceivedEvent.CommandArgs} spreche ich leider nicht.", chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
