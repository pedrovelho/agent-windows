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
            ConfigParser.Action.addDefaultJvmParameters(jvmParametersList, this._configuration.agentConfig.proactiveLocation);

            this._cmd = this._selectedAction.javaStarterClass;

            // user defined jvm parameters will be added after in order to let the user redefine default parameters                        
            if (this._selectedAction is AdvertAction)
            {
                AdvertAction advertAction = (AdvertAction)this._selectedAction;
                // Add action specific default jvm parameters
                // ... nothing to add
                // Add user defined jvm parameters
                this.addUserDefinedJvmParameters(jvmParametersList);
                this._jvmParameters = jvmParametersList.ToArray();
            }
            else if (this._selectedAction is RMAction)
            {
                RMAction rmAction = (RMAction)this._selectedAction;
                // Add action specific default jvm parameters
                RMAction.addDefaultJvmParameters(jvmParametersList, this.configuration.agentConfig.proactiveLocation);
                // Add user defined jvm parameters
                this.addUserDefinedJvmParameters(jvmParametersList);
                this._jvmParameters = jvmParametersList.ToArray();
            }
            else if (this._selectedAction is CustomAction)
            {
                CustomAction customAction = (CustomAction)this._selectedAction;
                // Add action specific default jvm parameters
                // ... nothing to add
                // Add user defined jvm parameters
                this.addUserDefinedJvmParameters(jvmParametersList);
                this._jvmParameters = jvmParametersList.ToArray();
            }
            else
            {
                // Unknown action
            }
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
