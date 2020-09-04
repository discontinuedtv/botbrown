namespace BotBrown
{
    using System;
    using System.Linq;
    using SpiderEye.Windows;

    public class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var dontConnectToTwitch = args.Any(x => x == "-notwitch");

            WindowsApplication.Init();
            Run(dontConnectToTwitch);
        }
    }
}
