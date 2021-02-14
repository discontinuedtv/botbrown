namespace BotbrownWPF.ViewModels
{
    public interface ISoundsPageViewModel : IViewModel
    {
        void Save();

        void AddSound(SoundViewModel soundToAdd);

        string DestinationPathFor(string targetFileName);

        bool HasExistingDefinitionForShortcut(string shortcut);
    }
}