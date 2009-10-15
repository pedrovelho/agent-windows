/*
* ################################################################
*
* ProActive: The Java(TM) library for Parallel, Distributed,
*            Concurrent computing with Security and Mobility
*
* Copyright (C) 1997-2009 INRIA/University of Nice-Sophia Antipolis
* Contact: proactive@ow2.org
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either version
* 2 of the License, or any later version.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this library; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
*
*  Initial developer(s):               The ProActive Team
*                        http://proactive.inria.fr/team_members.htm
*  Contributor(s): ActiveEon Team - http://www.activeeon.com
*
* ################################################################
* $$ACTIVEEON_CONTRIBUTOR$$
*/
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using ProActiveAgent;
using Microsoft.Win32;
using System.Security.Principal;

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
            // Check if the current user have admin rights   
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                // report error and exit                     
                MessageBox.Show("Adminstrator rights are required to run the ProActive Agent Control.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
