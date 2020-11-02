namespace BotbrownWPF.Views
{
    using BotbrownWPF.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für Soundspage.xaml
    /// </summary>
    public partial class Soundspage : UserControl, ISoundsPage
    {
        private readonly ISoundsPageViewModel vm;

        public Soundspage(ISoundsPageViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.Save();
        }
    }
}
