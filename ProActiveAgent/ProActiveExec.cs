using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ConfigParser;
using JobManagement;
using log4net;
using Microsoft.Win32;

/** Executor of runner script and thus ProActive runtime.
 *  This class implements semantics of all actions available
 */

namespace ProActiveAgent
{
    public class ProActiveExec
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ILog PROACTIVE_RUNTIME_PROCESS_LOGGER = LogManager.GetLogger("ProActiveRuntimeProcessLogger");

        //[DllImport("pkill.dll", EntryPoint = "_KillProcessEx@8", CallingConvention = CallingConvention.Winapi)]
        //private static extern bool KillProcessEx(uint dwProcessId, bool bTree);
        private const int INITIAL_RESTART_DELAY = 3000;
        private const int MAX_RESTART_DELAY = 10000;
        private const uint DEFAULT_JAVA_HEAP_SIZE_IN_MBYTES = 64; // for the -Xms jvm option
        private const uint MINIMAL_REQUIRED_MEMORY_FOR_PROACTIVE_RUNTIME_PROCESS_IN_MBYTES = 32; // for the job memory limitation
        /// <summary>
        /// The timer manager responsible of scheduling timed actions</summary>
        private readonly TimerManager timerManager;
        /// <summary>
        /// The configuration used to run the process.</summary>
        private readonly Configuration configuration;
        /// <summary>
        /// Process object that represents running runner script.</summary>                
        private bool disabledRestarting = false; // restarting of the process is disabled when the system shuts down
        /// <summary>
        /// The initial delay before restart.</summary>
        private int initialRestartDelay;
        /// <summary>
        /// For an application type (i.e AgentScheduler) boolean value = true if this type has sent the "start command". It is set to false when the same app type sends stop command.</summary>
        private readonly Dictionary<ApplicationType, Int32> callersState;
        /// <summary>
        /// The current delay before restart.</summary>
        private int restartDelay;
        /// <summary>
        /// All jvm parameters (default and user defined).</summary>
        private readonly string[] jvmParameters;
        /// <summary>
        /// The starter class name.</summary>
        private readonly string cmd;
        /// <summary>
        /// The arguments used for starter class.</summary>
        private readonly string[] args;
        /// <summary>
        /// Process object that represents running runner script.</summary>
        private Process proActiveRuntimeProcess;
        /// <summary>
        /// The job object used to set the usage limits for the ProActive Runtime process and its child processes.</summary>
        private readonly JobObject jobObject;
        /// <summary>
        /// The maximum java heap size for the ProActive Runtime process.</summary>
        private readonly uint maximumJavaHeapSize;
        /// <summary>
        /// The cpu limiter used to set a max allowed cpu usage for the ProActive Runtime process.</summary>
        private readonly CPULimiter cpuLimiter;

