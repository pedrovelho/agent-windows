using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

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

        private string myJvmParams;

        [XmlElement("proactiveLocation", IsNullable = false)]
        public string proactiveLocation
        {
            get
            {
                return myProActiveLocation;
            }

            set
            {
                myProActiveLocation = value;
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

        [XmlElement("jvm_params", IsNullable = false)]
        public string jvmParams
        {
            get
            {
                return myJvmParams;
            }
            set
            {
                myJvmParams = value;
            }
        }
    }
}
