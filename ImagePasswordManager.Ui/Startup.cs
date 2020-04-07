using System;
using System.Windows.Forms;
using SimplePasswordManager.States;

namespace SimplePasswordManager
{
    internal static class Startup
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(StartupState.StartupForm);
        }
    }
}