namespace BotBrown.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
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

                

                return items;
            }
        }
    }
}
