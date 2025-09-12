using System;
using System.Windows.Forms;

namespace SqlServerTool
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // We now run the SplashScreen first, which will then launch Form1.
            Application.Run(new SplashScreen());
        }
    }
}
