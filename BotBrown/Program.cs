namespace BotBrown
{
    using Avalonia;
    using Avalonia.ReactiveUI;
    using BotBrown.DI;
    using CefGlue.Avalonia;
    using System;

    public class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            BotArguments botArguments = new BotArguments(true, true, null, null, "./logs");
            using (var container = new BotContainer(botArguments))
            using (var bot = new Bot())
            {
                bot.Execute(botArguments, container);
                AvaloniaLocator.Current = new BotBrownLocator(container, AvaloniaLocator.Current);

                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
#if DEBUG
            BotArguments botArguments = new BotArguments(true, true, null, null, "./logs");
            AvaloniaLocator.Current = new BotBrownLocator(new BotContainer(botArguments), AvaloniaLocator.Current);
#endif
            return AppBuilder.Configure<App>()
                  .UsePlatformDetect()
                  .UseSkia()
                  .ConfigureCefGlue(Array.Empty<string>())
                  .UseReactiveUI();
        }
    }
}
