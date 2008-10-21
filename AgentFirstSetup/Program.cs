using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AgentFirstSetup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainWindow());
            
            string path = "";
            foreach (string str in args)
            {
                path = String.Concat(path, str);
            }

            Application.Run(new MainWindow(path));
        }
    }
}
