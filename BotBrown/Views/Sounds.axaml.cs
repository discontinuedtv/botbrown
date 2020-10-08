using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BotBrown.Views
{
    public class Sounds : UserControl
    {
        public Sounds()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
