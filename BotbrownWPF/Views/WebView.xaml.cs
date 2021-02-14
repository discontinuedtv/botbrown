namespace BotbrownWPF.Views
{
    using BotBrown.Configuration;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Windows;

    /// <summary>
    /// Interaktionslogik für WebView.xaml
    /// </summary>
    public partial class WebView : Window
    {
        private readonly WebViewModel vm;

        public WebView(WebViewModel vm)
        {
            this.vm = vm;
            InitializeComponent();
            DataContext = vm;
            WebView1.Source = vm.TargetUri;
            WebView1.NavigationCompleted += WebView1_NavigationCompleted;
        }

        private async void WebView1_NavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
        {
            string windowLocationHref = await WebView1.InvokeScriptAsync("eval", "window.location.href");

            if (!Uri.TryCreate(windowLocationHref, UriKind.RelativeOrAbsolute, out Uri parsedUri))
            {
                return;
            }

            if (parsedUri.Fragment.Contains("#access"))
            {
                var queryParameters = HttpUtility.ParseQueryString(parsedUri.Fragment);

                var token = queryParameters.Get("#access_token");
                if (token != null)
                {
                    vm.AccessToken = token;
                    var userInfo = await RequestUserId(vm.ChannelName, token);
                    vm.UserInfo = userInfo;

                    Close();
                }
            }
        }

        private async Task<TwitchUserInfo> RequestUserId(string channelname, string accessToken)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri($"https://api.twitch.tv/helix/users?login={channelname}");

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                client.DefaultRequestHeaders.Add("Client-Id", TwitchConfiguration.ApiClientId);
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var resp = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TwitchApiResponse<TwitchUserInfo>>(resp);
                if (result?.Data == null || !result.Data.Any())
                {
                    throw new InvalidOperationException("Could not resolve Twitch user");
                }

                return result.Data.First();
            }
        }
    }
}
