/*
 * ################################################################
 *
 * ProActive Parallel Suite(TM): The Java(TM) library for
 *    Parallel, Distributed, Multi-Core Computing for
 *    Enterprise Grids & Clouds
 *
 * Copyright (C) 1997-2011 INRIA/University of
 *                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 *
 * If needed, contact us to obtain a release under GPL Version 2 or 3
 * or a different license than the AGPL.
 *
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################  
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
using System.Collections.Generic;
using System.Xml.Serialization;

/*
 * Action represents an activity that will be taken by ProActive Agent
 * in case of Event (Calendar Event, Idleness or ScreenSaver Launch
 * 
 */

namespace ConfigParserOLD
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