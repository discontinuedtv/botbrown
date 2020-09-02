namespace BotBrown
{
    using SpiderEye;
    using System;
    using System.Diagnostics;
    using System.Threading;

    public abstract class ProgramBase
    {
        protected static void Run()
        {
            using (var bot = new Bot())
            using (var window = new Window())
            {
                bot.Execute();

                window.Icon = AppIcon.FromFile("botbrown", ".");
                window.EnableDevTools = true;

                // this relates to the path defined in the .csproj file
                Application.ContentProvider = new EmbeddedContentProvider("App");

                // runs the application and opens the window with the given page loaded
                Application.Run(window, "index.html");
            }
        }

        [Conditional("DEBUG")]
        private static void SetDevSettings(Window window)
        {
            window.EnableDevTools = true;

            // this is just to give some suggestions in case something isn't set up correctly for development
            window.PageLoaded += (s, e) =>
            {
                if (!e.Success)
                {
                    string message = $"Page did not load!{Environment.NewLine}Did you start the Angular dev server?";
                    if (Application.OS == SpiderEye.OperatingSystem.Windows)
                    {
                        message += $"{Environment.NewLine}On Windows 10 you also have to allow localhost. More info can be found in the SpiderEye readme.";
                    }

                    MessageBox.Show(window, message, "Page load failed", MessageBoxButtons.Ok);
                }
            };
        }
    }
}