using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/*
 * Action represents an activity that will be taken by ProActive Agent
 * in case of Event (Calendar Event, Idleness or ScreenSaver Launch
 * 
 */

namespace ConfigParser
{
    [XmlInclude(typeof(AdvertAction)),
    XmlInclude(typeof(RMAction)),    
    XmlInclude(typeof(CustomAction))
    ]
    public class Action
    {        
        private int myInitialRestartDelay;        
        private string myJavaStarterClass;
        private bool myIsEnabled;        

        [XmlElement("initialRestartDelay")]
        public int initialRestartDelay
        {
            get
            {
                return myInitialRestartDelay;
            }

            set
            {
                myInitialRestartDelay = value;
            }
        }

        [XmlElement("javaStarterClass")]
        public string javaStarterClass
        {
            get
            {
                return this.myJavaStarterClass;
            }

            set
            {
                this.myJavaStarterClass = value;
            }
        }

        [XmlElement("isEnabled")]
        public bool isEnabled
        {
            get
            {
                return this.myIsEnabled;
            }

            set
            {
                this.myIsEnabled = value;
            }
        }

        // Sub classes must override this class
        public virtual string[] getArgs() { return new string[0]; }        

        // Default jvm parameters needed for this type of action
        public static void addDefaultJvmParameters(List<string> jvmParameters, string proactiveLocation)
        {
            jvmParameters.Add("-Dproactive.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Dproactive.configuration=\"file:" + proactiveLocation + "\\config\\proactive\\ProActiveConfiguration.xml\"");
            jvmParameters.Add("-Djava.security.manager");
        }
    }
}