namespace BotbrownWPF.Views
{
    using BotbrownWPF.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl, ISettingsPage
    {
        private ISettingsViewModel vm;
        public SettingsPage(ISettingsViewModel vm)
        {
            this.vm = vm;
            InitializeComponent();

            this.DataContext = vm;
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.Save();
        }
    }
}
