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

/** Executor of runner script and thus ProActive runtime.
 *  This class implements semantics of all actions available
 */

namespace ProActiveAgent
{

    public class ProActiveExec
    {
        [DllImport("pkill.dll", EntryPoint = "_KillProcessEx@8", CallingConvention = CallingConvention.Winapi)]
        private static extern bool KillProcessEx(uint dwProcessId, bool bTree);

        private static string SCRIPT_NAME = "agentservice.bat";
        private static int INITIAL_RESTART_DELAY = 3000;
        private static int MAX_RESTART_DELAY = 10000;

        private String scriptLocation;          // location of runner script
        private Process process;                // process object that represents running runner script
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
        private ProcessObserver observer;
        private Logger logger;
        private int restartDelay;

        public ProActiveExec(Logger logger, String scriptLocation, String jvmOptions, String javaLocation,
            String proactiveLocation, String priority, int initialRestartDelay)
        {
            WindowsService.log("#>ProActiveExec", LogLevel.INFO);
            //this.observer = new ProcessObserver(logger);
            this.callersState = new Dictionary<ApplicationType, Int32>();
            this.scriptLocation = scriptLocation;
            this.jvmOptions = jvmOptions;
            this.javaLocation = javaLocation;
            this.proactiveLocation = proactiveLocation;
            this.timerMgr = null;
            this.process = null;
            this.cmd = null;
            this.args = null;
            this.logger = logger;
            this.observer = new ProcessObserver(logger);

            this.priority = getPriority(priority);

            if (initialRestartDelay > 0)
            {
                this.initialRestartDelay = initialRestartDelay;
            }
            else
                this.initialRestartDelay = INITIAL_RESTART_DELAY;
            //setRegistryAllowRuntime(true);
            //todoStack  = new ArrayList();
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

        public Logger getLogger()
        {
            return logger;
        }

        private bool startP2P(string[] hosts)
        {
            return start("P2P", hosts);
        }

        private bool startRM(string rmHost, string nodeName)
        {
            String[] args;
            if (nodeName != "")
            {
                args = new String[] { rmHost, nodeName };
                WindowsService.log("start with nodename",LogLevel.INFO);
            }
            else
            {
                args = new String[] { rmHost};
                WindowsService.log("start without nodename",LogLevel.INFO);
            }

            return start("RM", args);
        }

        private bool startAdvert(string nodeName)
        {
            String[] args;
            if (nodeName != null)
                args = new String[] { nodeName };
            else
                args = new String[0];
            //WindowsService.log("We are just before the start method", LogLevel.TRACE);
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
            if (!disabledRestarting)
            {

                //WindowsService.log("We are inside start method", LogLevel.TRACE);
                if (process != null)
                {
                    //WindowsService.log("The process is not null", LogLevel.TRACE);
                    return false;
                }
                WindowsService.log("Starting: " + cmd, LogLevel.TRACE);

                this.cmd = cmd;
                this.args = args;
                // We start a new process
                process = new Process();

                process.StartInfo.FileName = scriptLocation + "\\" + SCRIPT_NAME;

                //process.StartInfo.FileName = "C:\\win\\run.bat";

                // Command-line params building
                StringBuilder argsBld = new StringBuilder();
                foreach (string arg in args)
                {
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
                process.Exited += manageRestart;
                
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;

                WindowsService.log("Command line: \"" + process.StartInfo.FileName + "\" " + process.StartInfo.Arguments, LogLevel.TRACE);

                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    WindowsService.log("Exception: " + ex.Message, LogLevel.TRACE);
                }
                
                process.PriorityClass = ProcessPriorityClass.Idle;
                //observer.setMonitorredProcess(process);
                WindowsService.log("The ProActive process was successfuly started", LogLevel.INFO);

                //-- runtime started = true
                setRegistryIsRuntimeStarted(true);

                return true;
            }
            return false;
        }

        private void manageRestart(object o, EventArgs e)
        {
            Thread.Sleep(1000);
            restart(o,e);
        }

        public static void setRegistryIsRuntimeStarted(Boolean value)
        {
            WindowsService.log("setRegistryIsRuntimeStarted = "+ value, LogLevel.INFO);
            RegistryKey confKey = Registry.LocalMachine.CreateSubKey("Software\\ProActiveAgent");
            if (confKey != null)
            {
                confKey.SetValue("IsRuntimeStarted", value);
            }
            confKey.Close();
        }

        /*public static void setRegistryAllowRuntime(Boolean value)
        {
            RegistryKey confKey = Registry.LocalMachine.CreateSubKey("Software\\ProActiveAgent");
            if (confKey != null)
            {
                confKey.SetValue("AllowRuntime", value);
            }
            confKey.Close();
        }*/

        // if the process is killed and should be running, it is restarted
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void restart(object o, EventArgs e)
        {
            //this registry shows that the runtime is not running:
            setRegistryIsRuntimeStarted(false);
            if (!disabledRestarting)
            {
                observer.setMonitorredProcess(null);
                WindowsService.log("ProActive process ended prematurely", LogLevel.INFO);

                // if we use timer based config then we will use binary expotential backoff retry
                // in other case we restart process immediately

                // we only perform delayed restart when action originated from scheduled calendar event

                bool delayRestart = false;

                if (callersState.ContainsKey(ApplicationType.AgentScheduler) && (callersState[ApplicationType.AgentScheduler]) > 0)
                    delayRestart = true;

                this.process = null;

                if (timerMgr != null && delayRestart)
                {
                    WindowsService.log("Restarting in " + restartDelay + " ms", LogLevel.TRACE);
                    //                    try
                    //                    {
                    timerMgr.addDelayedRetry(restartDelay);
                    //                    }
                    //                    catch (Exception)
                    //                    {
                    //                        WindowsService.log("Restart operation failed.", LogLevel.TRACE);
                    //                    }
                }
                else
                {
                    WindowsService.log("Restarting now!", LogLevel.TRACE);
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
            if (this.process == null)
                return;
            WindowsService.log("Stopping ProActive process...", LogLevel.INFO);
            observer.setMonitorredProcess(null);

            this.cmd = null;
            this.args = null;
            // kill the process tree
            //disableRestarting();

            if (!this.process.HasExited)
            {
                this.process.Exited -= manageRestart;
                KillProcessEx((uint)this.process.Id, true);
            }
            this.process = null;
            //-- runtime started = false
            setRegistryIsRuntimeStarted(false);
        }

        // called from other parts of ProActive Agent
        // this method dispatches action depending on type
        // and calls internal methods

        public void sendStartAction(object whatToDo, ApplicationType appType)
        {
            WindowsService.log("Received start action request from " + appType.ToString(), LogLevel.INFO);
            if (callersState.ContainsKey(appType))
            {
                callersState[appType]++;
                //WindowsService.log("callersState ++ " + appType.ToString(), LogLevel.INFO);
            }
            else
            {
                callersState.Add(appType, 1);
                //WindowsService.log("callersState 1 " + appType.ToString(), LogLevel.INFO);
            }

            /*{
                ArrayList action2 = new ArrayList();
                action2.Add(whatToDo);
                action2.Add(appType);

                todoStack.Add(action2);
            }
            if (!allowRuntime)
            {
                return;
            }*/
            if (whatToDo is P2PAction)
            {
                P2PAction action = (P2PAction)whatToDo;
                startP2P(action.contacts);
            }
            else if (whatToDo is RMAction)
            {
                RMAction action = (RMAction)whatToDo;
                startRM(action.url,action.nodeName);
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
            WindowsService.log("Received start action request from AgentScheduler", LogLevel.INFO);
            restartDelay <<= 1;
            if (restartDelay > MAX_RESTART_DELAY)
                restartDelay = MAX_RESTART_DELAY;
            start(this.cmd, this.args);
        }

        // user pushed red button - STOP EVERYTHING
        public void sendGlobalStop()
        {
            WindowsService.log("Received global stop request", LogLevel.INFO);
            callersState.Clear(); // we delete everything from the state
            stop();
        }

        public void sendStopAction(object whatToDo, ApplicationType appType)
        {
            WindowsService.log("Received stop action request from " + appType.ToString(), LogLevel.INFO);
            if (!callersState.ContainsKey(appType))
            {
                WindowsService.log("No caller for this appType", LogLevel.INFO);
                return; // there were no previous actions from this application type (or we deleted them)
            }

            if (callersState[appType] == 0)
            {
                WindowsService.log("No right", LogLevel.INFO);
                return; // this app type didn't start the action so it doesn't have the right to stop it
            }
            // change state
            callersState[appType]--;
            WindowsService.log("callersState -- " + appType.ToString(), LogLevel.INFO);

            foreach (KeyValuePair<ApplicationType, Int32> app in callersState)
            {
                //WindowsService.log("loop", LogLevel.INFO);
                if (app.Value > 0)
                {
                    //WindowsService.log("exit", LogLevel.INFO);
                    // someone else sent the start command too
                    return;
                }
            }
            //WindowsService.log("Before stop", LogLevel.INFO);
            stop();

            //--Remove from ArrayList
            /* int i = 0;
             int j = todoStack.Count;

             while (i < j)
             {
                 //--Retrieve action
                 ArrayList action = (ArrayList)todoStack[i];

                 if (whatToDo.Equals(action[0]) && appType.Equals(action[1]))
                 {

                     todoStack.Remove(action);
                     j--;
                 }
                 else
                     i++;
             }*/
        }

        /*public void sendAllowRuntime()
        {
            //save to a registry
            ProActiveExec.setRegistryAllowRuntime(true);
            allowRuntime = true;

            //read toDo stack and start each of them
            int i = 0;
            while (i < todoStack.Count)
            {
                //--Retrieve action
                ArrayList action = (ArrayList)todoStack[i];

                //--Start action
                sendStartAction(action[0], (ApplicationType)action[1]);
                start((String)action[0], (string[])action[1]);
            }
         }*/
        //old
        /*while (0 < todoStack.Count)
        {
            //--Retrieve action
            ArrayList action = (ArrayList)todoStack[0];

            //--Start action
            sendStartAction(action[0], (ApplicationType)action[1]);
            start((String)action[0], (string[])action[1]);

            //--Remove from ArrayList
            todoStack.RemoveAt(0);

            WindowsService.log((String)action[0], LogLevel.INFO);
        }*/

        //marche sans suppresion
        /*foreach (ArrayList action in todoStack)
        {
            start((String)action[0],(string[])action[1]);
            WindowsService.log((String)action[0], LogLevel.INFO);
        }*/
        //WindowsService.log("end", LogLevel.INFO);


        /*public void sendForbidRuntime()
        {
            //save to a registry
            ProActiveExec.setRegistryAllowRuntime(false);

            allowRuntime = false;

            stop();
            //sendGlobalStop();
        }*/

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void dispose()
        {
            this.timerMgr = null;
        }

        

    }
}
