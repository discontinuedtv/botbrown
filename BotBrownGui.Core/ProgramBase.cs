namespace BotBrown
{
    using SpiderEye;
    using System;
    using System.Diagnostics;

    public abstract class ProgramBase
    {
        protected static void Run(bool dontConnectToTwitch)
        {
            using (var bot = new Bot())
            using (var window = new Window())
            {
                bot.Execute(dontConnectToTwitch);

                window.UseBrowserTitle = false;
                window.Title = "Bot Brown";

                window.Icon = AppIcon.FromFile("botbrown", ".");
                window.EnableDevTools = false;

                // this relates to the path defined in the .csproj file
                Application.ContentProvider = new EmbeddedContentProvider("App");

                // runs the application and opens the window with the given page loaded
                //Application.Run(window, "index.html");
                Application.Run(window, "http://localhost:12345");
            }
        }
    }
}