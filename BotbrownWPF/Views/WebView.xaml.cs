using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BotbrownWPF.Views
{
    /// <summary>
    /// Interaktionslogik für WebView.xaml
    /// </summary>
    public partial class WebView : Window
    {
        public WebView()
        {
            InitializeComponent();
            this.DataContext = new WebViewModel();

            //this.webView.Source = new Uri("https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=bv8ex3iuo52pc1ob3ti9lq698t45ey&redirect_uri=http://localhost&scope=viewing_activity_read&state=c3ab8aa609ea11e793ae92361f002671");

            this.WebView1.Source = new Uri("https://www.google.de");
            this.WebView1.Loaded += WebView_Loaded;
            //this.webView.Navigate("www.google.de");
        }

        private void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            var awe = 123;
        }

        private void WebBrowser_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            var awe = 123;

        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var asd = 3;

        }
    }
}
