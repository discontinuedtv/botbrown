namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class AddOrDeleteSimpleTextCommand : BaseChatCommand
    {
        private readonly IConfigurationManager configurationManager;

        public AddOrDeleteSimpleTextCommand(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "+", "-" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var splittedString = chatCommandReceivedEvent.CommandArgs.Split(' ');
            var command = splittedString[0];

            var simpleTextCommandConfiguration = configurationManager.LoadConfiguration<SimpleTextCommandConfiguration>();
            switch (chatCommandReceivedEvent.CommandText)
            {
                case "+":
                    var commandText = string.Join(" ", splittedString.Skip(1));
                    simpleTextCommandConfiguration.AddOrUpdateCommand(command, commandText);
                    break;
                case "-":
                    simpleTextCommandConfiguration.DeleteCommand(command);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
