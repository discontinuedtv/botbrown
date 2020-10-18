namespace BotbrownWPF
{
    using BotBrown;
    using BotBrown.DI;
    using System;
    using System.Windows;

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private BotContainer container;
        private Bot bot;

        protected override void OnStartup(StartupEventArgs startupEventArguments)
        {
            base.OnStartup(startupEventArguments);

            string[] args = startupEventArguments.Args ?? Array.Empty<string>();

            BotArguments botArguments = CreateBotArguments(args);

            container = new BotContainer(botArguments);
            ConfigureContainer(container);

            CreateAndStartBot(botArguments);

            CreateAndStartWindow(container);            
        }

        private void CreateAndStartWindow(BotContainer container)
        {
            IViewFactory factory = container.Resolve<IViewFactory>();
            IMainView main = factory.CreateView<IMainView>();
            main.ShowDialog();
        }

        private void ConfigureContainer(BotContainer container)
        {
            container.Install(new WpfInstaller());
        }

        private void CreateAndStartBot(BotArguments botArguments)
        {
            bot = new Bot();
            bot.Execute(botArguments, container);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            container?.Dispose();
            bot?.Dispose();
        }

        private static BotArguments CreateBotArguments(string[] args)
        {
            return new BotArgumentsFactory().BuildFrom(args);
        }
    }
}
