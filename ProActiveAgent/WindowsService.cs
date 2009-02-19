using System;
using System.Collections.Generic;
using System.ServiceProcess;
using ConfigParser;
using log4net;
using Microsoft.Win32;

namespace ProActiveAgent
{
    class WindowsService : ServiceBase
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// The location whre the agent is installed.</summary>
        private readonly string agentInstallLocation;
        /// <summary>
        /// The location of the cofig file.</summary>
        private readonly string agentConfigLocation;
        /// <summary>
        /// Process object that represents running runner script.</summary>                        
        private readonly List<ProActiveExec> processExecutors;

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

            this.processExecutors = new List<ProActiveExec>();
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
                        
            ProActiveExec.setRegistryIsRuntimeStarted(false);

            // Parse the configuration file once per start for all ProActive executors
            Configuration configuration = ConfigurationParser.parseXml(this.agentConfigLocation, this.agentInstallLocation);

            // Read classpath
            if (configuration.agentConfig.classpath == null || configuration.agentConfig.classpath.Equals(""))
            {
                try
                {
                    Utils.readClasspath(configuration.agentConfig);
                }
                catch (Exception ex)
                {
                    LOGGER.Error("An exception occured when reading the classpath!", ex);
                    return;
                }
            }

            // Find enabled action, ONLY ONE ACTION CAN BE ENABLED
            ConfigParser.Action enabledAction = null;
            foreach (ConfigParser.Action action in configuration.actions)
            {
                if (action.isEnabled) {
                    enabledAction = action;
                    break;
                } 
            }

            if (enabledAction == null) {
                LOGGER.Warn("No enabled action in the configuration. Exiting ...");
                return;
            }

            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Starting action " + enabledAction.GetType().Name);
            }            

            // Create new executor and initialize it
            ProActiveExec executor = new ProActiveExec(configuration, enabledAction);
            this.processExecutors.Add(executor);
            executor.init();

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
            if (this.processExecutors != null && this.processExecutors.Count > 0)
            {
                foreach (ProActiveExec p in processExecutors)
                {
                    p.dispose();
                }
                processExecutors.Clear();
            }

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
            base.OnPause();
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
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
            foreach (ProActiveExec p in processExecutors)
            {
                p.disableRestarting();
            }
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
                    foreach (ProActiveExec p in processExecutors)
                    {
                        p.sendStartAction(ApplicationType.AgentScreensaver);
                    }
                    break;
                case PAACommands.ScreenSaverStop:
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Received stop command from ProActive Agent screenSaver.");
                    }
                    foreach (ProActiveExec p in processExecutors)
                    {
                        p.sendStopAction(ApplicationType.AgentScreensaver);
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
}