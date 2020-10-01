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
            string customConfigurationPath = null;
            string customSoundsPath = null;

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

                if (arg.StartsWith("-ccp:"))
                {
                    customConfigurationPath = arg.Split(':')[1];
                    continue;
                }

                if (arg.StartsWith("-csp:"))
                {
                    customSoundsPath = arg.Split(':')[1];
                    continue;
                }
            }

            var botArguments = new BotArguments(isDebug, dontConnectToTwitch, customConfigurationPath, customSoundsPath);

            WindowsApplication.Init();
            Run(botArguments);
        }
    }
}
