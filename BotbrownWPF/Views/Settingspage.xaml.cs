namespace BotbrownWPF.Views
{
    using BotbrownWPF.ViewModels;
    using System.Diagnostics;
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

        private void RequestToken_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var webview = new WebView();
            webview.Show();
        }
    }
}
