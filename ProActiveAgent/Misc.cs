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
using System.Text;

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
        public const string PROACTIVE_AGENT_SERVICE_NAME = "ProActiveAgent";
        /// <summary>
        /// The name of the ProActive Agent executable</summary>
        public const string PROACTIVE_AGENT_EXECUTABLE_NAME = "ProActiveAgent.exe";
        /// <summary>
        /// The name of the windows event logging system used by the ProActive Agent.
        /// This can be Application, System, Security, or cusotm log name.</summary>
        public const string PROACTIVE_AGENT_WINDOWS_EVENTLOG_LOG = "Application";
        /// <summary>
        /// The default install dir location of the ProActive Agent.</summary>
        public const string PROACTIVE_AGENT_DEFAULT_INSTALL_LOCATION = "C:\\Program Files\\ProActiveAgent";
        /// <summary>
        /// The default config location of the ProActive Agent.</summary>
        public const string PROACTIVE_AGENT_DEFAULT_CONFIG_LOCATION = PROACTIVE_AGENT_DEFAULT_INSTALL_LOCATION + "\\PAAgent-config.xml";
        /// <summary>
        /// The windows registry subkey used by ProActive Agent.</summary>
        public const string PROACTIVE_AGENT_REG_SUBKEY = "Software\\ProActiveAgent";
        /// <summary>
        /// The name of the reg value used for install location.</summary>
        public const string PROACTIVE_AGENT_INSTALL_LOCATION_REG_VALUE_NAME = "AgentLocation";
        /// <summary>
        /// The name of the reg value used for config location.</summary>
        public const string PROACTIVE_AGENT_CONFIG_LOCATION_REG_VALUE_NAME = "ConfigLocation";
        /// <summary>
        /// The name of the reg value used for config location.</summary>
        public const string PROACTIVE_AGENT_IS_RUNNING_EXECUTOR_REG_VALUE_NAME = "IsRunning";
        /// <summary>
        /// The name of the reg value used for the service user.</summary>
        public const string PROACTIVE_AGENT_SERVICE_USER_REG_VALUE_NAME = "ServiceUser";
        /// <summary>
        /// The windows registry subkey used for storing executors status.</summary>
        public const string PROACTIVE_AGENT_EXECUTORS_REG_SUBKEY = "Software\\ProActiveAgent\\Executors";
        /// <summary>
        /// This timeout can be usefull in case of service re-installation to avoid "1072 - Service marked for deletion problem".</summary>
        public const int PROACTIVE_AGENT_POST_UNINSTALL_SERVICE_TIMEOUT = 3;
        /// <summary>
        /// A boolean flag to allow memory limitation per job (set of processes).</summary>
        public const bool ALLOW_PROCESS_MEMORY_LIMIT = true;
        /// <summary>
        /// The minimal requried memory of a jvm.</summary>
        public const uint MINIMAL_REQUIRED_MEMORY = 96; // 64 for initial heap size + 32 jvm internals
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
        /// The name of the ProActive Communication Protocol java property.</summary>
        public const string PROACTIVE_COMMUNICATION_PROTOCOL_JAVA_PROPERTY = "-Dproactive.communication.protocol";
        /// <summary>
        /// The name of the ProActive Rmi Port java property.</summary>
        public const string PROACTIVE_RMI_PORT_JAVA_PROPERTY = "-Dproactive.rmi.port";
        /// <summary>
        /// The name of the ProActive Http Port java property.</summary>
        public const string PROACTIVE_HTTP_PORT_JAVA_PROPERTY = "-Dproactive.http.port";
        /// <summary>
        /// The maximum value allowed for the proactive rmi port.</summary>
        public const int MAX_PROACTIVE_RMI_PORT = 65534;
        /// <summary>
        /// The link to the official documentation.</summary>
        public const string DOC_LINK = "http://proactive.inria.fr/release-doc/ResourceManager/single_html/ResourceManagerManual.html#ProActiveWindowsAgent_89";
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
        public static void readClasspath(ConfigParser.AgentConfig config)
        {
            if (config.proactiveLocation == null || config.proactiveLocation.Equals(""))
            {
                throw new ApplicationException("Unable to read the classpath, the ProActive location is unknown!");
            }

            string binDirectory = config.proactiveLocation + @"\bin";

            // First check if the dir 'bin' exists
            if (!System.IO.Directory.Exists(binDirectory))
            {
                // If the 'bin' directory does not exists throw an exception
                throw new ApplicationException("Unable to read the classpath, invalid ProActive location! " + binDirectory);
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
            info.EnvironmentVariables["PA_SCHEDULER"] = config.proactiveLocation;
            info.EnvironmentVariables["PROACTIVE"] = config.proactiveLocation;

            // 1) Use the java location if it's specified in the configuration
            // 2) If not specified use JAVA_HOME variable
            // 3) If JAVA_HOME is not defined or empty throw exception

            if (config.javaHome == null || config.javaHome.Equals(""))
            {
                string envJavaHome = System.Environment.GetEnvironmentVariable(Constants.JAVA_HOME);

                if (envJavaHome == null || envJavaHome.Equals(""))
                {
                    throw new ApplicationException("Unable to read the classpath, please specify the java location in the configuration or set JAVA_HOME environement variable.");
                }
                else
                {
                    // Fill classpath in the configuration using the JAVA_HOME variable defined in the parent environement
                    config.classpath = VariableEchoer.echoVariable(initScript, Constants.CLASSPATH, info);
                }
            }
            else
            {
                // Use configuration specific java location
                info.EnvironmentVariables[Constants.JAVA_HOME] = config.javaHome;
                // Fill classpath in the configuration
                config.classpath = VariableEchoer.echoVariable(initScript, Constants.CLASSPATH, info);
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
    }

    public static class JavaNetworkInterfaceLister
    {

        public static string[] listJavaNetworkInterfaces(string javaLocation, string agentLocation)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            // Prepare to create a process that will run the java class
            info.FileName = javaLocation + "\\bin\\java.exe";
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
}
