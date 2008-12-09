using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AgentFirstSetup
{
    public static class Program
    {
        // The name of the ProActive Agent executable
        public const string PROACTIVE_AGENT_EXECUTABLE_NAME = "ProActiveAgent.exe";
        public const string PROACTIVE_AGENT_NAME = "ProActive Agent";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // If no args then print usage
            if (args == null || args.Length  == 0 || args[0].Equals("-h"))
            {
                string usage = "Usage:\n";
                usage += "-i agent_exe_dir\t to install the ProActive Agent service. The agent_exe_dir must contain " + PROACTIVE_AGENT_EXECUTABLE_NAME + "\n";
                usage += "-u\t to uninstall the ProActive Agent service.\n";
                usage += "-h\t prints this help message.";
                Console.WriteLine(usage);
            }

            // Check option
            // -i for install            
            if (args[0].Equals("-i"))
            {
                if (args[1] == null || args[1].Length == 0)
                {
                    Console.WriteLine("To install the ProActive Agent service please specify the directory that contains " + PROACTIVE_AGENT_EXECUTABLE_NAME);
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainWindow(args[1]));

                }
            }
            // -u for unistall
            else if (args[0].Equals("-u"))
            {
                SrvInstaller.UnInstallService("ProActive Agent");
            }
            // Unknown option
            else
            {
                Console.WriteLine("Uknown option: " + args[0] + " Use -h option for more help.");
            }                                         
        }
    }
}
