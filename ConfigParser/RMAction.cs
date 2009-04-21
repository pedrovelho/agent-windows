using System.Collections.Generic;
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
        /// <summary>
        /// The username used in case of anonymous authetication.</summary>
        public const string ANONYMOUS_USERNAME = "anonymous";
        /// <summary>
        /// The password used in case of anonymous authetication.</summary>
        public const string ANONYMOUS_PASSWORD = "anonymous";

        private string myUsername;
        private string myPassword;
        private string myRmUrl;
        private string myNodeName;

        public RMAction() {
            base.javaStarterClass = DEFAULT_JAVA_STARTER_CLASS;
            this.myUsername = "";
            this.myPassword = "";
            this.myRmUrl = "";
            this.myNodeName = "";
        }

        [XmlElement("username", IsNullable = false)]
        public string username
        {
            get
            {
                return this.myUsername;
            }
            set
            {
                this.myUsername = value;
            }
        }

        [XmlElement("password", IsNullable = false)]
        public string password
        {
            get
            {
                return this.myPassword;
            }
            set
            {
                this.myPassword = value;
            }
        }

        [XmlElement("url", IsNullable = false)]
        public string url
        {
            get
            {
                return this.myRmUrl;
            }
            set
            {
                this.myRmUrl = value;
            }
        }

        [XmlElement("nodeName", IsNullable = false)]
        public string nodeName
        {
            get
            {
                return this.myNodeName;
            }
            set
            {
                this.myNodeName = value;
            }
        }

        public override string[] getArgs(int processRank)
        {
            return new string[] { this.myUsername, this.myPassword, this.myRmUrl, this.myNodeName+processRank };
        }

        // Default jvm parameters needed for this type of action
        public static new void addDefaultJvmParameters(List<string> jvmParameters, string proactiveLocation) {
            jvmParameters.Add("-Dpa.scheduler.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Dpa.rm.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Djava.security.policy=\"" + proactiveLocation + "\\config\\scheduler.java.policy\"");
            jvmParameters.Add("-Dlog4j.configuration=\"file:///" + proactiveLocation + "\\config\\log4j\\log4j-client\"");
        }
    }
}
