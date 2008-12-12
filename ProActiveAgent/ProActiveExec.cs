using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using ConfigParser;
using System.Threading;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using System.Collections;
using System.Security;
using System.Security.Principal;
using log4net;

/** Executor of runner script and thus ProActive runtime.
 *  This class implements semantics of all actions available
 */

namespace ProActiveAgent
{
    public class ProActiveExec
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ILog PROACTIVE_RUNTIME_PROCESS_LOGGER = LogManager.GetLogger("ProActiveRuntimeProcessLogger");

        [DllImport("pkill.dll", EntryPoint = "_KillProcessEx@8", CallingConvention = CallingConvention.Winapi)]
        private static extern bool KillProcessEx(uint dwProcessId, bool bTree);

        private static string SCRIPT_NAME = "agentservice.bat";
        private static int INITIAL_RESTART_DELAY = 3000;
        private static int MAX_RESTART_DELAY = 10000;

        private String scriptLocation;          // location of runner script
        private Process proActiveRuntimeProcess;                // process object that represents running runner script
        private String cmd;                     // [command] argument used to launch runner script
        private String[] args;                  // other arguments provided to runner script
        private String jvmOptions;
        private String javaLocation;
        private String proactiveLocation;
        private TimerManager timerMgr;
        private ProcessPriorityClass priority;
        private bool disabledRestarting = false; // restarting of the process is disabled when the system shuts down
        //private bool allowRuntime = true;
        private int initialRestartDelay;

        //For an application type (i.e AgentScheduler) boolean value = true if this type has sent the "start command". It is set to false when the same app type sends stop command. 
        private Dictionary<ApplicationType, Int32> callersState;
        // state of the calling applications 
        // (if there were unstopped start actions)

        //private Dictionary<String, string[]> todoStack;
        //private ArrayList todoStack;
        //private ProcessObserver observer;
        private int restartDelay;

        public ProActiveExec(String scriptLocation, String jvmOptions, String javaLocation,
            String proactiveLocation, String priority, int initialRestartDelay)
        {
            this.callersState = new Dictionary<ApplicationType, Int32>();
            this.scriptLocation = scriptLocation;
            this.jvmOptions = jvmOptions;
            this.javaLocation = javaLocation;
            this.proactiveLocation = proactiveLocation;
            this.timerMgr = null;
            this.proActiveRuntimeProcess = null;
            this.cmd = null;
            this.args = null;

            this.priority = getPriority(priority);
            this.initialRestartDelay = (initialRestartDelay > 0 ? initialRestartDelay : INITIAL_RESTART_DELAY);
        }

        public void resetRestartDelay()
        {
            this.restartDelay = initialRestartDelay;
        }

        private ProcessPriorityClass getPriority(string priority)
        {
            if (priority.Equals("Idle"))
            {
                return ProcessPriorityClass.Idle;
            }
            else if (priority.Equals("Normal"))
            {
                return ProcessPriorityClass.Normal;
            }
            else if (priority.Equals("High"))
            {
                return ProcessPriorityClass.High;
            }
            else if (priority.Equals("RealTime"))
            {
                return ProcessPriorityClass.RealTime;
            }
            else return ProcessPriorityClass.Normal;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void setTimerMgr(TimerManager timerMgr)
        {
            this.timerMgr = timerMgr;
        }

        private bool startP2P(string[] hosts)
        {
            return start("P2P", hosts);
        }

        private bool startRM(string rmHost, string nodeName)
        {
            String[] args = (nodeName != "" ?
                args = new String[] { rmHost, nodeName } :
                args = new String[] { rmHost }
            );
            return start("RM", args);
        }

        private bool startAdvert(string nodeName)
        {
            String[] args = (nodeName != null ?
                args = new String[] { nodeName } :
                args = new String[0]);
            return start("ADVERT", args);
        }

        private string quote(string toQuote)
        {
            return "\"" + toQuote + "\"";
        }

        // Starts runner script with given [command] argument
        // and optionally other arguments (args parameter)
        // this method has to be synchronized as it is dealing with a process object

        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool start(String cmd, string[] args)
        {
            if (this.disabledRestarting || this.proActiveRuntimeProcess != null)
            {
                return false;
            }

            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Preparing to start new process with cmd : " + cmd + " args : " + args);
            }

            this.cmd = cmd;
            this.args = args;

            // Command-line params building
            StringBuilder argsBld = new StringBuilder();
            foreach (string arg in args)
            {
                argsBld.Append(" " + arg);
            }

            // Quote all params (Ask Olivier why we need to do this ?)
            string action_args = quote(argsBld.ToString());
            string action_cmd = quote(cmd);
            string proactive_location = quote(proactiveLocation);
            string jvm_location = quote(javaLocation);
            string jvm_args = quote(jvmOptions);

            // We start a new process
            proActiveRuntimeProcess = new Process();

            // We attach a handler in order to intercept killing of that process
            // Therefore, we will be able to relaunch script in that event
            proActiveRuntimeProcess.EnableRaisingEvents = true;
            proActiveRuntimeProcess.Exited += manageRestart;

            // Use process info to specify all options
            ProcessStartInfo info = new ProcessStartInfo();

            // Configure the command-line                         
            info.FileName = scriptLocation + "\\" + SCRIPT_NAME;
            info.Arguments = proactive_location + " " + jvm_location + " " + jvm_args + " " + action_cmd + " " + action_args;

            // Configure runtime specifics
            info.UseShellExecute = false; // needed to redirect output
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            proActiveRuntimeProcess.StartInfo = info;

            // setup event handlers
            proActiveRuntimeProcess.EnableRaisingEvents = true;
            proActiveRuntimeProcess.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            proActiveRuntimeProcess.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);

