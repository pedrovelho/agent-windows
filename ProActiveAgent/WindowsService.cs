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
using System.IO.Pipes;
using System.IO;
using System.Threading;

namespace ProActiveAgent
{
    class WindowsService : ServiceBase
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// The executors manager that loads events and keeps start/stop timers.</summary>
        private ExecutorsManager executorsManager;
        private Worker pipeServerWorker;

        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsService()
        {
            // Set the service name
            base.ServiceName = Constants.PROACTIVE_AGENT_SERVICE_NAME;
            // These Flags set whether or not to handle that specific
            // type of event. Set to true if you need it, false otherwise.
            this.CanPauseAndContinue = false;
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

            // The location will be read from the registry
            string agentInstallLocation;
            // The location of the cofig file
            string agentConfigLocation;

            // Try to read install and config locations from registry
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY);
            if (confKey == null)
            {
                if (LOGGER.IsWarnEnabled)
                {
                    LOGGER.Warn("ProActive Agent could not read " + Constants.PROACTIVE_AGENT_REG_SUBKEY + " from windows registry");
                }
                // If registry key is unknown set default locations
                agentInstallLocation = Constants.PROACTIVE_AGENT_DEFAULT_INSTALL_LOCATION;
                agentConfigLocation = Constants.PROACTIVE_AGENT_DEFAULT_CONFIG_LOCATION;
            }
            else
            {
                agentInstallLocation = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_INSTALL_LOCATION_REG_VALUE_NAME);
                agentConfigLocation = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_CONFIG_LOCATION_REG_VALUE_NAME);
                confKey.Close();
            }

            // Parse the configuration file once per start
            AgentType configuration = ConfigurationParser.parseXml(agentConfigLocation, agentInstallLocation);
            configuration.agentInstallLocation = agentInstallLocation;

            // Read classpath
            try
            {
                Utils.readClasspath(configuration.config);
            }
            catch (Exception ex)
            {
                LOGGER.Error("An exception occured when reading the classpath", ex);
                return;
            }

            this.executorsManager = new ExecutorsManager(configuration);

            this.pipeServerWorker = new Worker(this.executorsManager);

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

            this.pipeServerWorker.requestStop();

            //BUT THIS SHOULD NOT BE NECESSARY:
            base.OnStop();
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

        /// <summary>
        /// This worker class will start a thread that will wait for the         
        /// </summary>
        sealed class Worker
        {
            private readonly ExecutorsManager manager;
            private readonly Thread thread;
            private volatile bool shouldStop;

            public Worker(ExecutorsManager manager)
            {
                this.manager = manager;
                thread = new Thread(sendExecutorsCount);
                thread.IsBackground = true;
                // Start the server pipe 
                thread.Start();
            }

            // This method can be called outside worker thread
            public void requestStop()
            {               
                // Tell the main loop to stop
                this.shouldStop = true;

                // Wait until the thread is stopped (2s timeout)
                thread.Join(2000);                
            }

            private void sendExecutorsCount()
            {
                // Until the service stop loop and recreate a new pipe
                while (!shouldStop)
                {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(Constants.PIPE_NAME, PipeDirection.Out))
                    {
                        // Wait for a client to connect
                        pipeServer.WaitForConnection();

                        LOGGER.Info("The GUI client has connected");

                        try
                        {
                            // Create a stream writer                
                            using (StreamWriter sw = new StreamWriter(pipeServer))
                            {
                                sw.AutoFlush = true;

                                while (!shouldStop)
                                {
                                    int count = 0;
                                    // Count the running executors
                                    foreach (ProActiveRuntimeExecutor p in manager.getExecutors())
                                    {
                                        if (p.isStarted())
                                        {
                                            count++;
                                        }
                                    }
                                    // Write the count into the stream
                                    sw.WriteLine(count);

                                    // Wait for 1 sec
                                    Thread.Sleep(1 * 1000);
                                }

                                sw.WriteLine(0);
                            }
                        }
                        // Catch the IOException that is raised if the pipe is broken
                        // or disconnected.
                        catch(IOException) {
                            LOGGER.Info("The the GUI client has disconnected");
                        }
                        catch (Exception e)
                        {
                            LOGGER.Error("A problem occured when writing into the pipe", e);
                        }

                        try
                        {
                            // Close the server
                            pipeServer.Close();
                        }
                        catch (Exception e)
                        {
                            // Log me
                            LOGGER.Error("A problem occured during pipe close", e);
                        }
                    }
                }
            }

        }
    }
}
