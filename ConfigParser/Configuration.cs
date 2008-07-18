using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * Object representation of ProActive Agent configuration 
 * Contains information about runner scripts, 
 * events and action to take
 */

namespace ConfigParser
{
    [XmlRoot("agent")]
    public class Configuration
    {
        // Configuration of runner script for launching ProActive
        
        private AgentConfig myAgentConfig;

        [XmlElement("internalConfig", IsNullable = false)]
        public AgentConfig agentConfig
        {
            get
            {
                return myAgentConfig;
            }

            set
            {
                myAgentConfig = value;
            }
        }

        // Collection of events that the system will react on

        private Events myEvents;

        [XmlElement("events", IsNullable = false)]
        public Events events
        {
            get
            {
                return myEvents;
            }

            set
            {
                myEvents = value;
            }
        }

        // Action that will be taken in case of any event triggered

        private Action myAction;

        [XmlElement("action", IsNullable = false)]
        public Action action
        {
            get
            {
                return myAction;
            }

            set
            {
                myAction = value;
            }
        }

    }
}
