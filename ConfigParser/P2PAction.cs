using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ConfigParser
{
    /**
     * In P2PAction a machine registers itself in P2P network
     * The configuration contains a list of first-contact
     * nodes that P2P service will be initialized with
     * 
     * !! NOT AVAILABLE !!
     */
    public class P2PAction : Action
    {
        /// <summary>
        /// The string description of this action.</summary>
        public const string DESCRIPTION = "Peer-to-peer";
        /// <summary>
        /// The java class that corresponds to this action.</summary>
        public const string DEFAULT_JAVA_STARTER_CLASS = "org.objectweb.proactive.extra.p2p.daemon.PAAgentServiceP2PStarter";
        /// <summary>
        /// The default protocol.</summary>
        public const string DEFAULT_P2P_PROTOCOL = "RMI";

        private String myProtocol;
        private String[] myContacts;

        public P2PAction()
        {
            this.myProtocol = DEFAULT_P2P_PROTOCOL;
            this.myContacts = new string[0];
        }

        [XmlElement("protocol")]
        public String protocol
        {
            get
            {
                return myProtocol;
            }
            set
            {
                myProtocol = value;
            }
        }

        [XmlArray("contacts", IsNullable = false)]
        [XmlArrayItem("contact", IsNullable = true)]
        public String[] contacts
        {
            get
            {
                return this.myContacts;
            }
            set
            {
                this.myContacts = value;
            }
        }
    }
}
