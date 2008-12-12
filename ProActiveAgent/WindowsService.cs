using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using ConfigParser;
using System.Threading;
using Microsoft.Win32;
using System.Collections;
using System.Security;
using System.Security.Principal;
using log4net;

namespace ProActiveAgent
{
    class WindowsService : ServiceBase
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string agentInstallLocation;
        private readonly string agentConfigLocation;

        private Configuration configuration;
        private TimerManager timerManager;
        private ProActiveExec exec;
        private ArrayList agregations;        

        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsService()
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("ProActive Agent application loaded successfully.");
            }

            // Set the service name
            base.ServiceName = Constants.PROACTIVE_AGENT_SERVICE_NAME;
            // Set the windows event log type
            base.EventLog.Log = Constants.PROACTIVE_AGENT_WINDOWS_EVENTLOG_LOG;

            // Try to read install and config locations from registry
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY);
            if (confKey == null)
            {
                if (LOGGER.IsWarnEnabled)
                {
                    LOGGER.Warn("ProActive Agent could not read " + Constants.PROACTIVE_AGENT_REG_SUBKEY + " from windows registry.");
                }
                // If registry key is unknown set default locations
                this.agentInstallLocation = Constants.PROACTIVE_AGENT_DEFAULT_INSTALL_LOCATION;
                this.agentConfigLocation = Constants.PROACTIVE_AGENT_DEFAULT_CONFIG_LOCATION;
            }
            else
            {
                this.agentInstallLocation = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_INSTALL_REG_VALUE_NAME);
                this.agentConfigLocation = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_CONFIG_REG_VALUE_NAME);
                confKey.Close();
            }

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;

            this.timerManager = null;
            this.exec = null;
            this.agregations = new ArrayList();
        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether
        ///    or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Starting ProActive Agent service.");
            }

            // TODO: zero all of the members/properties
            //-- runtime started = true
            ProActiveExec.setRegistryIsRuntimeStarted(false);

            // Parse the configuration file
            this.configuration = ConfigurationParser.parseXml(this.agentConfigLocation, this.agentInstallLocation);

            // Init loggers
            // LoggerComposite composite = new LoggerComposite();
            // composite.addLogger(new FileLogger(this.agentLocation));
            // composite.addLogger(new EventLogger());
            // logger = composite;

            //--Foreach action
            ProActiveExec exe;
            TimerManager tim;
            Agregation agre;

            foreach (ConfigParser.Action action in configuration.actions.actions)
            {
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("Starting action " + action.GetType().Name);                    
                } 

                exe = new ProActiveExec(agentInstallLocation, configuration.agentConfig.jvmParams,
                configuration.agentConfig.javaHome, configuration.agentConfig.proactiveLocation,
                action.priority, action.initialRestartDelay);

                tim = new TimerManager(this.configuration, action, exe);

                exe.setTimerMgr(tim);

                //--Save in collection
                agre = new Agregation(exe, tim, action);
                agregations.Add(agre);
            }
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("All actions are scheduled.");                
            }             
            base.OnStart(args);
        }

        /// <summary>
        /// OnStop(): Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Stopping ProActive Agent service.");
            }
            
            foreach (Agregation a in agregations)
            {
                a.exec.dispose();
                a.timerManager.dispose();
                a.exec.sendGlobalStop();
            }
            agregations.Clear();
            
            this.configuration = null;
            this.timerManager = null;
            this.exec = null;            

            //-- runtime started = false
            ProActiveExec.setRegistryIsRuntimeStarted(false);

            //BUT THIS SHOULD NOT BE NECESSARY:
            base.OnStop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            timerManager.onPause();
            base.OnPause();
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            timerManager.onResume();
            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Shutting down the ProActive Agent service.");
            }            
            exec.disableRestarting();
            /*            exec.disableRestarting();
                        exec.sendGlobalStop();
                        this.configuration = null;
                        this.timerManager = null;
                        this.exec = null; */

            //-- runtime started = false
            //TODO: need to think if you should keep this: 
            //ProActiveExec.setRegistryIsRuntimeStarted(false);
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);
            switch ((PAACommands)command)
            {
                case PAACommands.ScreenSaverStart:
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Received start command from ProActive Agent screenSaver.");
                    }
                    foreach (Agregation agregation in agregations)
                    {
                        agregation.exec.sendStartAction(agregation.action, ApplicationType.AgentScreensaver);
                    }
                    break;
                case PAACommands.ScreenSaverStop:
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Received stop command from ProActive Agent screenSaver.");
                    }                    
                    foreach (Agregation agregation in agregations)
                    {
                        agregation.exec.sendStopAction(agregation.action, ApplicationType.AgentScreensaver);
                    }
                    break;
                case PAACommands.GlobalStop:                    
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Received global stop command.");
                    }  
                    foreach (Agregation agregation in agregations)
                    {
                        agregation.exec.sendGlobalStop();
                    }
                    break;
                default:                    
                    break;
            }
            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcast Status
        /// (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event
        ///   from a Terminal Server session.
        ///   Useful if you need to determine
        ///   when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription">The Session Change
        /// Event that occured.</param>
        protected override void OnSessionChange(
                  SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main()
        {
            ServiceBase.Run(new WindowsService());
        }
    }

    public class Agregation
    {
        public readonly TimerManager timerManager;
        public readonly ProActiveExec exec;
        public readonly ConfigParser.Action action;

        public Agregation(ProActiveExec exec, TimerManager timerManager, ConfigParser.Action action)
        {
            this.timerManager = timerManager;
            this.exec = exec;
            this.action = action;            
        }
    }
}