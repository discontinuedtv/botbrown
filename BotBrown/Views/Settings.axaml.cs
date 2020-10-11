﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BotBrown.Configuration;

namespace BotBrown.Views
{
    public class Settings : UserControl
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var result = this.Get<StackPanel>("stacktwitch");

        }
    }
}
