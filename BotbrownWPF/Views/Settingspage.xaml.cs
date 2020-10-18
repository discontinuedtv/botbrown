namespace BotbrownWPF.Views
{
    using BotbrownWPF.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl, ISettingsPage
    {
        public SettingsPage(ISettingsViewModel vm)
        {
            InitializeComponent();

            this.DataContext = vm;
        }
    }
}
