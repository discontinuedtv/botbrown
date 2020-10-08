using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BotBrown.Views
{
    public class StartPage : UserControl
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
