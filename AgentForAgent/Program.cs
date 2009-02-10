using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace AgentForAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Check if its already running
            Process[] alreadyRunningProcesses = Process.GetProcessesByName("AgentForAgent");
            if (alreadyRunningProcesses != null && alreadyRunningProcesses.Length > 1)
            {
                return;
            }

            //-------------------------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConfigurationDialog());
        }
    }
}