        /// <summary>
        /// Process object that represents running runner script.</summary>                        
        public ProActiveExec(Configuration configuration, ConfigParser.Action action)
        {
            this.timerManager = new TimerManager(this, action);
            this.configuration = configuration;
            this.initialRestartDelay = (action.initialRestartDelay > 0 ? action.initialRestartDelay : INITIAL_RESTART_DELAY);
            this.callersState = new Dictionary<ApplicationType, Int32>();
            this.proActiveRuntimeProcess = null;
            // Create a new job object for limits
            this.jobObject = new JobObject(Constants.JOB_OBJECT_NAME);
            this.jobObject.Events.OnNewProcess += new jobEventHandler<NewProcessEventArgs>(Events_OnNewProcess);
            this.maximumJavaHeapSize = DEFAULT_JAVA_HEAP_SIZE_IN_MBYTES;
            // If memory management is enabled
            if (this.configuration.agentConfig.enableMemoryManagement)
            {
                // Add user defined memory limitations
                this.maximumJavaHeapSize += this.configuration.agentConfig.javaMemory;
                uint memoryLimit = this.maximumJavaHeapSize + this.configuration.agentConfig.nativeMemory + MINIMAL_REQUIRED_MEMORY_FOR_PROACTIVE_RUNTIME_PROCESS_IN_MBYTES;
                LOGGER.Info("A memory limitation of " + memoryLimit + " Mbytes is set for the ProActive Runtime process.");
                this.jobObject.Limits.JobMemoryLimit = new IntPtr(memoryLimit * 1024 * 1024);
                // Add event handlers to keep track of job events
                this.jobObject.Events.OnJobMemoryLimit += new jobEventHandler<JobMemoryLimitEventArgs>(Events_OnJobMemoryLimit);
            }
            // Create new instance of the cpu limiter
            this.cpuLimiter = new CPULimiter();

            // Prepare jvm parameters, cmd and args from the given action type

            // All jvm parameters
            List<string> jvmParametersList = new List<string>();

            // Add default parameters
            ConfigParser.Action.addDefaultJvmParameters(jvmParametersList, this.configuration.agentConfig.proactiveLocation);

            this.cmd = action.javaStarterClass;
            this.args = action.getArgs();

            // user defined jvm parameters will be added after in order to let the user redefine default parameters                        
            if (action is AdvertAction)
            {
                AdvertAction advertAction = (AdvertAction)action;
                // Add action specific default jvm parameters
                // ... nothing to add
                // Add user defined jvm parameters
                this.addUserDefinedJvmParameters(jvmParametersList);
                this.jvmParameters = jvmParametersList.ToArray();                 
            }
            else if (action is RMAction)
            {
                RMAction rmAction = (RMAction)action;
                // Add action specific default jvm parameters
                RMAction.addDefaultJvmParameters(jvmParametersList, this.configuration.agentConfig.proactiveLocation);
                // Add user defined jvm parameters
                this.addUserDefinedJvmParameters(jvmParametersList);
                this.jvmParameters = jvmParametersList.ToArray();                
            }
            else if (action is CustomAction)
            {
                CustomAction customAction = (CustomAction)action;
                // Add action specific default jvm parameters
                // ... nothing to add
                // Add user defined jvm parameters
                this.addUserDefinedJvmParameters(jvmParametersList);
                this.jvmParameters = jvmParametersList.ToArray();                
            }
            else
            {
                // Unknown action
            }
        }

        private void addUserDefinedJvmParameters(List<string> jvmParameters)
        {
            // Add all user defined jvm parameters             
            // Init -Xms and -Xmx jvm paremeters to default values from the configuration
            string xms = "-Xms" + DEFAULT_JAVA_HEAP_SIZE_IN_MBYTES + "M";
            string xmx = "-Xmx" + this.maximumJavaHeapSize + "M";
            // Append all params and check for overriden jvm memory parameters
            foreach (string s in this.configuration.agentConfig.jvmParameters)
            {
                // Check if user specifically defined jvm memory params and override them with user values
                if (s != null && s.Contains("-Xms"))
                {
                    xms = s;
                    continue;
                }
                if (s != null && s.Contains("-Xmx"))
                {
                    xmx = s;
                    continue;
                }
                jvmParameters.Add(s);
            }
            // Log the memory jvm parameters
            LOGGER.Info("The initial java heap size is " + xms + " and maximum java heap size is " + xmx);
            jvmParameters.Add(xms);
            jvmParameters.Add(xmx);
        }

        /// <summary>
        /// The initialization method loads events on the current timer manager.
        /// </summary>
        public void init()
        {
            this.timerManager.loadEvents(this.configuration.events);
        }

        /// <summary>
        /// Resets the restart delay to the initial restart delay.
        /// </summary>
        public void resetRestartDelay()
        {
            this.restartDelay = initialRestartDelay;
        }

        /// <summary>
        /// Creates a new ProActive Runtime process from the builded java command and starts it with given [command] argument
        /// and optionally other arguments (args parameter)
        /// this method has to be synchronized as it is dealing with a process object.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool start()
        {
            if (this.disabledRestarting || this.proActiveRuntimeProcess != null)
            {
                return false;
            }

            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Preparing to start new ProActive Runtime process with cmd : " + cmd + " args : " + args);
            }

