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
