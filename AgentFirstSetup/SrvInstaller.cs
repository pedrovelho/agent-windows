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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;

namespace AgentFirstSetup
{
    /// <summary>
    /// Summary description for ServiceInstaller.
    /// </summary>
    class SrvInstaller
    {

        #region DLLImport
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
        int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
        string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DeleteService(IntPtr SVHANDLE);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLastError();
        #endregion DLLImport

        /// <summary>
        /// This method installs and runs the service in the service control manager.
        /// </summary>
        /// <param name="svcPath">The complete path of the service.</param>
        /// <param name="svcName">Name of the service.</param>
        /// <param name="svcDispName">Display name of the service.</param>
        /// <returns>True if the process went thro successfully. False if there was anyerror.</returns>      
        [STAThread]
        public static bool Install(string svcPath, string svcName, string svcDispName, string domain, string name, string password)
        {
            #region Constants declaration.
            const int SC_MANAGER_CREATE_SERVICE = 0x0002;
            const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            //int SERVICE_DEMAND_START = 0x00000003;
            const int SERVICE_ERROR_NORMAL = 0x00000001;
            const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            const int SERVICE_QUERY_CONFIG = 0x0001;
            const int SERVICE_CHANGE_CONFIG = 0x0002;
            const int SERVICE_QUERY_STATUS = 0x0004;
            const int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            const int SERVICE_START = 0x0010;
            const int SERVICE_STOP = 0x0020;
            const int SERVICE_PAUSE_CONTINUE = 0x0040;
            const int SERVICE_INTERROGATE = 0x0080;
            const int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            const int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            SERVICE_QUERY_CONFIG |
            SERVICE_CHANGE_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_ENUMERATE_DEPENDENTS |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_INTERROGATE |
            SERVICE_USER_DEFINED_CONTROL);
            const int SERVICE_AUTO_START = 0x00000002;
            #endregion Constants declaration.

            // Used to close the handle and check the failure  
            IntPtr sc_handle;

            // 1) Open the service control manager database            
            try
            {
                sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                // Handle failure
                if (sc_handle == null || sc_handle.ToInt32() == 0)
                {
                    throw new Win32Exception();
                }
            }
            catch (Exception ex)
            {
                // Cannot continue show error message box and exit from this method
                MessageBox.Show("Could not open the service control manager database. Error code: " + Marshal.GetLastWin32Error() + "\n" + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // 2) Try to create a service
            try
            {
                string serviceStartName = domain + "\\" + name;
                string serviceStartPassword = password;

                // If empty account name is provided then the service will be logged on as LocalSystem account with an empty string as password
                if (name == null || name.Equals(""))
                {
                    serviceStartName = null;
                    serviceStartPassword = "";
                }

                // If the provided password is null then set it to an empty string (in case of an account without password)
                if (password == null)
                {
                    serviceStartPassword = "";
                }

                // Create the service
                IntPtr sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, serviceStartName, serviceStartPassword);

                // Even if there was a failure close the handle to the service control manager
                CloseServiceHandle(sc_handle);

                // Handle service creation failure
                if (sv_handle == null || sv_handle.ToInt32() == 0)
                {
                    throw new Win32Exception();
                }
            }
            catch (Exception ex)
            {
                // Cannot continue show error message box and exit from this method
                MessageBox.Show("Could not create the ProActive Agent service. Error code: " + Marshal.GetLastWin32Error() + "\n" + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method uninstalls the service from the service conrol manager.
        /// </summary>
        /// <param name="svcName">Name of the service to uninstall.</param>
        [STAThread]
        public static bool UnInstallService(string svcName)
        {
            const int GENERIC_WRITE = 0x40000000;
            IntPtr sc_handle;
            // 1) Open the service control manager database  
            try
            {
                sc_handle = OpenSCManager(null, null, GENERIC_WRITE);
                // Handle failure
                if (sc_handle == null || sc_handle.ToInt32() == 0)
                {
                    throw new Win32Exception();
                }
            }
            catch (Exception ex)
            {
                // Cannot continue show error message box and exit from this method
                MessageBox.Show("Could not open the service control manager database. Error code: " + Marshal.GetLastWin32Error() + "\n" + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // 2) Try to delete the service
            try
            {
                const int DELETE = 0x10000;
                IntPtr sv_handle = OpenService(sc_handle, svcName, DELETE);
                // Check for service open failure
                if (sv_handle == null || sv_handle.ToInt32() == 0)
                {
                    // If the service does not exists just skip the service deletion
                    CloseServiceHandle(sc_handle);
                    return false;
                }
                int i = DeleteService(sv_handle);
                CloseServiceHandle(sc_handle);
                // Check for service deletion failure
                if (i == 0)
                {
                    throw new Win32Exception();
                }
            }
            catch (Exception ex)
            {
                // Cannot continue show error message box and exit from this method
                MessageBox.Show("Could not delete the ProActive Agent service. Error code: " + Marshal.GetLastWin32Error() + "\n" + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
