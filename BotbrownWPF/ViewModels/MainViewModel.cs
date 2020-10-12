using BotbrownWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BotbrownWPF.ViewModels
{
    public class MainViewModel
    {
        public List<TabItem> Tabs
        {
            get
            {
                List<TabItem> tabs = new List<TabItem>();
                tabs.Add(new TabItem
                {
                    VerticalContentAlignment = VerticalAlignment.Top,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    IsEnabled = false,
                    Name = "Logo",
                    IsSelected = false,
                    Height = 112,
                    Header = new Image
                    {
                        Width = 112,
                        Source = new BitmapImage(new Uri("pack://application:,,,/Assets/doc-112x112.png"))
                    }
                });

                tabs.Add(new TabItem
                {
                    Header = "Startseite",
                    Content = new Startpage()
                });

                tabs.Add(new TabItem
                {
                    Header = "Kommandos",
                    Content = new Commandspage()
                });

                tabs.Add(new TabItem
                {
                    Header = "Sounds",
                    Content = new Soundspage()
                });

                tabs.Add(new TabItem
                {
                    Header = "Eigenschaften",
                    Content = new Settingspage()
                });

                return tabs;
            }
        }
    }
}
