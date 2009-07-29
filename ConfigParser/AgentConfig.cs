using System;
using System.Xml.Serialization;

/**
 * The configuration of runner script for launching services provided by ProActive
 * 
 * 
 * 
 */

namespace ConfigParser
{
    /// <summary>
    /// Enumeration type of ProActive Agent running type.
    /// </summary>
    public enum ProActiveCommunicationProtocol
    {
        undefined = 0,
        rmi = 1,
        http = 2
    }

    public class AgentConfig
    {
        private string myProActiveLocation;

        private string myJavaHome;

        private string[] myJvmParameters;

        private bool myEnableMemoryManagement;

        private uint myJavaMemory;

        private uint myNativeMemory;

        private int myNbProcesses;

        private bool myUseAllCPUs;

        private ProActiveCommunicationProtocol myProActiveCommunicationProtocol;

        private int myProActiveCommunicationPortInitialValue;

        private string myOnRuntimeExitScript;

        // Not serialized used only during runtime
        private string myClasspath;

        [XmlElement("proactiveLocation", IsNullable = false)]
        public string proactiveLocation
        {
            get
            {
                return this.myProActiveLocation;
            }

            set
            {
                this.myProActiveLocation = value;
            }
        }

        [XmlElement("java_home", IsNullable = false)]
        public string javaHome
        {
            get
            {
                return myJavaHome;
            }
            set
            {
                myJavaHome = value;
            }
        }

        [XmlArray("jvm_parameters", IsNullable = false)]
        [XmlArrayItem("jvm_parameter", IsNullable = true)]
        public string[] jvmParameters
        {
            get
            {
                return this.myJvmParameters;
            }
            set
            {
                this.myJvmParameters = value;
            }
        }

        [XmlElement("enable_memory_management", IsNullable = false)]
        public bool enableMemoryManagement
        {
            get
            {
                return this.myEnableMemoryManagement;
            }
            set
            {
                this.myEnableMemoryManagement = value;
            }
        }

        [XmlElement("java_memory", IsNullable = false)]
        public uint javaMemory
        {
            get
            {
                return this.myJavaMemory;
            }
            set
            {
                this.myJavaMemory = value;
            }
        }

        [XmlElement("native_memory", IsNullable = false)]
        public uint nativeMemory
        {
            get
            {
                return this.myNativeMemory;
            }
            set
            {
                this.myNativeMemory = value;
            }
        }

        [XmlElement("nb_processes", IsNullable = false)] 
        public int nbProcesses
        {
            get
            {
                return this.myNbProcesses;
            }
            set
            {
                this.myNbProcesses = value;
            }
        }

        [XmlElement("use_all_cpus", IsNullable = false)] 
        public bool useAllCPUs
        {
            get
            {
                return this.myUseAllCPUs;
            }
            set
            {
                this.myUseAllCPUs = value;
            }
        }

        [XmlElement("protocol")]
        public ProActiveCommunicationProtocol proActiveCommunicationProtocol
        {
            get
            {
                return this.myProActiveCommunicationProtocol;
            }

            set
            {
                this.myProActiveCommunicationProtocol = value;
            }
        }

        [XmlElement("port_initial_value")]
        public int proActiveCommunicationPortInitialValue
        {
            get
            {
                return this.myProActiveCommunicationPortInitialValue;
            }

            set
            {
                this.myProActiveCommunicationPortInitialValue = value;
            }
        }

        [XmlElement("onRuntimeExitScript")]
        public string onRuntimeExitScript
        {
            get
            {
                return this.myOnRuntimeExitScript;
            }

            set
            {
                this.myOnRuntimeExitScript = value;
            }
        }

        [XmlIgnore]
        public string classpath
        {
            set
            {
                this.myClasspath = value;
            }
            get
            {
                return this.myClasspath;
            }
        }

        public static void Main(string[] args)
        {
        }
    }
}
