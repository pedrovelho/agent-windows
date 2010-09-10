/*
 * ################################################################
 *
 * ProActive: The Java(TM) library for Parallel, Distributed,
 *            Concurrent computing with Security and Mobility
 *
 * Copyright (C) 1997-2010 INRIA/University of 
 *                                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
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
 * If needed, contact us to obtain a release under GPL Version 2 
 * or a different license than the GPL.
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
using ConfigParser;
using Microsoft.Win32;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Text;
using ProActiveAgent;

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
            if (!ProcessChecker.IsOnlyProcess("ProActive Agent Control"))
            {
                return;
            }

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
                ConfigurationDialog dialog = new ConfigurationDialog(agentLocation, configLocation);
                Application.Run(dialog);
                dialog.Show();
            }
            else
            {
                // Cannot continue, report error message box and exit                         
                MessageBox.Show("Can not open the following registry subkey (LocalMachine) " + Constants.PROACTIVE_AGENT_REG_SUBKEY + ". It appears that the agent might not have been installed properly.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Check running processes for an already-running instance. Implements a simple and
    /// always effective algorithm to find currently running processes with a main window
    /// matching a given substring and focus it.
    /// Combines code written by Lion Shi (MS) and Sam Allen.
    /// </summary>
    static class ProcessChecker
    {
        /// <summary>
        /// Stores a required string that must be present in the window title for it
        /// to be detected.
        /// </summary>
        static string _requiredString;

        /// <summary>
        /// Contains signatures for C++ DLLs using interop.
        /// </summary>
        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProcDel lpEnumFunc,
                Int32 lParam);

            [DllImport("user32.dll")]
            public static extern int GetWindowThreadProcessId(IntPtr hWnd,
                ref Int32 lpdwProcessId);

            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
                Int32 nMaxCount);

            public const int SW_SHOWNORMAL = 1;
        }

        public delegate bool EnumWindowsProcDel(IntPtr hWnd, Int32 lParam);

        /// <summary>
        /// Perform finding and showing of running window.
        /// </summary>
        /// <returns>Bool, which is important and must be kept to match up
        /// with system call.</returns>
        static private bool EnumWindowsProc(IntPtr hWnd, Int32 lParam)
        {
            int processId = 0;
            NativeMethods.GetWindowThreadProcessId(hWnd, ref processId);

            StringBuilder caption = new StringBuilder(1024);
            NativeMethods.GetWindowText(hWnd, caption, 1024);

            // Use IndexOf to make sure our required string is in the title.
            if (processId == lParam && (caption.ToString().IndexOf(_requiredString,
                StringComparison.OrdinalIgnoreCase) != -1))
            {
                // Restore the window.
                NativeMethods.ShowWindowAsync(hWnd, NativeMethods.SW_SHOWNORMAL);
                NativeMethods.SetForegroundWindow(hWnd);
            }
            return true; // Keep this.
        }

        /// <summary>
        /// Find out if we need to continue to load the current process. If we
        /// don't focus the old process that is equivalent to this one.
        /// </summary>
        /// <param name="forceTitle">This string must be contained in the window
        /// to restore. Use a string that contains the most
        /// unique sequence possible. If the program has windows with the string
        /// "Journal", pass that word.</param>
        /// <returns>False if no previous process was activated. True if we did
        /// focus a previous process and should simply exit the current one.</returns>
        static public bool IsOnlyProcess(string forceTitle)
        {
            _requiredString = forceTitle;
            foreach (Process proc in Process.GetProcessesByName(Application.ProductName))
            {
                if (proc.Id != Process.GetCurrentProcess().Id)
                {
                    NativeMethods.EnumWindows(new EnumWindowsProcDel(EnumWindowsProc),
                        proc.Id);
                    return false;
                }
            }
            return true;
        }
    }
}
