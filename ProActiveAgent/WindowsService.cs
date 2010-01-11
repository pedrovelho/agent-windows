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
 *  Initial developer(s):               The ProActive Team
 *                        http://proactive.inria.fr/team_members.htm
 *  Contributor(s): ActiveEon Team - http://www.activeeon.com
 *
 * ################################################################
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
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
        private string agentInstallLocation;
        /// <summary>
        /// The location of the cofig file.</summary>
        private string agentConfigLocation;
        /// <summary>
        /// The executors manager that loads events and keeps start/stop timers.</summary>
        private ExecutorsManager executorsManager;
        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsService()
        {
            // Set the service name
            base.ServiceName = Constants.PROACTIVE_AGENT_SERVICE_NAME;
            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.            
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            LOGGER.Info("Starting ProActive Agent service");

            // Try to read install and config locations from registry
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY);
            if (confKey == null)
            {
                if (LOGGER.IsWarnEnabled)
                {
                    LOGGER.Warn("ProActive Agent could not read " + Constants.PROACTIVE_AGENT_REG_SUBKEY + " from windows registry");
                }
                // If registry key is unknown set default locations
                this.agentInstallLocation = Constants.PROACTIVE_AGENT_DEFAULT_INSTALL_LOCATION;
                this.agentConfigLocation = Constants.PROACTIVE_AGENT_DEFAULT_CONFIG_LOCATION;
            }
            else
            {
                this.agentInstallLocation = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_INSTALL_LOCATION_REG_VALUE_NAME);
                this.agentConfigLocation = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_CONFIG_LOCATION_REG_VALUE_NAME);
                confKey.Close();
            }

            // Parse the configuration file once per start
            Configuration configuration = ConfigurationParser.parseXml(this.agentConfigLocation, this.agentInstallLocation);

            // Read classpath
            try
            {
                Utils.readClasspath(configuration.agentConfig);
            }
            catch (Exception ex)
            {
                LOGGER.Error("An exception occured when reading the classpath", ex);
                return;
            }

            this.executorsManager = new ExecutorsManager(configuration);

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
                LOGGER.Debug("Stopping ProActive Agent service");
            }

            if (this.executorsManager != null)
            {
                this.executorsManager.dispose();
            }

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
                LOGGER.Debug("Shutting down the ProActive Agent service");
            }

            foreach (ProActiveRuntimeExecutor p in this.executorsManager.getExecutors())
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
                    foreach (ProActiveRuntimeExecutor p in this.executorsManager.getExecutors())
                    {
                        p.sendStartAction(ApplicationType.AgentScreensaver);
                    }
                    break;
                case PAACommands.ScreenSaverStop:
                    foreach (ProActiveRuntimeExecutor p in this.executorsManager.getExecutors())
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
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether
        ///    or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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
