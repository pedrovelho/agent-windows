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
    XmlInclude(typeof(P2PAction)),
    XmlInclude(typeof(RMAction))]
    public class Action
    {
        private String myPriority;
        private int myInitialRestartDelay;

        [XmlAttribute("priority")]
        public String priority
        {
            get
            {
                return myPriority;
            }

            set
            {
                myPriority = value;
            }
        }

        [XmlAttribute("initialRestartDelay")]
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
    }
}
