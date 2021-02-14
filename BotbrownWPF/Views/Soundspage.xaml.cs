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
            this.vm = vm;
            InitializeComponent();
            this.DataContext = vm;
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.Save();
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addSoundView = new AddSoundView(vm);
            addSoundView.ShowDialog();
        }
    }
}
