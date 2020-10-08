namespace BotBrown
{
    using Avalonia;
    using Avalonia.Logging.Serilog;
    using Avalonia.ReactiveUI;

    public class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            var botArguments = new BotArguments(true, true, null, null, "/logs");
            using (var bot = new Bot())
            {
                bot.Execute(botArguments);

                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                  .UsePlatformDetect()
                  .LogToDebug()
                  .UseReactiveUI();
        }
    }
}
