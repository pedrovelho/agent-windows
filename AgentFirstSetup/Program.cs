using System;
using System.ServiceProcess;
using System.Windows.Forms;
using ProActiveAgent;

namespace AgentFirstSetup
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // If no args then print usage
            if (args == null || args.Length == 0 || args[0].Equals("-h"))
            {
                string usage = "Usage:\n";
                usage += "-i agent_exe_dir\t to install the ProActive Agent service. The agent_exe_dir must contain " + Constants.PROACTIVE_AGENT_EXECUTABLE_NAME + "\n";
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
                    Console.WriteLine("To install the ProActive Agent service please specify the directory that contains " + Constants.PROACTIVE_AGENT_EXECUTABLE_NAME);
                }
                else
                {
                    // Uninstall precedent versions of the service
                    internalUninstall();

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainWindow(args[1]));
                }
            }
            // -u for unistall
            else if (args[0].Equals("-u"))
            {
                internalUninstall();
            }
            // Unknown option
            else
            {
                Console.WriteLine("Unknown option: " + args[0] + " Use -h option for more help.");
            }
        }

        private static void internalUninstall()
        {
            try
            {
                // Stop the service 
                ServiceController sc = new ServiceController(Constants.PROACTIVE_AGENT_SERVICE_NAME);
                sc.Stop();
            }
            catch (Exception)
            {
                // Nothing to do ...
            }
            // Before uninstallation of the service kill AgentForAgent.exe or ConfigParser.exe if it's running            
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in processes)
            {
                try
                {
                    if (process.ProcessName.Equals("AgentForAgent.exe") || process.ProcessName.Equals("ConfigParser.exe"))
                    {
                        process.CloseMainWindow();                        
                        process.Close();                        
                        if (!process.HasExited)
                        {
                            process.WaitForExit();
                        }                        
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception) { }
                    // Do nothing ... 
                }
            }

            // Proceed with uninstallation
            // Try sc windows utility method
            try
            {
                // Uninstall previous version of the service                
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "sc";
                proc.StartInfo.Arguments = "delete \"" + Constants.PROACTIVE_AGENT_SERVICE_NAME + "\"";
                proc.Start();
                proc.WaitForExit(5 * 1000);  // 5 sec timeout   
            }
            catch (Exception)
            {
                // If failed use manual method
                SrvInstaller.UnInstallService(Constants.PROACTIVE_AGENT_SERVICE_NAME);
            }
        }
    }
}
