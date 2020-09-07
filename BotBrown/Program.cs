namespace BotBrown
{
    using System;
    using SpiderEye.Windows;

    public class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            bool dontConnectToTwitch = false;
            bool isDebug = false;

            foreach (var arg in args)
            {
                if (arg == "-notwitch")
                {
                    dontConnectToTwitch = true;
                    continue;
                }

                if (arg == "-debug")
                {
                    isDebug = true;
                    continue;
                }
            }

            var botArguments = new BotArguments(isDebug, dontConnectToTwitch);

            WindowsApplication.Init();
            Run(botArguments);
        }
    }
}