            ProcessStartInfo info = new ProcessStartInfo();
            try
            {
                // Merge all jvm parameters
                StringBuilder jvmParametersBuilder = new StringBuilder();
                foreach (string parameter in this.jvmParameters)
                {
                    jvmParametersBuilder.Append(" " + parameter);
                }

                // Merge all arguments
                StringBuilder argumentsBuilder = new StringBuilder();
                foreach (string arg in this.args)
                {
                    argumentsBuilder.Append(" " + arg);
                }

                // Create a new process
                this.proActiveRuntimeProcess = new Process();

                // Use process info to specify all options               
                // Application filename is java executable with full path
                info.FileName = this.configuration.agentConfig.javaHome + "\\bin\\java.exe";
                // Application arguments will be 
                info.Arguments = jvmParametersBuilder.ToString() + " " + this.cmd + " " + argumentsBuilder.ToString();
                // Set the classpath 
                info.EnvironmentVariables[Constants.CLASSPATH_VAR_NAME] = this.configuration.agentConfig.classpath;
                // Configure runtime specifics
                info.UseShellExecute = false; // needed to redirect output
                info.CreateNoWindow = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;                
                // Set the process start info 
                this.proActiveRuntimeProcess.StartInfo = info;

                // We attach a handler in order to intercept killing of that process
                // Therefore, we will be able to relaunch script in that event
                this.proActiveRuntimeProcess.EnableRaisingEvents = true;
                this.proActiveRuntimeProcess.Exited += Events_OnProActiveRuntimeProcessExit;
                this.proActiveRuntimeProcess.ErrorDataReceived += Events_OnProActiveRuntimeProcessErrorDataReceived;
                this.proActiveRuntimeProcess.OutputDataReceived += Events_OnProActiveRuntimeProcessOutputDataReceived;

                if (!this.proActiveRuntimeProcess.Start())
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(errorCode);
                }
                LOGGER.Info("Started ProActive Runtime process [pid:" + this.proActiveRuntimeProcess.Id + "]" + System.Environment.NewLine +
                            "Command-line: " + info.FileName + " " + info.Arguments);
                //if (LOGGER.IsDebugEnabled) {
                //    LOGGER.Debug(Constants.CLASSPATH_VAR_NAME + ": " + info.EnvironmentVariables[Constants.CLASSPATH_VAR_NAME]);
                //}
            }
            catch (Exception ex)
            {
                LOGGER.Error("Could not start the ProActive Runtime process! Command-line: " + info.FileName + " " + info.Arguments, ex);
                return false;
            }

            // Assign the process to the job for limitations
            try
            {
                this.jobObject.AssignProcessToJob(this.proActiveRuntimeProcess);
            }
            catch (Exception ex)
            {
                int errorCode = Marshal.GetLastWin32Error();
                Win32Exception internalWin32Exception = new Win32Exception(errorCode);
                LOGGER.Error("Could not assign process to job, error code: " + errorCode + " " + internalWin32Exception.ToString(), ex);
                return false;
            }

            // notify process about asynchronous reads
            this.proActiveRuntimeProcess.BeginErrorReadLine();
            this.proActiveRuntimeProcess.BeginOutputReadLine();

            //-- runtime started = true
            setRegistryIsRuntimeStarted(true);

            return true;
        }

        // fires whenever errors output is produced
        private static void Events_OnProActiveRuntimeProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Length == 0)
            {
                return;
            }
            try
            {
                PROACTIVE_RUNTIME_PROCESS_LOGGER.Info(e.Data);
            }
            catch (Exception ex)
            {
                LOGGER.Error("Error occurred while trying to log the ProActive Runtime process stderr.", ex);
            }
        }

        // fires whenever standard output is produced
        private static void Events_OnProActiveRuntimeProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Length == 0)
            {
                return;
            }
            try
            {
                PROACTIVE_RUNTIME_PROCESS_LOGGER.Info(e.Data);
            }
            catch (Exception ex)
            {
                LOGGER.Error("Error occurred while trying to log the ProActive Runtime process stdout.", ex);
            }
        }

        /// <summary>
        /// The event fired when a job reaches its memory limit.
        /// If the incriminated process is the ProActive Runtime process then it's stopped permanently.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="args">The arguments of this event</param>
        void Events_OnJobMemoryLimit(object sender, JobMemoryLimitEventArgs args)
        {
            Process incriminatedProcess = args.TheProcess;
            // If the incriminated process is the ProActiveRuntime process then stop it permanently
            if (this.proActiveRuntimeProcess.Id == incriminatedProcess.Id)
            {
                LOGGER.Info("The ProActive Runtime process [pid:" + this.proActiveRuntimeProcess.Id + "] reached the memory limit and will be killed.");
                this.stop();
                return;
            }
            // Log info about the incriminated process            
            LOGGER.Info("The process " + incriminatedProcess.ProcessName + " [pid:" + incriminatedProcess.Id + "] reached the memory limit and will be killed.");
            // Kill the incriminated process that reached the job memory limit
            try
            {
                if (!incriminatedProcess.HasExited)
                {
                    incriminatedProcess.Kill();
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("Could not kill the incriminated process [pid:" + args.TheProcessId + "].", ex);
            }
        }

        /// <summary>
        /// The event fired when a job detects a new process.
        /// This method logs the name and the pid of the new process.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="args">The arguments of this event</param>
        void Events_OnNewProcess(object sender, NewProcessEventArgs args)
        {
            try
            {
                Process incriminatedProcess = args.TheProcess;
                if (this.proActiveRuntimeProcess.Id != incriminatedProcess.Id)
                {
                    // Log info about the incriminated process            
                    LOGGER.Info("A new process " + incriminatedProcess.ProcessName + " [pid:" + incriminatedProcess.Id + "] has been detected.");
                }
                LOGGER.Info("Adding new process " + incriminatedProcess.ProcessName + " [pid:" + incriminatedProcess.Id + "] to the cpu limiter.");
                // add the process to the cpu limiter
                this.cpuLimiter.addProcessToWatchList(incriminatedProcess);
            }
            catch (Exception ex)
            {
                LOGGER.Info("An error occured in Events_OnNewProcess incrminatedProcess " + args.TheProcess, ex);
            }
        }

        /// <summary>
        /// The event fired when the ProActive Runtime process has exited all
        /// forked processes will be killed.
        /// </summary>
        /// <param name="o">The sender object</param>
        /// <param name="e">The arguments of this event</param>
        [MethodImpl(MethodImplOptions.Synchronized)] // to avoid concurrent problems with stop method
        private void Events_OnProActiveRuntimeProcessExit(object o, EventArgs e)
        {
            LOGGER.Info("The ProActive Runtime process has exited for unknown reason.");

            Thread.Sleep(1000);

            // remove listeners and kill all forked processes
            this.internalClean();

            //this registry shows that the runtime is not running:
            setRegistryIsRuntimeStarted(false);

            if (!disabledRestarting)
            {
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("Preparing to restart the ProActive Runtime process.");
                }

                // if we use timer based config then we will use binary expotential backoff retry
                // in other case we restart process immediately

                // we only perform delayed restart when action originated from scheduled calendar event
                bool delayRestart = callersState.ContainsKey(ApplicationType.AgentScheduler) && (callersState[ApplicationType.AgentScheduler]) > 0;

                // Set current process to null
                this.proActiveRuntimeProcess = null;

                if (delayRestart)
                {
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Restarting the ProActive Runtime process in " + restartDelay + " ms.");
                    }
                    timerManager.addDelayedRetry(restartDelay);
                }
                else
                {
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Restarting the ProActive Runtime process immediately !");
                    }
                    this.start();
                }
            }
        }

        public static void setRegistryIsRuntimeStarted(bool value)
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY, true);
            if (confKey != null)
            {
                confKey.SetValue("IsRuntimeStarted", value);
            }
            confKey.Close();
        }

        // this method has to be synchronized as it is dealing with a process object

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void disableRestarting()
        {
            disabledRestarting = true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void stop()
        {
            if (this.proActiveRuntimeProcess == null)
            {
                return;
            }
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Stopping the ProActive Runtime process.");
            }

            if (!this.proActiveRuntimeProcess.HasExited)
            {
                // Remove event listeners of the job object
                this.jobObject.Events.OnNewProcess -= Events_OnNewProcess;
                if (this.configuration.agentConfig.enableMemoryManagement)
                {
                    this.jobObject.Events.OnJobMemoryLimit -= Events_OnJobMemoryLimit;
                }

                // clean listeners and kill all forked processes
                this.internalClean();
            }
            this.proActiveRuntimeProcess = null;
            //-- runtime started = false
            setRegistryIsRuntimeStarted(false);
        }

        // !!WARNING!! This method removes all listeners and kills all processes in the job object
        // use it with caution, only inside a OnExit event or stop
        private void internalClean() {
            // Remove event listeners of the process
            this.proActiveRuntimeProcess.Exited -= Events_OnProActiveRuntimeProcessExit;
            this.proActiveRuntimeProcess.ErrorDataReceived -= Events_OnProActiveRuntimeProcessErrorDataReceived;
            this.proActiveRuntimeProcess.OutputDataReceived -= Events_OnProActiveRuntimeProcessOutputDataReceived;

            // Clear the watch list of the cpu limiter
            this.cpuLimiter.clearWatchList();

            // Use the job object to kill the ProActive Runtime process and its forked processes
            this.jobObject.TerminateAllProcesses(42); // this exit code was used in the examples of the JobObjectWrapper API
        }

        // called from other parts of ProActive Agent
        // this method dispatches action depending on type
        // and calls internal methods

        public void sendStartAction(ApplicationType appType)
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Received start action request from " + appType.ToString());
            }
            if (callersState.ContainsKey(appType))
            {
                callersState[appType]++;
            }
            else
            {
                callersState.Add(appType, 1);
            }
            
            // Invoke start
            this.start();
        }

        // called when the process is to be restarted after given amount of time
        public void sendRestartAction()
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Received start action request from AgentScheduler");
            }
            restartDelay <<= 1;
            if (restartDelay > MAX_RESTART_DELAY)
            {
                restartDelay = MAX_RESTART_DELAY;
            }
            this.start();
        }

        public void sendStopAction(ApplicationType appType)
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Received stop action request from " + appType.ToString());
            }
            if (!callersState.ContainsKey(appType))
            {
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("No caller for this appType");
                }
                // there were no previous actions from this application type (or we deleted them)
                return;
            }

            if (callersState[appType] == 0)
            {
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("This app type didn't start the action so it doesn't have the right to stop it.");
                }
                // this app type didn't start the action so it doesn't have the right to stop it
                return;
            }
            // change state
            callersState[appType]--;
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("callersState -- " + appType.ToString());
            }

            foreach (KeyValuePair<ApplicationType, Int32> app in callersState)
            {
                if (app.Value > 0)
                {
                    // someone else sent the start command too
                    return;
                }
            }
            this.stop();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void dispose()
        {
            // Delete everything from the state
            callersState.Clear();
            // Stop the current process
            stop();
            // Dispose the timer manger
            this.timerManager.dispose();
            // Dispose the job object
            this.jobObject.Dispose();           
        }

        /// <summary>
        /// Sets the priority and max cpu usage to the ProActive Runtime process and its children processes (forked processes).
        /// The ProActive Runtime process must be started.
        /// </summary>
        /// <param name="processPriority">The ProActive Runtime process priority</param>
        /// <param name="maxCpuUsage">The maximum cpu usage</param>
        public void setProcessBehaviour(ProcessPriorityClass processPriority, uint maxCpuUsage)
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Setting process priority to " + processPriority + " and max cpu usage to " + maxCpuUsage + "%");
            }
            // Apply the specified process priority to all processes inside the job
            foreach (Process process in this.jobObject.ConstructAssignedProcessList())
            {
                if (!process.HasExited)
                {
                    process.PriorityClass = processPriority;
                }
            }
            this.cpuLimiter.setNewMaxCpuUsage(maxCpuUsage);
        }
    }
}
