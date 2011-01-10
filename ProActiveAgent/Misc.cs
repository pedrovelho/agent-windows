/*
 * ################################################################
 *
 * ProActive Parallel Suite(TM): The Java(TM) library for
 *    Parallel, Distributed, Multi-Core Computing for
 *    Enterprise Grids & Clouds
 *
 * Copyright (C) 1997-2011 INRIA/University of
 *                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 *
 * If needed, contact us to obtain a release under GPL Version 2 or 3
 * or a different license than the AGPL.
 *
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################
 * $$ACTIVEEON_INITIAL_DEV$$
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ProActiveAgent
{
    /// <summary>
    /// Class used for constants definition</summary>
    /// <remarks>
    /// Some of these constants may be used by other projects. The entire solution may need to be regenerated in case of modification.</remarks>
    public static class Constants
    {
        /// <summary>
        /// The name of the ProActive Agent service</summary>
        public const string SERVICE_NAME = "ProActiveAgent";
        /// <summary>
        /// The default install dir location of the ProActive Agent.</summary>
        public const string DEFAULT_INSTALL_LOCATION = "C:\\Program Files\\ProActiveAgent";
        /// <summary>
        /// The default config location of the ProActive Agent.</summary>
        public const string DEFAULT_CONFIG_LOCATION = DEFAULT_INSTALL_LOCATION + "\\config\\PAAgent-config.xml";
        /// <summary>
        /// The windows registry subkey used by ProActive Agent.</summary>
        public const string REG_SUBKEY = "Software\\ProActiveAgent";
        /// <summary>
        /// The name of the reg value used for install location.</summary>
        public const string INSTALL_LOCATION_REG_VALUE_NAME = "AgentLocation";
        /// <summary>
        /// The name of the reg value used for config location.</summary>
        public const string CONFIG_LOCATION_REG_VALUE_NAME = "ConfigLocation";
        /// <summary>
        /// The name of the reg value used for logs directory.</summary>
        public const string LOGS_DIR_REG_VALUE_NAME = "LogsDirectory";
        /// <summary>
        /// A boolean flag to allow memory limitation per job (set of processes).</summary>
        public const bool ALLOW_PROCESS_MEMORY_LIMIT = true;
        /// <summary>
        /// The name of the job object used for usage limitations.</summary>
        public const string JOB_OBJECT_NAME = "ProActiveAgentJobObject";
        /// <summary>
        /// The name of the classpath variable.</summary>
        public const string CLASSPATH = "CLASSPATH";
        /// <summary>
        /// The name of the classpath variable.</summary>
        public const string JAVA_HOME = "JAVA_HOME";
        /// <summary>
        /// The java.exe location relative to JAVA_HOME.</summary>
        public const string BIN_JAVA = @"\bin\java.exe";
        /// <summary>
        /// The name of the ProActive Communication Protocol java property.</summary>
        public const string PROACTIVE_COMMUNICATION_PROTOCOL_JAVA_PROPERTY = "-Dproactive.communication.protocol";
        /// <summary>
        /// The name of the ProActive Agent rank java property.</summary>
        public const string PROACTIVE_AGENT_RANK_JAVA_PROPERTY = "-Dproactive.agent.rank";
        /// <summary>
        /// The maximum value allowed for the proactive rmi port.</summary>
        public const int MAX_PROACTIVE_RMI_PORT = 65534;
        /// <summary>
        /// The link to the official documentation.</summary>
        public const string DOC_LINK = "http://proactive.inria.fr/release-doc/Resourcing/multiple_html/ProActiveWindowsAgent_89.html";
        /// <summary>
        /// The default Resource Manager url.</summary>
        public const string DEFAULT_RM_URL = "rmi://localhost:1099";
        /// <summary>
        /// The name of the pipe used by the ProActive agent to communicate with the gui.</summary>
        public const string PIPE_NAME = "ProActiveAgentPipe";
    }

    /// <summary>
    /// Enumeration type of ProActive Agent Service custom commands.
    /// There are following commands:
    ///    Start - trigger action
    ///    Stop  - stops triggered action or do nothing when it has not been triggered
    /// </summary>
    public enum PAACommands
    {
        ScreenSaverStart = 128,
        ScreenSaverStop = 129
    }

    /// <summary>
    /// Enumeration type of ProActive Agent running type.
    /// </summary>
    public enum ApplicationType
    {
        AgentScheduler = 1,
        AgentScreensaver = 2
    }

    /// <summary>
    /// A static class that contains several utilitary methods</summary>
    public static class Utils
    {        
        /// <summary>
        /// Returns a decimal value of the available physical memory in mbytes of this computer.
        /// </summary> 
        /// <returns>The available physical memory of this computer.</returns>
        public static Decimal getAvailablePhysicalMemory()
        {
            Decimal result = 0;
            // THE FOLLOWING DOES NOT WORK ON VISTA :
            //ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_LogicalMemoryConfiguration");
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);
            //foreach (ManagementObject item in searcher.Get())
            //{
            //    String s = item["TotalPhysicalMemory"].ToString();
            //    result = System.Decimal.Ceiling(System.Decimal.Parse(s) / 1024); // convert to Mbytes
            //}
            // This seems more appropriate            
            // result = System.Decimal.Ceiling(new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory / (1024 * 1024)); // from bytes to mbytes
            // Another method
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            result = Convert.ToDecimal(ramCounter.NextValue());
            return result;
        }

        /// <summary>
        /// Reads the value of the CLASSPATH variable defined in a script and stores it into the configuration.
        /// This method checks if the provided ProActive location contains \bin\windows\init.bat script.        
        /// </summary>
        /// <param name="config">The user defined configuration.</param>
        public static void readClasspath(ConfigParser.AgentConfigType config)
        {
            if ("".Equals(config.proactiveHome))
            {
                throw new ApplicationException("Unable to read the classpath, the ProActive location is unknown!");
            }

            string binDirectory = config.proactiveHome + @"\bin";

            // First check if the directory exists
            if (!Directory.Exists(binDirectory))
            {
                // Check for UNC path 
                checkUNC(config.proactiveHome);

                // If here it certainly means that the 'bin' directory simply does not exist
                throw new ApplicationException("Unable to read the classpath, invalid ProActive location " + binDirectory);
            }

            string initScript = binDirectory + @"\init.bat";
            // Check if the 'bin\init.bat' script exists
            if (!System.IO.File.Exists(initScript))
            {
                // If the 'init.bat' script does not exists in the 'bin' directory check in 'bin\windows' directory
                initScript = binDirectory + @"\windows\init.bat";
                if (!System.IO.File.Exists(initScript))
                {
                    throw new ApplicationException("Unable to read the classpath, cannot find the initialization script " + initScript);
                }
            }

            ProcessStartInfo info = new ProcessStartInfo();
            info.EnvironmentVariables["PA_SCHEDULER"] = config.proactiveHome;
            info.EnvironmentVariables["PROACTIVE"] = config.proactiveHome;

            // 1) Use the java location if it's specified in the configuration
            // 2) If not specified use JAVA_HOME variable
            // 3) If JAVA_HOME is not defined or empty throw exception

            if (config.javaHome == null || config.javaHome.Equals(""))
            {
                // The classpath will be filled using the JAVA_HOME variable defined in the parent environement
                string envJavaHome = System.Environment.GetEnvironmentVariable(Constants.JAVA_HOME);

                if (envJavaHome == null || envJavaHome.Equals(""))
                {
                    throw new ApplicationException("Unable to read the classpath, please specify the java location in the configuration or set JAVA_HOME environement variable.");
                }
            }
            else
            {
                // First check if the directory exists
                if (!Directory.Exists(config.javaHome))
                {
                    // Check for UNC path 
                    checkUNC(config.javaHome);

                    // If here it certainly means that the directory simply does not exist
                    throw new ApplicationException("Unable to read the classpath, invalid java home " + config.javaHome);
                }

                // Use configuration specific java location
                info.EnvironmentVariables[Constants.JAVA_HOME] = config.javaHome;
            }

            // Fill classpath in the configuration
            config.classpath = VariableEchoer.echoVariable(initScript, Constants.CLASSPATH, info);
        }

        private static void checkUNC(string directory)
        {
            // If the Directory.Exists() method return false it can be an access restriction/problem or the
            // directory does not exists. In case of an UNC path (ie remote resource) the following code checks
            // if the proactive location is accessible, the following code can throw an exception if the remote
            // machine has "Password protected sharing turned on" (usually on Vista and 7)

            try
            {
                DirectoryInfo binDirectoryInfo = new DirectoryInfo(directory);
                binDirectoryInfo.GetAccessControl();
            }
            catch (IOException e)
            {
                // Maybe an authentication is required ... 
                throw new ApplicationException("Unable to read the classpath, cannot access the location " + directory + System.Environment.NewLine +
                    "In case of an UNC path it is possible that 'Password protected sharing' is turned on on the remote machine", e);
            }
            catch (Exception e)
            {
                // A problem can occur, for example path too long, etc ...
                throw new ApplicationException("Unable to read the classpath, cannot access the location " + directory, e);
            }
        }

        /// <summary>        
        /// This method checks if a tcp port is available. 
        /// !! DOES NOT CHECK FOR MAX VALUE !!
        /// </summary>
        /// <param name="config">The user defined configuration.</param>
        public static bool isTcpPortAvailable(int port)
        {
            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat -ano | find "port_num" command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will return false
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            // Search through active tcp listeners            
            foreach (IPEndPoint tcpl in ipGlobalProperties.GetActiveTcpListeners())
            {
                if (tcpl.Port == port)
                {

                    return false;
                }
            }
            // Search through active tcp connections
            foreach (TcpConnectionInformation tcpi in ipGlobalProperties.GetActiveTcpConnections())
            {
                if (tcpi.LocalEndPoint.Port == port)
                {

                    return false;
                }
            }

            return true;
        }

        public static string[] listJavaNetworkInterfaces(string javaLocation, string agentLocation)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            // Prepare to create a process that will run the java class
            info.FileName = javaLocation + Constants.BIN_JAVA;
            // Add the agent location as classpath and the name of the java class to execute
            info.Arguments = "-cp \"" + agentLocation + "\" ListNetworkInterfaces";
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;

            // Create new process 
            Process p = new Process();
            p.StartInfo = info;

            try
            {
                // Start the process
                if (!p.Start())
                {
                    throw new ApplicationException("Unable to start the process " + info.FileName);
                }

                StreamReader myStreamReader = p.StandardOutput;
                List<string> ar = new List<string>();
                string line = myStreamReader.ReadLine();
                while (line != null && !line.Equals(""))
                {
                    ar.Add(line);
                    // Read the standard output of the spawned process.
                    line = myStreamReader.ReadLine();
                }
                p.Close();

                return ar.ToArray();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to list java network interfaces ", e);
            }
        }
    }

    /// <summary>
    /// A static class that spawns a process in order to echo a variable defined in the specified script.</summary>
    public static class VariableEchoer
    {
        private static readonly string PROMPT = System.IO.Directory.GetCurrentDirectory() + ">";

        private static StringBuilder initializerOutput;

        public static String echoVariable(string scriptFilename, string variableToEcho, ProcessStartInfo info)
        {
            // Prepare to create a process that will run the script            
            info.FileName = "cmd.exe";
            // /V:ON is equivalent to setlocal enabledelayedexpansion
            info.Arguments = "/V:ON /K \"" + scriptFilename + "\"";
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;

            // Create new process 
            Process p = new Process();
            p.StartInfo = info;

            // Create output buffer
            VariableEchoer.initializerOutput = new StringBuilder("");

            // Set our event handler to asynchronously read the output.
            p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);

            try
            {
                // Start the process
                if (!p.Start())
                {
                    throw new ApplicationException("Unable to run the script " + scriptFilename);
                }

                // Use a stream writer to synchronously write the sort input.
                System.IO.StreamWriter streamWriter = p.StandardInput;

                // Start the asynchronous read of the output stream.
                p.BeginOutputReadLine();

                // Write the command that will print the full java command
                streamWriter.WriteLine("echo %" + variableToEcho + "%");

                // End the input stream 
                streamWriter.Close();

                // Wait for the process to write the text lines
                p.WaitForExit();

                // Kill the process if its not finished
                if (!p.HasExited)
                {
                    p.Kill();
                }

                return VariableEchoer.initializerOutput.ToString();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to echo the variable " + variableToEcho, e);
            }
            finally
            {
                p.Close();
            }
        }

        private static void OutputHandler(object sendingProcess, System.Diagnostics.DataReceivedEventArgs outLine)
        {
            if (outLine.Data == null || outLine.Data.Equals("") || outLine.Data.Contains(PROMPT))
            {
                return;
            }
            // Add the text to the collected output
            initializerOutput.Append(outLine.Data);
        }
    }

    /// <summary>
    /// A static class that spawns a process in order to execute a specified script.</summary>
    public static class ScriptExecutor
    {
        public static string executeScript(string scriptAbsolutePath, string scriptArguments)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            // Prepare to create a process that will run the specified script
            info.FileName = scriptAbsolutePath;
            info.Arguments = scriptArguments;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;

            // Create new process 
            Process p = new Process();
            p.StartInfo = info;

            try
            {
                // Start the process
                if (!p.Start())
                {
                    throw new ApplicationException("Unable to start the process " + info.FileName);
                }

                StreamReader myStreamReader = p.StandardOutput;
                string output = myStreamReader.ReadToEnd();
                p.Close();
                return output;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to execute the script " + scriptAbsolutePath, e);
            }
        }
    }

    /// <summary>
    /// Check running processes for an already-running instance. Implements a simple and
    /// always effective algorithm to find currently running processes with a main window
    /// matching a given substring and focus it.
    /// Combines code written by Lion Shi (MS) and Sam Allen.
    /// </summary>
    public static class ProcessChecker
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
