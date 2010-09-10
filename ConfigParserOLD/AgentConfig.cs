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
using System.Xml.Serialization;

/**
 * Configuration related utilities.
 */
namespace ConfigParserOLD
{
    /// <summary>
    /// Enumeration type of ProActive Agent running type.
    /// </summary>
    public enum ProActiveCommunicationProtocol
    {
        undefined = 0,
        rmi = 1,
        http = 2
    }

    public class AgentConfig
    {
        private string myProActiveLocation;

        private string myJavaHome;

        private string[] myJvmParameters;

        private bool myEnableMemoryManagement;

        private uint myJavaMemory;

        private uint myNativeMemory;

        private int myNbProcesses;

        private bool myUseAllCPUs;

        private ProActiveCommunicationProtocol myRuntimeIncomingProtocol;

        private int myProActiveCommunicationPortInitialValue;

        private string myOnRuntimeExitScript;

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

        [XmlElement("nb_processes", IsNullable = false)]
        public int nbProcesses
        {
            get
            {
                return this.myNbProcesses;
            }
            set
            {
                this.myNbProcesses = value;
            }
        }

        [XmlElement("use_all_cpus", IsNullable = false)]
        public bool useAllCPUs
        {
            get
            {
                return this.myUseAllCPUs;
            }
            set
            {
                this.myUseAllCPUs = value;
            }
        }

        [XmlElement("protocol")]
        public ProActiveCommunicationProtocol runtimeIncomingProtocol
        {
            get
            {
                return this.myRuntimeIncomingProtocol;
            }

            set
            {
                this.myRuntimeIncomingProtocol = value;
            }
        }

        [XmlElement("port_initial_value")]
        public int proActiveCommunicationPortInitialValue
        {
            get
            {
                return this.myProActiveCommunicationPortInitialValue;
            }

            set
            {
                this.myProActiveCommunicationPortInitialValue = value;
            }
        }

        [XmlElement("onRuntimeExitScript")]
        public string onRuntimeExitScript
        {
            get
            {
                return this.myOnRuntimeExitScript;
            }

            set
            {
                this.myOnRuntimeExitScript = value;
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
