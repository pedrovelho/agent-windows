using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using JobManagement;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Win32;

/**
 * Executor of the ProActive Runtime process. If the executed process exits, the executor handles the restart with a Timer.
 */
namespace ProActiveAgent
{
    public class ProActiveRuntimeExecutor
    {
        private const int INITIAL_RESTART_DELAY_IN_MS = 5000;
        private const int MAX_RESTART_DELAY_IN_MS = 25000;
        private const int RESTART_DELAY_INCREMENT_IN_MS = 5000;

        /// <summary>
        /// A lock shared between multiple instances of executors.</summary>
        private static readonly object interExecutorLock = new object();

        /// <summary>
        /// The current ProActive Rmi Port that will be initialized by the first executor, then it cycles incrementally until max value.</summary>
        private static int currentProActiveRmiPort;

        /// <summary>
        /// The unique rank of this executor</summary>
        private readonly int rank;
        /// <summary>
        /// The logger of this class, logs all info about this executor.</summary>
        private readonly ILog LOGGER;
        /// <summary>
        /// The logger used to append the logs of the ProActive Runtime Process.</summary>
        private readonly ILog processLogger;
        /// <summary>
        /// Process object that represents running runner script.</summary>                
        private bool disabledRestarting = false; // restarting of the process is disabled when the system shuts down
        /// <summary>
        /// The delay before restart in ms.</summary>
        private long restartDelayInMs;
        /// <summary>
        /// The restart barrier date time, after this point now restart can occur.</summary>
        public DateTime restartBarrierDateTime;
        /// <summary>
        /// A timer used to delay the restart of the ProActive Runtime process.</summary>
        private readonly System.Threading.Timer restartTimer;
        /// <summary>
        /// For an application type (i.e AgentScheduler) boolean value = true if this type has sent the "start command". It is set to false when the same app type sends stop command.</summary>
        private readonly Dictionary<ApplicationType, Int32> callersState;
        /// <summary>
        /// Process object that represents running runner script.</summary>
        private Process proActiveRuntimeProcess;
        /// <summary>
        /// The job object used to set the usage limits for the ProActive Runtime process and its child processes.</summary>
        private readonly JobObject jobObject;
        /// <summary>
        /// The cpu limiter used to set a max allowed cpu usage for the ProActive Runtime process.</summary>
        private readonly CPULimiter cpuLimiter;
        /// <summary>
        /// The common start information shared between all executors.</summary>
        private readonly CommonStartInfo commonStartInfo;

        /// <summary>
        /// Process object that represents running runner script.</summary>                        
        public ProActiveRuntimeExecutor(CommonStartInfo commonStartInfo, int rank)
        {
            this.rank = rank;
            // The first executor initializes the current value of the ProActive Rmi Port
            if (this.rank == 0)
            {
                // Get the initial ProActive Rmi Port specified by the configuration
                currentProActiveRmiPort = commonStartInfo.configuration.agentConfig.proActiveRmiPortInitialValue;
            }
            this.LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType + "" + this.rank);
            // The logger needs to be customized programmatically to log stout/stderr into a separate file
            this.processLogger = LogManager.GetLogger("Executor" + this.rank + "ProcessLogger");
            Logger customLogger = this.processLogger.Logger as log4net.Repository.Hierarchy.Logger;
            if (customLogger != null)
            {
                customLogger.Additivity = false;
                customLogger.AddAppender(CreateRollingFileAppender("Executor" + this.rank + "RollingFileAppender", "Executor" + this.rank + "Process-log.txt"));
            }
            this.commonStartInfo = commonStartInfo;
            // The restart timer is only created it will not start until Timer.Change() method is called
            this.restartTimer = new Timer(new TimerCallback(internalRestart), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            this.callersState = new Dictionary<ApplicationType, Int32>();
            this.proActiveRuntimeProcess = null;
            // Create a new job object for limits
            this.jobObject = new JobObject(Constants.JOB_OBJECT_NAME + this.rank);
            this.jobObject.Events.OnNewProcess += new jobEventHandler<NewProcessEventArgs>(Events_OnNewProcess);
            // If memory management is enabled
            if (this.commonStartInfo.configuration.agentConfig.enableMemoryManagement)
            {
                // Add user defined memory limitations
                this.jobObject.Limits.JobMemoryLimit = new IntPtr(this.commonStartInfo.memoryLimit * 1024 * 1024);
                LOGGER.Info("A memory limitation of " + this.commonStartInfo.memoryLimit + " Mbytes is set for the ProActive Runtime process");
                // Add event handlers to keep track of job events
                this.jobObject.Events.OnJobMemoryLimit += new jobEventHandler<JobMemoryLimitEventArgs>(Events_OnJobMemoryLimit);
            }
            // Create new instance of the cpu limiter
            this.cpuLimiter = new CPULimiter();
        }

