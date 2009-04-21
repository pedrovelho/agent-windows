using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using ProActiveAgent;
using Microsoft.Win32;

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
            // Check if this application is already running
            Process[] alreadyRunningProcesses = Process.GetProcessesByName("AgentForAgent");
            if (alreadyRunningProcesses != null && alreadyRunningProcesses.Length > 1)
            {
                return;
            }

            // Check agent and config locations in the registry (setted during agent installation)
            // ie check if the agent was correctly installed
            RegistryKey agentKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY);            
            if (agentKey != null)
            {
                string agentLocation = (string)agentKey.GetValue(Constants.PROACTIVE_AGENT_INSTALL_LOCATION_REG_VALUE_NAME);
                if (agentLocation == null)
                {
                    // report error and exit                     
                    MessageBox.Show("Cannot get the agent location in " + Constants.PROACTIVE_AGENT_REG_SUBKEY + ". It appears that the agent might not have been installed properly.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Close the registry key
                    agentKey.Close();
                    return;
                }

                string configLocation = (string)agentKey.GetValue(Constants.PROACTIVE_AGENT_CONFIG_LOCATION_REG_VALUE_NAME);
                if (configLocation == null)
                {
                    // report error and exit
                    MessageBox.Show("Cannot get the config location in " + Constants.PROACTIVE_AGENT_REG_SUBKEY + ". It appears that the agent might not have been installed properly.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Close the registry key
                    agentKey.Close();
                    return;
                }

                // Close the registry key
                agentKey.Close();

                // -------------------------------------------------
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ConfigurationDialog(agentLocation, configLocation));
            }
            else
            {
                // Cannot continue, report error message box and exit                         
                MessageBox.Show("Can not open the following registry subkey (LocalMachine) " + Constants.PROACTIVE_AGENT_REG_SUBKEY + ". It appears that the agent might not have been installed properly.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);             
            }
        }
    }
}
