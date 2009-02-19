using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public const string PROACTIVE_AGENT_SERVICE_NAME = "ProActive Agent";
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
        public const string PROACTIVE_AGENT_INSTALL_REG_VALUE_NAME = "AgentDirectory";
        /// <summary>
        /// The name of the reg value used for config location.</summary>
        public const string PROACTIVE_AGENT_CONFIG_REG_VALUE_NAME = "ConfigLocation";
        /// <summary>
        /// This timeout can be usefull in case of service re-installation to avoid "1072 - Service marked for deletion problem".</summary>
        public const int PROACTIVE_AGENT_POST_UNINSTALL_SERVICE_TIMEOUT = 3; 
        /// <summary>
        /// A boolean flag to allow memory limitation per job (set of processes).</summary>
        public const bool ALLOW_PROCESS_MEMORY_LIMIT = true;
        /// <summary>
        /// The name of the job object used for usage limitations.</summary>
        public const string JOB_OBJECT_NAME = "ProActiveAgentJobObject";
        /// <summary>
        /// The name of the classpath variable.</summary>
        public const string CLASSPATH_VAR_NAME = "CLASSPATH";
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
            result = System.Decimal.Ceiling(new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory / (1024 * 1024)); // from bytes to mbytes
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

            // Check if the location contains \bin\windows\init.bat
            // in order to get the java command
            string initScript = config.proactiveLocation + @"\bin\windows\init.bat";
            // Second check if the init.bat script exists
            if (!System.IO.File.Exists(initScript))
            {
                throw new ApplicationException("Unable to read the classpath, cannot find the initialization script " + initScript);
            }

            ProcessStartInfo info = new ProcessStartInfo();
            info.EnvironmentVariables["PA_SCHEDULER"] = config.proactiveLocation;

            //// 1) Use the java location if it's specified in the configuration
            //// 2) If not specified use JAVA_HOME variable
            //// 3) If JAVA_HOME is not defined or empty throw exception

            //if (config.javaHome == null || config.javaHome.Equals(""))
            //{
            //    string envJavaHome = System.Environment.GetEnvironmentVariable("JAVA_HOME");

            //    if (envJavaHome == null || envJavaHome.Equals(""))
            //    {
            //        throw new ApplicationException("Unable to build java command, please specify the java location in the configuration file or set an environement JAVA_HOME variable.");
            //    }
            //    else
            //    {
            //        // Get the value of the JAVA_CMD variable defined by the script
            //        config.fullJavaCommand = VariableEchoer.echoVariable(initScript, "JAVA_CMD", info);
            //    }
            //}
            //else
            //{
            //    // Use configuration specific java location
            //    info.EnvironmentVariables["JAVA_HOME"] = config.javaHome;
            //    // Get the value of the JAVA_CMD variable defined by the script
            //    config.fullJavaCommand = VariableEchoer.echoVariable(initScript, "JAVA_CMD", info);
            //}
            // Fill classpath in the configuration
            config.classpath = VariableEchoer.echoVariable(initScript, Constants.CLASSPATH_VAR_NAME, info);
        }
    }

    public static class JavaNetworkInterfaceLister {

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
                    throw new ApplicationException("Could not start the process " + info.FileName);
                }

                StreamReader myStreamReader = p.StandardOutput;
                List<string> ar = new List<string>();
                string line = myStreamReader.ReadLine();
                while (line != null && !line.Equals("")) {                    
                    ar.Add(line);
                    // Read the standard output of the spawned process.
                    line = myStreamReader.ReadLine();
                }                
                p.Close();

                return ar.ToArray();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not list java network interfaces ", e);
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
                    throw new ApplicationException("Could not run the script " + scriptFilename);
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
                throw new ApplicationException("Could not echo the variable " + variableToEcho, e);
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
}
