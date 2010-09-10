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
 * Object representation of ProActive Agent configuration 
 * Contains information about runner scripts, 
 * events and action to take
 */
namespace ConfigParserOLD
{
    [XmlRoot("agent")]
    public class ConfigurationOLD
    {
        // Configuration of runner script for launching ProActive        
        private AgentConfig _agentConfig;        
        /// <summary>
        /// Actions that will be taken in case of any event triggered.</summary>
        private Action[] _actions;        
        /// <summary>
        /// Collection of events that the system will react on.</summary>
        private List<CalendarEvent> _events;
        /// <summary>
        /// The location where the agent is installed. This attribute is not serialized !</summary>
        private string _agentInstallLocation;

        // Public constructor
        public ConfigurationOLD(){
            this._events = new List<CalendarEvent>();
        }

        [XmlElement("internalConfig", IsNullable = false)]
        public AgentConfig agentConfig
        {
            get
            {
                return this._agentConfig;
            }

            set
            {
                this._agentConfig = value;
            }
        }
                
        [XmlArray("events", IsNullable = false )]
        [XmlArrayItem("event", typeof(CalendarEvent), IsNullable = true)]
        public List<CalendarEvent> events
        {
            get
            {
                return this._events;
            }

            set
            {
                this._events = value;
            }
        }
        
        [XmlArray("actions", IsNullable = false)]
        [XmlArrayItem("action", IsNullable = true)]
        public Action[] actions
        {
            get
            {
                return this._actions;
            }

            set
            {
                this._actions = value;
            }
        }

        public string agentInstallLocation
        {
            get
            {
                return this._agentInstallLocation;
            }

            set
            {
                this._agentInstallLocation = value;
            }
        }

        public Action getSelectedAction()
        {
            // Find enabled action, ONLY ONE ACTION CAN BE ENABLED
            Action enabledAction = null;
            foreach (Action action in this.actions)
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
