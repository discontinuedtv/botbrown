namespace BotbrownWPF.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaktionslogik für WebView.xaml
    /// </summary>
    public partial class WebView : Window
    {
        private WebViewModel vm;

        public WebView(WebViewModel vm)
        {
            this.vm = vm;
            InitializeComponent();
            DataContext = vm;
            WebView1.Source = new Uri("https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=bv8ex3iuo52pc1ob3ti9lq698t45ey&redirect_uri=http://www.legien.org&scope=viewing_activity_read&state=c3ab8aa609ea11e793ae92361f002671");
            WebView1.NavigationCompleted += WebView1_NavigationCompleted;
        }

        private async void WebView1_NavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
        {
            string windowLocationHref = await WebView1.InvokeScriptAsync("eval", "window.location.href");
            vm.AccessToken = windowLocationHref;
            Close();
        }
    }
}
