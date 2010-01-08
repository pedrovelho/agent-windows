/*
* ################################################################
*
* ProActive: The Java(TM) library for Parallel, Distributed,
*            Concurrent computing with Security and Mobility
*
* Copyright (C) 1997-2009 INRIA/University of Nice-Sophia Antipolis
* Contact: proactive@ow2.org
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
* If needed, contact us to obtain a release under GPL Version 2. 
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
using ConfigParser;
using log4net;

namespace ProActiveAgent
{
    /** Common information collected at runtime and shared between multiple ProActive Runtime Executors */
    public class CommonStartInfo
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const uint DEFAULT_JAVA_HEAP_SIZE_IN_MBYTES = 64; // for the -Xms jvm option
        private const uint MINIMAL_REQUIRED_MEMORY_FOR_PROACTIVE_RUNTIME_PROCESS_IN_MBYTES = 32;
        /// <summary>
        /// The configuration used to run the process.</summary>
        private readonly Configuration _configuration;
        /// <summary>
        /// The selected action.</summary>
        private readonly ConfigParser.Action _selectedAction;
        /// <summary>
        /// All jvm parameters (default and user defined).</summary>
        private readonly string[] _jvmParameters;
        /// <summary>
        /// The starter class name.</summary>
        private readonly string _cmd;
        /// <summary>
        /// The maximum java heap size for the ProActive Runtime process.</summary>
        private readonly uint _maximumJavaHeapSize;
        /// <summary>
        /// The total memory limit to be applied to the job object.</summary>
        private readonly uint _memoryLimit;

        /// <summary>
        /// The constructor of this class.</summary>
        public CommonStartInfo(Configuration configuration)
        {
            this._configuration = configuration;
            // Get the selected action, if no action is selected it is an error
            this._selectedAction = configuration.getSelectedAction();
            if (this._selectedAction == null)
            {
                LOGGER.Error("No selected action in the configuration. Exiting ...");
                Environment.Exit(0);
            }
            else
            {
                LOGGER.Info("Selected action " + this._selectedAction.GetType().Name);
            }

            this._maximumJavaHeapSize = DEFAULT_JAVA_HEAP_SIZE_IN_MBYTES;
            // If memory management is enabled
            if (this._configuration.agentConfig.enableMemoryManagement)
            {
                // Add user defined memory limitations
                this._maximumJavaHeapSize += this._configuration.agentConfig.javaMemory;
                this._memoryLimit = this._maximumJavaHeapSize + this._configuration.agentConfig.nativeMemory + MINIMAL_REQUIRED_MEMORY_FOR_PROACTIVE_RUNTIME_PROCESS_IN_MBYTES;
            }

            // Prepare jvm parameters, cmd and args from the given action type

            // All jvm parameters
            List<string> jvmParametersList = new List<string>();

            // Add default parameters            
            this._selectedAction.fillDefaultJvmParameters(jvmParametersList, this.configuration.agentConfig.proactiveLocation);

            this._cmd = this._selectedAction.javaStarterClass;

            // Add user defined jvm parameters
            this.addUserDefinedJvmParameters(jvmParametersList);
            this._jvmParameters = jvmParametersList.ToArray();
        }

        public Configuration configuration
        {
            get
            {
                return this._configuration;
            }
        }

        public ConfigParser.Action selectedAction
        {
            get
            {
                return this._selectedAction;
            }
        }

        public string[] jvmParameters
        {
            get
            {
                return this._jvmParameters;
            }
        }

        public string cmd
        {
            get
            {
                return this._cmd;
            }
        }

        public uint maximumJavaHeapSize
        {
            get
            {
                return this._maximumJavaHeapSize;
            }
        }

        public uint memoryLimit
        {
            get
            {
                return this._memoryLimit;
            }
        }

        private void addUserDefinedJvmParameters(List<string> jvmParameters)
        {
            // Add all user defined jvm parameters             
            // Init -Xms and -Xmx jvm paremeters to default values from the configuration
            string xms = "-Xms" + DEFAULT_JAVA_HEAP_SIZE_IN_MBYTES + "M";
            string xmx = "-Xmx" + this._maximumJavaHeapSize + "M";
            // Append all params and check for overriden jvm memory parameters
            foreach (string s in this._configuration.agentConfig.jvmParameters)
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
    }
}
