using System.Xml.Serialization;
using System;

/**
 * The configuration of runner script for launching services provided by ProActive
 * 
 * 
 * 
 */

namespace ConfigParser
{
    public class AgentConfig
    {
        private string myProActiveLocation;

        private string myJavaHome;

        private string[] myJvmParameters;

        private bool myEnableMemoryManagement;

        private uint myJavaMemory;

        private uint myNativeMemory;

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
    }
}
