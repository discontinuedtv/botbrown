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
            string port = null;

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

                if (arg.StartsWith("-port:"))
                {
                    port = arg.Split(':')[1];
                }
            }

            var botArguments = new BotArguments(isDebug, dontConnectToTwitch, port);

            WindowsApplication.Init();
            Run(botArguments);
        }
    }
}
