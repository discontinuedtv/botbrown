using BotbrownWPF.Views;
using System;

namespace BotbrownWPF.ViewModels
{
    public class SoundViewModel : Notifier, IViewModel
    {
        private readonly Func<BotBrown.CommandDefinition, bool> validationFunc;

        public SoundViewModel(BotBrown.CommandDefinition definition, Func<BotBrown.CommandDefinition, bool> validationFunc)
        {
            Definition = definition;
            this.validationFunc = validationFunc;
        }

        public BotBrown.CommandDefinition Definition { get; private set; }

        public string Name
        {
            get
            {
                return Definition.Name;
            }

            set
            {
                if (value != Definition.Name)
                {
                    Definition.Name = value;
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public float Volume
        {
            get
            {
                return Definition.Volume;
            }

            set
            {
                if (value != Definition.Volume)
                {
                    Definition.Volume = value;
                    OnPropertyChanged(nameof(Volume));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public string Shortcut
        {
            get
            {
                return Definition.Shortcut;
            }

            set
            {
                if (value != Definition.Shortcut)
                {
                    Definition.Shortcut = value;
                    OnPropertyChanged(nameof(Shortcut));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public string Filename
        {
            get
            {
                return Definition.Filename;
            }

            set
            {
                if (value != Definition.Filename)
                {
                    Definition.Filename = value;
                    OnPropertyChanged(nameof(Filename));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public int CooldownInSeconds
        {
            get
            {
                return Definition.CooldownInSeconds;
            }

            set
            {
                if (value != Definition.CooldownInSeconds)
                {
                    Definition.CooldownInSeconds = value;
                    OnPropertyChanged(nameof(CooldownInSeconds));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public string VolumeInPercent
        {
            get
            {
                return $"{Convert.ToInt32(Volume)}%";
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Name) 
                    && !string.IsNullOrEmpty(Shortcut) 
                    && !string.IsNullOrEmpty(Filename)
                    && validationFunc(Definition);
            }
        }
    }
}
