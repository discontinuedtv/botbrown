namespace BotbrownWPF.ViewModels
{
    using BotbrownWPF.Views;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    public class MainViewModel : IMainViewModel, IViewModel
    {
        private ISettingsPage settingsPage;

        public MainViewModel(ISettingsPage settingsPage)
        {
            this.settingsPage = settingsPage;
        }

        public ObservableCollection<TabItem> Tabs
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
                    Content = settingsPage
                });

                return new ObservableCollection<TabItem>(tabs);
            }
        }
    }
}
