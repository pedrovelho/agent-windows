/*
 * ################################################################
 *
 * ProActive: The Java(TM) library for Parallel, Distributed,
 *            Concurrent computing with Security and Mobility
 *
 * Copyright (C) 1997-2010 INRIA/University of 
 *                                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
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
 * If needed, contact us to obtain a release under GPL Version 2 
 * or a different license than the GPL.
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

/**
 * AdvertAction is a description of advertisement action.
 * In this scenario a new ProActive node will be created and
 * advertised in RMI registry
 */

namespace ConfigParser
{

    public class AdvertAction : Action
    {
        /** The string description of this action **/
        public const string DESCRIPTION = "RMI Registration";
        /// <summary>
        /// The java class that corresponds to this action.</summary>
        public const string DEFAULT_JAVA_STARTER_CLASS = "org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter";

        // Name of the node to create
        // If the name is null, a default name will be used
        private string myNodeName;

        public AdvertAction()
        {
            base.javaStarterClass = DEFAULT_JAVA_STARTER_CLASS;
            this.myNodeName = "";
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

        public override string[] getArgs()
        {
            return new string[] { this.myNodeName };
        }

        // Default jvm parameters needed for this type of action
        public override void fillDefaultJvmParameters(List<string> jvmParameters, string proactiveLocation)
        {
            base.fillDefaultJvmParameters(jvmParameters, proactiveLocation);
            // Check 2 locations of the security policy file
            string location = proactiveLocation + "\\config\\security.java.policy";
            // ProActive Scheduling
            if (System.IO.File.Exists(location))
            {
                jvmParameters.Add("-Djava.security.policy=\"" + location + "\"");
            }
            // ProActive Programming
            else 
            {               
                jvmParameters.Add("-Djava.security.policy=\"" + proactiveLocation + "\\examples\\proactive.java.policy\"");
            }            
        }
    }
}