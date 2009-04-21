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

        // Actions that will be taken in case of any event triggered
        private Action[] myActions;

        // Collection of events that the system will react on        
        private List<Event> myEvents;

        // Public constructor
        public Configuration(){
            this.myEvents = new List<Event>();
        }

        [XmlElement("internalConfig", IsNullable = false)]
        public AgentConfig agentConfig
        {
            get
            {
                return this.myAgentConfig;
            }

            set
            {
                this.myAgentConfig = value;
            }
        }
                
        [XmlArray("events", IsNullable = false )]
        [XmlArrayItem("event", typeof(Event), IsNullable = true)]
        public List<Event> events
        {
            get
            {
                return this.myEvents;
            }

            set
            {
                this.myEvents = value;
            }
        }
        
        [XmlArray("actions", IsNullable = false)]
        [XmlArrayItem("action", IsNullable = true)]
        public Action[] actions
        {
            get
            {
                return this.myActions;
            }

            set
            {
                this.myActions = value;
            }
        }

        public ConfigParser.Action getSelectedAction()
        {
            // Find enabled action, ONLY ONE ACTION CAN BE ENABLED
            ConfigParser.Action enabledAction = null;
            foreach (ConfigParser.Action action in this.actions)
            {
                if (action.isEnabled)
                {
                    enabledAction = action;
                    break;
                }
            }
            return enabledAction;
        }
    }
}
