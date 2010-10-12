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
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################
 * $$ACTIVEEON_INITIAL_DEV$$
 */
using System;
using System.Collections.Generic;
using ConfigParser;
using log4net;

namespace ProActiveAgent
{
    /// <summary>
    /// Common information collected at runtime and shared between multiple ProActive Runtime Executors
    /// </summary> 
    sealed class CommonStartInfo
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // Initialized by the WindowsService.main(), the directory will contain all logs file (runtime and executors logs)
        public static string logsDirectory;
        /// <summary>
        /// The configuration used to run the process</summary>
        private readonly AgentType _configuration;
        /// <summary>
        /// The selected connection</summary>
        private readonly ConnectionType _enabledConnection;
        /// <summary>
        /// All jvm parameters (default and user defined)</summary>
        private readonly string[] _jvmParameters;
        /// <summary>
        /// The log directory read from the registry</summary>
        // private readonly string _logsDirectory;

        /// <summary>
        /// The constructor of this class.</summary>
        public CommonStartInfo(AgentType configuration)
        {
            this._configuration = configuration;
            // Get the selected action, if no action is enabled it is an error
            this._enabledConnection = configuration.getEnabledConnection();
            if (this._enabledConnection == null)
            {
                LOGGER.Error("No selected action in the configuration. Exiting ...");
                Environment.Exit(0);
            }
            else
            {
                LOGGER.Info("Selected action " + this._enabledConnection.GetType().Name);
            }    

            // Prepare jvm parameters, cmd and args from the given action type

            // The list of jvm parameters
            List<string> jvmParametersList = new List<string>();

            // Add default parameters
            this._enabledConnection.fillDefaultJvmParameters(jvmParametersList, this._configuration.config.proactiveHome);
            
            // Init -Xms to default value unless it was explicitely specified by the user from the configuration
            string xms = "-Xms" + Constants.MINIMAL_REQUIRED_MEMORY + "M";
            if (this._configuration.config.jvmParameters != null)
            {
                // Append all params and check for overriden jvm memory parameters
                foreach (string s in this._configuration.config.jvmParameters)
                {
                    // Check if user specifically defined jvm memory params and override them with user values
                    if (s != null && s.Contains("-Xms"))
                    {
                        xms = s;
                        continue;
                    }
                    jvmParametersList.Add(s);
                }
            }
            jvmParametersList.Add(xms);

            this._jvmParameters = jvmParametersList.ToArray();
        }

        public AgentType configuration
        {
            get
            {
                return this._configuration;
            }
        }

        public ConnectionType enabledConnection
        {
            get
            {
                return this._enabledConnection;
            }
        }

        public string[] jvmParameters
        {
            get
            {
                return this._jvmParameters;
            }
        }

        public string starterClass
        {
            get
            {
                return this._enabledConnection.javaStarterClass;
            }
        }

        //public string logsDirectory {
        //    get
        //    {
        //        return this._logsDirectory;
        //    }
        //}
    }
}
