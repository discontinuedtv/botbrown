using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace BotbrownWPF.Views
{
    /// <summary>
    /// Interaktionslogik für AddSoundView.xaml
    /// </summary>
    public partial class AddSoundView : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        private readonly ViewModels.ISoundsPageViewModel vm;
        private readonly ViewModels.SoundViewModel soundViewModel;
        private readonly EditMode mode;

        public AddSoundView(ViewModels.ISoundsPageViewModel vm)
        {
            this.vm = vm;
            this.mode = EditMode.Add;
            soundViewModel = new ViewModels.SoundViewModel(new BotBrown.CommandDefinition() { Volume = 100, CooldownInSeconds = 60 }, Validate);
            this.DataContext = soundViewModel;
            InitializeComponent();
        }      

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private bool Validate(BotBrown.CommandDefinition definition)
        {
            if (mode == EditMode.Add)
            {
                return !vm.HasExistingDefinitionForShortcut(definition.Shortcut);
            }

            return false;
        }

        private void PreviewTextInputHandlerasd(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.AddSound(soundViewModel);
            Close();
        }

        private void Choose_SoundFile(object sender, System.Windows.RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                DialogResult result = openFileDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var srcFilePath = Path.GetFullPath(openFileDialog.FileName);
                    var targetFileName = Path.GetFileName(openFileDialog.FileName);
                    var targetFilePath = vm.DestinationPathFor(targetFileName);

                    if (!File.Exists(targetFilePath))
                    {
                        File.Copy(srcFilePath, targetFilePath);
                    }

                    soundViewModel.Filename = targetFileName;
                }
            }
        }
    }
}
