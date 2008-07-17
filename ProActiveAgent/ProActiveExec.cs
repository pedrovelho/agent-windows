using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using ConfigParser;
using System.Threading;

/** Executor of runner script and thus ProActive runtime.
 *  This class implements semantics of all actions available
 */

namespace ProActiveAgent
{
    public class ProActiveExec
    {
        // Import of unmanaged library responsible for killing Windows processes recursively
        
        [DllImport("pkill.dll", EntryPoint = "_KillProcessEx@8", CallingConvention = CallingConvention.Winapi)]
        private static extern bool KillProcessEx(uint dwProcessId, bool bTree);

        private static string SCRIPT_NAME = "agentservice.bat";

        private String scriptLocation;          // location of runner script
        private Process process;                // process object that represents running runner script
        private String cmd;                     // [command] argument used to launch runner script
        private String[] args;              // other arguments provided to runner script
        private String jvmOptions;
        private String javaLocation;
        private String proactiveLocation;
        private TimerManager timerMgr;
        private ProcessPriorityClass priority;

        private Dictionary<ApplicationType, Boolean> callersState; // state of the calling applications 
                                                                   // (if there were unstopped start actions)
        private ProcessObserver observer;
        private Logger logger;
        private int restartDelay = 1000;
        
        public ProActiveExec(Logger logger, String scriptLocation, String jvmOptions, String javaLocation,
            String proactiveLocation, String priority)
        {
            this.observer = new ProcessObserver(logger);
            this.callersState = new Dictionary<ApplicationType, Boolean>();
            this.scriptLocation = scriptLocation;
            this.jvmOptions = jvmOptions;
            this.javaLocation = javaLocation;
            this.proactiveLocation = proactiveLocation;
            this.timerMgr = null;
            this.process = null;
            this.cmd = null;
            this.args = null;
            this.logger = logger;
            this.priority = getPriority(priority);
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

        public void setTimerMgr(TimerManager timerMgr)
        {
            this.timerMgr = timerMgr;
        }

        public Logger getLogger()
        {
            return logger;
        }

        public bool isRunning()
        {
            return process != null;
        }

        private bool startP2P(string[] hosts)
        {
            if (isRunning())
                return false;
            return start("P2P", hosts);
        }

        private bool startRM(string rmHost)
        {
            if (isRunning())
                return false;
            String[] args = new String[] { rmHost };
            return start("RM", args);
        }

        private bool startAdvert(string nodeName)
        {
            if (isRunning())
                return false;
            String[] args;
            if (nodeName != null)
                args = new String[] { nodeName };
            else
                args = new String[0];
            return start("ADVERT", args);
        }

        private string quote(string toQuote)
        {
            return "\"" + toQuote + "\"";
        }

        // Precond: process is null
        // Starts runner script with given [command] argument
        // and optionally other arguments (args parameter)

        private bool start(String cmd, string[] args)
        {
            logger.log("Starting: " + cmd, LogLevel.TRACE);

            this.cmd = cmd;
            this.args = args;
            // We start a new process
            process = new Process();
            process.StartInfo.FileName = scriptLocation + "\\" + SCRIPT_NAME;
            
            // Command-line params building
            StringBuilder argsBld = new StringBuilder();
            foreach( string arg in args) {
                argsBld.Append(" " + arg);
            }

            string action_args = quote(argsBld.ToString());
            string action_cmd = quote(cmd);
            string proactive_location = quote(proactiveLocation);
            string jvm_location = quote(javaLocation);
            string jvm_args = quote(jvmOptions);

            process.StartInfo.Arguments = proactive_location + " " + jvm_location + " " + jvm_args + " " + action_cmd + " " + action_args;
            // We attach a handler in order to intercept killing of that process
            // Therefore, we will be able to relaunch script in that event
            process.EnableRaisingEvents = true;
            process.Exited += restart;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            logger.log("Full path of the script: " + process.StartInfo.FileName, LogLevel.TRACE);
            logger.log("Arguments: " + process.StartInfo.Arguments, LogLevel.TRACE);

            process.Start();
            process.PriorityClass = ProcessPriorityClass.Idle;
            observer.setMonitorredProcess(process);
            logger.log("The ProActive process was successfuly started", LogLevel.INFO);
            return true;
        }

        // if the process is killed and should be running, it is restarted
        private void restart(object o, EventArgs e)
        {
            observer.setMonitorredProcess(null);
            logger.log("ProActive process ended prematurely", LogLevel.INFO);
            
            // if we use timer based config then we will use binary expotential backoff retry
            // in other case we restart process immediately

            // we only perform delayed restart when action originated from scheduled calendar event

            bool delayRestart = false;

            if (callersState.ContainsKey(ApplicationType.AgentScheduler) && callersState[ApplicationType.AgentScheduler])
                delayRestart = true;

            if (timerMgr != null && delayRestart)
            {
                logger.log("Restarting in " + restartDelay + " ms", LogLevel.TRACE);
                timerMgr.addDelayedRetry(restartDelay);
            }
            else
            {
                logger.log("Restarting now!", LogLevel.TRACE);
                start(this.cmd, this.args);
            }
        }

        private void stop()
        {
            logger.log("Stopping ProActive process...", LogLevel.INFO);
            observer.setMonitorredProcess(null);
            
            this.cmd = null;
            this.args = null;
            // kill the process tree
            if (!this.process.HasExited)
            {
                process.Exited -= restart;
                KillProcessEx((uint)this.process.Id, true);
            }
            this.process = null;
        }

        // called from other parts of ProActive Agent
        // this method dispatches action depending on type
        // and calls internal methods

        public void sendStartAction(object whatToDo, ApplicationType appType)
        {
            logger.log("Received start action request from " + appType.ToString(), LogLevel.INFO);
            callersState[appType] = true;
            if (whatToDo is P2PAction)
            {
                P2PAction action = (P2PAction)whatToDo;
                startP2P(action.contacts);
            }
            else if (whatToDo is RMAction)
            {
                RMAction action = (RMAction)whatToDo;
                startRM(action.url);
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
            logger.log("Received start action request from AgentScheduler", LogLevel.INFO);
            restartDelay <<= 1;
            start(this.cmd, this.args);
        }

        // user pushed red button - STOP EVERYTHING

        public void sendGlobalStop()
        {
            logger.log("Received global stop request", LogLevel.INFO);
            callersState.Clear(); // we delete everything from the state
            if (isRunning())
                stop();
        }

        public void sendStopAction(object whatToDo, ApplicationType appType)
        {
            logger.log("Received stop action request from " + appType.ToString(), LogLevel.INFO);
            if (!callersState.ContainsKey(appType))
                return; // there were no previous actions from this application type (or we deleted them)
            if (!callersState[appType])
                return; // this app type didn't start the action
            // change state
            callersState[appType] = false;
            foreach (KeyValuePair<ApplicationType,Boolean> app in callersState)
            {
                if (app.Value)
                    // someone else sent the start command too
                    return; 
            }
            // if we reach that far, then we can stop the action provided it is still running ;)
            if (isRunning())
                stop();
        }

    }
}
