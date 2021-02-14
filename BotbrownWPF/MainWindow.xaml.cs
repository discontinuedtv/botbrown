namespace BotbrownWPF
{
    using BotbrownWPF.ViewModels;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainView
    {
        public MainWindow(IMainViewModel mainViewModel)
        {
            InitializeComponent();

            this.DataContext = mainViewModel;
            this.MenuTabControl.SelectedIndex = 1;

            Debugger.Launch();
            Debugger.Break();
        }
    }
}
