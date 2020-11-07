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
        private readonly ISettingsViewModel vm;

        public SettingsPage(ISettingsViewModel vm)
        {
            this.vm = vm;
            InitializeComponent();

            this.DataContext = vm;
        }

        private void RequestToken_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WebViewModel webVm = new WebViewModel
            {
                ChannelName = vm.TwitchConfiguration.Channel
            };

            WebView webview = new WebView(webVm);

            webview.ShowDialog();
            vm.TwitchConfiguration.AccessToken = $"oauth:{webVm.AccessToken}";
            vm.TwitchConfiguration.BroadcasterUserId = webVm.UserInfo.Id;
        }

        private void SaveTwitch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.TwitchConfiguration.Save();
        }

        private void SaveAudio_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.AudioConfiguration.Save();
        }

        private void SaveGeneral_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.GeneralConfiguration.Save();
        }
    }
}
