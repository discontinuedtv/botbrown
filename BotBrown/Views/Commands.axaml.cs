﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BotBrown.Views
{
    public class Commands : UserControl
    {
        public Commands()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
