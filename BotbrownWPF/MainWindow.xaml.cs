namespace BotbrownWPF
{
    using BotbrownWPF.ViewModels;
    using System.Windows;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();

            this.MenuTabControl.SelectedIndex = 1;
        }
    }
}
