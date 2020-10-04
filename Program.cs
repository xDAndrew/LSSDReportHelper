using System;
using System.Windows.Forms;
using WindowsFormsApp1;
using LowLevelHooking;

namespace LSSDReportHelper
{
    internal static class Program
    {
        public static GlobalKeyboardHook GlobalKeyboardHook { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (GlobalKeyboardHook = new GlobalKeyboardHook())
            {
                Application.Run(new MainForm());
            }
        }
    }
}