        /// <summary>
        /// Creates a new ProActive Runtime process from the java command and starts it with given [command] argument
        /// and optionally other arguments this method has to be synchronized as it is dealing with a process object.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool start()
        {
            if (this.disabledRestarting || this.proActiveRuntimeProcess != null)
            {
                return false;
            }

            // Before starting a ProActive Runtime process an available ProActive Rmi Port is needed.
            // In rare cases an external process can take the port between the test of its availablity and its occupation by the ProActive Runtime process
            // in that case the runtime will not start.

            // Lock all executors in this section
            lock (interExecutorLock)
            {
                // If the maximum is reached then re-cycle to the initial value
                if (currentProActiveRmiPort >= Constants.MAX_PROACTIVE_RMI_PORT)
                {
                    currentProActiveRmiPort = this.commonStartInfo.configuration.agentConfig.proActiveRmiPortInitialValue;
                }
            }
            int proActiveRmiPort = currentProActiveRmiPort;


            // Check the port availability and if it's not available increment and retry until max value
            while (!Utils.isTcpPortAvailable(proActiveRmiPort))
            {
                // This avoids cycling infinitely if the whole intervall is occupied
                if (++proActiveRmiPort >= Constants.MAX_PROACTIVE_RMI_PORT)
                {
                    // If the maximum is reached then exit from this method
                    LOGGER.Error("Could not start the ProActive Runtime process, unable to find an available ProActive Rmi Port");
                    return false;
                }
            }

            // Lock all executors here
            lock (interExecutorLock)
            {
                currentProActiveRmiPort = proActiveRmiPort + 1;
            }

            ProcessStartInfo info = new ProcessStartInfo();
            try
            {
                // Merge all jvm parameters
                StringBuilder jvmParametersBuilder = new StringBuilder();
                foreach (string parameter in this.commonStartInfo.jvmParameters)
                {
                    jvmParametersBuilder.Append(" " + parameter);
                }
                jvmParametersBuilder.Append(" ");
                // Add the property to force the ProActive Runtime to use the port
                jvmParametersBuilder.Append(Constants.PROACTIVE_RMI_PORT_JAVA_PROPERTY + "=" + proActiveRmiPort);

                // Merge all arguments
                StringBuilder argumentsBuilder = new StringBuilder();
                foreach (string arg in this.commonStartInfo.selectedAction.getArgs(this.rank))
                {
                    argumentsBuilder.Append(" " + arg);
                }

                // Create a new process
                this.proActiveRuntimeProcess = new Process();

                // Use process info to specify all options               
                // Application filename is java executable with full path

                // Check for java home 
                string javaHome = this.commonStartInfo.configuration.agentConfig.javaHome;
                if (javaHome == null || javaHome.Equals("")) {
                    javaHome = System.Environment.GetEnvironmentVariable("JAVA_HOME");
                    if (javaHome == null || javaHome.Equals(""))
                    {
                        throw new ApplicationException("Cannot locate java. Please specify the java location in the configuration or set JAVA_HOME environement variable.");
                    }
                }
                info.FileName = javaHome + "\\bin\\java.exe";
                // Application arguments will be 
                info.Arguments = jvmParametersBuilder.ToString() + " " + this.commonStartInfo.cmd + " " + argumentsBuilder.ToString();
                // Set the classpath 
                info.EnvironmentVariables[Constants.CLASSPATH_VAR_NAME] = this.commonStartInfo.configuration.agentConfig.classpath;
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
                            "CLASSPATH=" + info.EnvironmentVariables[Constants.CLASSPATH_VAR_NAME] + System.Environment.NewLine +
                            "Command-line: " + info.FileName + " " + info.Arguments);
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
        private void Events_OnProActiveRuntimeProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Length == 0)
            {
                return;
            }
            try
            {
                processLogger.Info(e.Data);
            }
            catch (Exception ex)
            {
                LOGGER.Error("Error occurred while trying to log the ProActive Runtime process stderr", ex);
            }
        }

