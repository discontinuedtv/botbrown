namespace BotBrown
{
    using System;
    using SpiderEye.Windows;

    public class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            WindowsApplication.Init();
            Run();
        }
    }
}
