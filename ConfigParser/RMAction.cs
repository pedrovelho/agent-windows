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
using System.Collections.Generic;
using System.Xml.Serialization;
using System;

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
        /// <summary>
        /// The string description of this action.</summary>        
        public const string DESCRIPTION = "Resource Manager Registration";
        /// <summary>
        /// The java class that corresponds to this action.</summary>
        public const string DEFAULT_JAVA_STARTER_CLASS = "org.ow2.proactive.resourcemanager.utils.PAAgentServiceRMStarter";
        private string myRmUrl;
        private string myNodeName;
        private string myNodeSourceName;
        private string myCredentialLocation;
        private bool myUseDefaultCredential;

        public RMAction()
        {
            base.javaStarterClass = DEFAULT_JAVA_STARTER_CLASS;
            this.myRmUrl = "";
            this.myNodeName = "";
            this.myNodeSourceName = "";
            this.myCredentialLocation = "";
            this.myUseDefaultCredential = true;
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

        [XmlElement("nodeSourceName", IsNullable = false)]
        public string nodeSourceName
        {
            get
            {
                return this.myNodeSourceName;
            }
            set
            {
                this.myNodeSourceName = value;
            }
        }

        [XmlElement("credentialLocation", IsNullable = false)]
        public string credentialLocation
        {
            get
            {
                return this.myCredentialLocation;
            }
            set
            {
                this.myCredentialLocation = value;
            }
        }

        [XmlElement("useDefaultCredential", IsNullable = false)]
        public bool useDefaultCredential
        {
            get
            {
                return this.myUseDefaultCredential;
            }
            set
            {
                this.myUseDefaultCredential = value;
            }
        }

        public override string[] getArgs()
        {
            string urlOpt = "-r " + this.myRmUrl;
            string nodeNameOpt = "";
            if (this.myNodeName != null && !this.myNodeName.Equals(""))
            {
                nodeNameOpt = "-n " + this.myNodeName;
            }

            string nodeSourceNameOpt = "";
            if (this.myNodeSourceName != null && !this.myNodeSourceName.Equals(""))
            {
                nodeSourceNameOpt = "-s " + this.myNodeSourceName;
            }

            if (this.myUseDefaultCredential)
            {
                return new string[] { urlOpt, nodeNameOpt, nodeSourceNameOpt };
            }
            else
            {
                string credentialLocationOpt = "";
                if (this.myCredentialLocation != null && !this.myCredentialLocation.Equals(""))
                {
                    credentialLocationOpt = "-f \"" + this.myCredentialLocation + "\"";
                }

                return new string[] { urlOpt, nodeNameOpt, nodeSourceNameOpt, credentialLocationOpt };
            }
        }

        // Default jvm parameters needed for this type of action
        public override void fillDefaultJvmParameters(List<string> jvmParameters, string proactiveLocation)
        {
            base.fillDefaultJvmParameters(jvmParameters,proactiveLocation);
            jvmParameters.Add("-Dpa.scheduler.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Dpa.rm.home=\"" + proactiveLocation + "\"");
            jvmParameters.Add("-Djava.security.policy=\"" + proactiveLocation + "\\config\\security.java.policy-client\"");
            jvmParameters.Add("-Dlog4j.configuration=\"file:///" + proactiveLocation + "\\config\\log4j\\log4j-client\"");
        }        
    }
}
