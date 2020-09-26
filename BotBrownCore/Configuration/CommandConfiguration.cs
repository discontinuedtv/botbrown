namespace BotBrown.Configuration
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class CommandConfiguration : IChangeableConfiguration
    {
        public CommandConfiguration()
        {
            CommandsDefinitions.CollectionChanged += CommandsDefinitions_CollectionChanged;
        }

        private void CommandsDefinitions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CommandsDefinitions)));
        }

        public ObservableCollection<CommandDefinition> CommandsDefinitions { get; set; } = new ObservableCollection<CommandDefinition>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}