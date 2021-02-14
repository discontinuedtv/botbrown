namespace BotbrownWPF.ViewModels
{
    using System.ComponentModel;

    public class Notifier : INotifyPropertyChanged
    {
        private bool isDirty;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (propertyName != nameof(IsDirty))
            {
                IsDirty = true;
            }

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }

            protected set
            {
                if (value != isDirty)
                {
                    isDirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }
    }
}