            try
            {
                proActiveRuntimeProcess.Start();

                if (LOGGER.IsInfoEnabled)
                {
                    LOGGER.Info("Started ProActive Runtime process. Pid : " + proActiveRuntimeProcess.Id + ", Command-line: \"" + info.FileName + "\" " + info.Arguments);
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("Could not start the ProActive Runtime process!", ex);
            }

            // notify process about asynchronous reads
            proActiveRuntimeProcess.BeginErrorReadLine();
            proActiveRuntimeProcess.BeginOutputReadLine();

            proActiveRuntimeProcess.PriorityClass = ProcessPriorityClass.Idle;            

            //-- runtime started = true
            setRegistryIsRuntimeStarted(true);

            return true;
        }

        // fires whenever errors output is produced
        private static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
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
        private static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
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

        private void manageRestart(object o, EventArgs e)
        {
            Thread.Sleep(1000);
            restart(o, e);
        }

        public static void setRegistryIsRuntimeStarted(Boolean value)
        {            
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY,true);
            if (confKey != null)
            {
                confKey.SetValue("IsRuntimeStarted", value);
            }
            confKey.Close();
        }

        // if the process is killed and should be running, it is restarted
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void restart(object o, EventArgs e)
        {
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

                if (timerMgr != null && delayRestart)
                {
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Restarting the ProActive Runtime process in " + restartDelay + " ms.");
                    }
                    timerMgr.addDelayedRetry(restartDelay);
                }
                else
                {
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Restarting the ProActive Runtime process immediately !");
                    }
                    start(this.cmd, this.args);
                }
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
                LOGGER.Debug("Stopping the ProActive Runtime process...");
            }

            this.cmd = null;
            this.args = null;

            // kill the process tree
            //disableRestarting();

            if (!this.proActiveRuntimeProcess.HasExited)
            {
                this.proActiveRuntimeProcess.Exited -= manageRestart;
                KillProcessEx((uint)this.proActiveRuntimeProcess.Id, true);
            }
            this.proActiveRuntimeProcess = null;
            //-- runtime started = false
            setRegistryIsRuntimeStarted(false);
        }

        // called from other parts of ProActive Agent
        // this method dispatches action depending on type
        // and calls internal methods

        public void sendStartAction(object whatToDo, ApplicationType appType)
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
            if (whatToDo is P2PAction)
            {
                P2PAction action = (P2PAction)whatToDo;
                startP2P(action.contacts);
            }
            else if (whatToDo is RMAction)
            {
                RMAction action = (RMAction)whatToDo;
                startRM(action.url, action.nodeName);
            }
            else if (whatToDo is AdvertAction)
            {
                AdvertAction action = (AdvertAction)whatToDo;
                startAdvert(action.nodeName);
            }
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
            start(this.cmd, this.args);
        }

        // user pushed red button - STOP EVERYTHING
        public void sendGlobalStop()
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Received global stop request");
            }
            callersState.Clear(); // we delete everything from the state
            stop();
        }

        public void sendStopAction(object whatToDo, ApplicationType appType)
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
            stop();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void dispose()
        {
            this.timerMgr = null;
        }
    }
}
