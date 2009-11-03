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
* as published by the Free Software Foundation; either version
* 2 of the License, or any later version.
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
*  Initial developer(s):               The ProActive Team
*                        http://proactive.inria.fr/team_members.htm
*  Contributor(s): ActiveEon Team - http://www.activeeon.com
*
* ################################################################
* $$ACTIVEEON_CONTRIBUTOR$$
*/
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

        // Sub classes must override this method
        public virtual string[] getArgs() { return new string[0]; }

        // Subclasses may override this method for default jvm parameters needed for this type of action
        public virtual void fillDefaultJvmParameters(List<string> jvmParameters, string proactiveLocation)
        {
            jvmParameters.Add("-Dproactive.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Dproactive.configuration=\"file:" + proactiveLocation + "\\config\\proactive\\ProActiveConfiguration.xml\"");
            jvmParameters.Add("-Djava.security.manager");
        }
    }
}