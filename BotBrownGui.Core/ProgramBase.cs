namespace BotBrown
{
    using BotBrown.Workers.Webserver;
    using SpiderEye;

    public abstract class ProgramBase
    {
        protected static void Run(BotArguments botArguments)
        {
            using (var bot = new Bot())
            using (var window = new Window())
            {
                bot.Execute(botArguments);

                window.UseBrowserTitle = false;
                window.Title = "Bot Brown";
                window.CanResize = true;
                window.SetWindowState(WindowState.Maximized);

                window.Icon = AppIcon.FromFile("botbrown", ".");
                window.EnableDevTools = false;

                // this relates to the path defined in the .csproj file
                Application.ContentProvider = new EmbeddedContentProvider("App");

                string port = botArguments.Port ?? (botArguments.IsDebug ? WebserverConstants.DebugPort : WebserverConstants.ProductivePort);
                Application.Run(window, $"http://localhost:{port}");
            }
        }
    }
}