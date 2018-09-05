using System;
using System.Windows.Forms;

namespace UI.Windows
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.Run(new MainForm());
        }
    }
}