using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * Action that registers the desktop machine
 * (specifically ProActive node created on that machine)
 * in Resource Manager instance
 * 
 * It is configured by specifying URL of Resource Manager instance
 * for example rmi://cheypa.inria.fr:1099
 */

namespace ConfigParser
{
    public class RMAction : Action
    {
        /** The string description of this action **/
        public const string DESCRIPTION = "Resource Manager Registration";
        /// <summary>
        /// The java class that corresponds to this action.</summary>
        public const string DEFAULT_JAVA_STARTER_CLASS = "org.ow2.proactive.resourcemanager.utils.PAAgentServiceRMStarter";

        private string myUrl;
        private string myNodeName;

        public RMAction() {
            base.javaStarterClass = DEFAULT_JAVA_STARTER_CLASS;
            this.myUrl = "";
            this.myNodeName = "";
        }

        [XmlElement("url", IsNullable = false)]
        public string url
        {
            get
            {
                return myUrl;
            }
            set
            {
                myUrl = value;
            }
        }


        [XmlElement("nodeName", IsNullable = false)]
        public string nodeName
        {
            get
            {
                return myNodeName;
            }
            set
            {
                myNodeName = value;
            }
        }

        // Default jvm parameters needed for this type of action
        public static new void addDefaultJvmParameters(List<string> jvmParameters, string proactiveLocation) {
            jvmParameters.Add("-Dpa.scheduler.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Dpa.rm.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Djava.security.policy=\"" + proactiveLocation + "\\config\\scheduler.java.policy\"");
            jvmParameters.Add("-Dlog4j.configuration=\"file:///" + proactiveLocation + "\\config\\scheduler-log4j\"");
        }
    }
}