        // fires whenever standard output is produced
        private void Events_OnProActiveRuntimeProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Length == 0)
            {
                return;
            }
            try
            {
                processLogger.Info(e.Data);
            }
            catch (Exception ex)
            {
                LOGGER.Error("Error occurred while trying to log the ProActive Runtime process stdout", ex);
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
                LOGGER.Info("The ProActive Runtime process [pid:" + this.proActiveRuntimeProcess.Id + "] reached the memory limit and will be killed");
                this.stop();
                return;
            }
            // Log info about the incriminated process            
            LOGGER.Info("The process " + incriminatedProcess.ProcessName + " [pid:" + incriminatedProcess.Id + "] reached the memory limit and will be killed");
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
                LOGGER.Error("Could not kill the incriminated process [pid:" + args.TheProcessId + "]", ex);
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
                    LOGGER.Info("A new process " + incriminatedProcess.ProcessName + " [pid:" + incriminatedProcess.Id + "] has been detected");
                }
                LOGGER.Info("Adding new process " + incriminatedProcess.ProcessName + " [pid:" + incriminatedProcess.Id + "] to the cpu limiter");
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
        [MethodImpl(MethodImplOptions.Synchronized)] // to avoid concurrent problems while calling stop method
        private void Events_OnProActiveRuntimeProcessExit(object o, EventArgs e)
        {
            LOGGER.Info("The ProActive Runtime process has exited for unknown reason");

            Thread.Sleep(1000);

            int proActiveRuntimeProcessPid = this.proActiveRuntimeProcess.Id;

            // Remove listeners and kill all forked processes
            this.internalClean();

            // Set current process to null
            this.proActiveRuntimeProcess = null;

            //this registry shows that the runtime is not running:
            setRegistryIsRuntimeStarted(false);

            if (disabledRestarting)
            {
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("The restarting has been disabled. Aborting restart ...");
                }
                return;
            }

            // if we use timer based config then we will use binary expotential backoff retry
            // in other case we restart process immediately

            // we only perform delayed restart when action originated from scheduled calendar event
            bool delayRestart = callersState.ContainsKey(ApplicationType.AgentScheduler) && (callersState[ApplicationType.AgentScheduler]) > 0;

            if (delayRestart)
            {
                // In order to restart correctly the delay must not be greater than the restart barrier date time
                // To do so add the restart delay in ms to now, the AddMilliseconds() method return a new instance of the modified date
                System.DateTime delayDateTime = System.DateTime.Now.AddMilliseconds(this.restartDelayInMs);
                if (delayDateTime < this.restartBarrierDateTime)
                {
                    // The restart timer will call the internalRestart method in the given delay
                    this.restartTimer.Change(this.restartDelayInMs, System.Threading.Timeout.Infinite);
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("The ProActive Runtime process restart delay is " + this.restartDelayInMs + " ms [barrier is " + this.restartBarrierDateTime.ToString() + "]");
                    }
                }
                else
                {
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Discarding the restart of the ProActive Runtime process because it would happen outside the allocated time. [delayDateTime: " + delayDateTime.ToString() + " and restartBarrierDateTime: " + this.restartBarrierDateTime.ToString() + "]");
                    }
                }
            }
            else
            {
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("Restarting the ProActive Runtime process immediately");
                }
                this.start();
            }
        }

        public void setRegistryIsRuntimeStarted(bool value)
        {
            try
            {
                RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_EXECUTORS_REG_SUBKEY, true);
                if (confKey != null)
                {
                    confKey.SetValue(this.rank + Constants.PROACTIVE_AGENT_IS_RUNNING_EXECUTOR_REG_VALUE_NAME, value);
                    confKey.Close();
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("The executor " + this.rank + " cannot write its state into the registry", e);
            }
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
                LOGGER.Debug("Stopping the ProActive Runtime process");
            }

            // Remove event listeners of the job object
            this.jobObject.Events.OnNewProcess -= Events_OnNewProcess;
            if (this.commonStartInfo.configuration.agentConfig.enableMemoryManagement)
            {
                this.jobObject.Events.OnJobMemoryLimit -= Events_OnJobMemoryLimit;
            }

            // clean listeners and kill all forked processes
            this.internalClean();
            this.proActiveRuntimeProcess = null;
            //-- runtime started = false
            setRegistryIsRuntimeStarted(false);
        }

        // !!WARNING!! This method removes all listeners and kills all processes in the job object
        // use it with caution, only inside a OnExit event or stop
        private void internalClean()
        {
            // Remove event listeners of the process
            this.proActiveRuntimeProcess.Exited -= Events_OnProActiveRuntimeProcessExit;
            this.proActiveRuntimeProcess.ErrorDataReceived -= Events_OnProActiveRuntimeProcessErrorDataReceived;
            this.proActiveRuntimeProcess.OutputDataReceived -= Events_OnProActiveRuntimeProcessOutputDataReceived;

            // Clear the watch list of the cpu limiter
            this.cpuLimiter.clearWatchList();

            // Use the job object to kill the ProActive Runtime process and its forked processes
            this.jobObject.TerminateAllProcesses(42); // this exit code was used in the examples of the JobObjectWrapper API                       

            // If a "On Runtime Exit" script was specified run it (this can cause serious issues in case of never-ending script)
            // Check if a script was specified 
            string scriptAbsolutePath = this.commonStartInfo.configuration.agentConfig.onRuntimeExitScript;
            if (scriptAbsolutePath == null || scriptAbsolutePath.Equals(""))
            {
                return;
            }

            LOGGER.Info("On runtime exit script: " + scriptAbsolutePath + " " + this.proActiveRuntimeProcess.Id);
            try
            {
                string scriptOutput = ScriptExecutor.executeScript(scriptAbsolutePath, "" + this.proActiveRuntimeProcess.Id);
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug(scriptOutput);
                }
            } catch (Exception e)
            {
                LOGGER.Error("Unable to execute on runtime exit script!", e);
            }
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
                    LOGGER.Debug("This app type didn't start the action so it doesn't have the right to stop it");
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

        // Called by the restart timer the ProActive Runtime process needs to be restarted after a delay
        // !! WARNING !! This method must be called by the restart timer only !
        private void internalRestart(object obj)
        {
            this.restartDelayInMs = this.restartDelayInMs + RESTART_DELAY_INCREMENT_IN_MS;
            if (this.restartDelayInMs > MAX_RESTART_DELAY_IN_MS)
            {
                this.restartDelayInMs = MAX_RESTART_DELAY_IN_MS;
            }
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Restart delay is now set to " + this.restartDelayInMs + " ms");
            }
            this.start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void dispose()
        {
            // Delete everything from the state
            callersState.Clear();
            // Dispose the restart timer
            this.restartTimer.Dispose();
            // Stop the current process
            this.stop();
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

        private static IAppender CreateRollingFileAppender(string name, string fileName)
        {
            log4net.Appender.RollingFileAppender appender = new log4net.Appender.RollingFileAppender();
            appender.Name = name;
            appender.File = fileName;
            appender.AppendToFile = true;
            appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
            appender.MaxSizeRollBackups = 10;
            appender.MaximumFileSize = "5MB";
            appender.StaticLogFileName = true;

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%date - %m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }
    }
}
