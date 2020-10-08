namespace BotBrown.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Templates;
    using Avalonia.Markup.Xaml.Templates;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using BotBrown.Views;
    using System.Collections.Generic;

    public class MainWindowViewModel : ViewModelBase
    {
        IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

        public string Greeting => "Welcome to Avalonia!";

        public IList<TabItem> MenuItems
        {
            get
            {
                var items = new List<TabItem>();

                items.Add(new TabItem()
                {
                    VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Top,
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    IsEnabled = false,
                    Name = "Logo",
                    IsSelected = false,
                    Header = new Image() { Source = new Bitmap(assets.Open(new System.Uri("avares://BotBrown/Assets/doc-112x112.png"))) }
                });

                items.Add(new TabItem()
                {
                    IsSelected = true,
                    Header = "Startseite",
                    Content = new StartPage()
                });

                items.Add(new TabItem()
                {
                    Header = "Kommandos",
                    Content = new Commands()
                });

                items.Add(new TabItem()
                {
                    Header = "Sounds",
                    Content = new Sounds()
                });

                items.Add(new TabItem()
                {
                    Header = "Eigenschaften",
                    Content = new Settings()
                });

                return items;
            }
        }
    }
}
