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
using System.Xml;
using System.Xml.Schema;
using System.IO;
using ConfigParserOLD;

namespace ConfigParser
{
    /// <summary>
    /// Class which objects are responsible
    /// for transformation from XML configuration file
    /// into internal object representation of configuration state.
    /// </summary>
    public sealed class ConfigurationParser
    {
        /// <summary>
        /// The namespace of config files.</summary>
        public const string CONFIG_NAMESPACE = "urn:proactive:agent:0.90:windows";

        private static string reason;

        public static void saveXml(String fileName, AgentType configuration)
        {            
            XmlSerializer serializer = new XmlSerializer(typeof(AgentType));
            TextWriter tw = new StreamWriter(fileName);
            serializer.Serialize(tw, configuration);
            tw.Close();
        }

        /// <summary>
        /// Throws Application exception if the file is invalid.
        /// </summary>
        /// <param name="filePath">The path to the configuration file</param>
        /// <param name="agentHome">The home dir of the agent</param>
        public static void validateXMLFile(string filePath, string agentHome)
        {
            // First try to validate against current version            
            try
            {                
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(CONFIG_NAMESPACE, agentHome + "\\xml\\agent-windows.xsd");
                internalValidate(filePath, agentHome, schemaSet);
            }
            catch (Exception e1)
            {
                // Try to validate against the older schema version (older than < 2.2)               
                try
                {                    
                    XmlSchemaSet schemaSet = new XmlSchemaSet();
                    schemaSet.Add(null, agentHome + "\\xml\\agent-old.xsd");
                    internalValidate(filePath, agentHome, schemaSet);
                }
                catch (Exception e2)
                {
                    throw new ApplicationException("Invalid configuration file. Against agent-windows.xsd: " + e1 + " Against agent-old.xsd: " + e2.Message);
                }
            }
        }
        
        /// <summary>
        /// Tries to parse and deserialize as AgentType with fallback to ConfigurationOLD
        /// </summary>
        /// <param name="filePath">The path to the configuration file</param>
        /// <param name="agentHome">The home dir of the agent</param>
        /// <returns></returns>
        public static AgentType parseXml(String filePath, string agentHome)
        {
            TextReader tr1 = new StreamReader(filePath);
            try
            {
                // Try to deserialize 
                XmlSerializer serializer = new XmlSerializer(typeof(AgentType));
                return (AgentType)serializer.Deserialize(tr1);
            }
            catch (Exception)
            {
                TextReader tr2 = new StreamReader(filePath);
                try
                {
                    // Try against older version
                    XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationOLD));
                    ConfigurationOLD oldConf = (ConfigurationOLD)serializer.Deserialize(tr2);
                    // Translate
                    return translateFromOLD(oldConf);
                }
                catch (Exception e2)
                {
                    throw e2;
                }
                finally
                {
                    tr2.Close();
                }
            }
            finally
            {
                tr1.Close();
            }
        }

        private static void internalValidate(string filePath, string agentHome, XmlSchemaSet schemaSet)
        {

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemaSet;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationError);

            XmlReader textReader = XmlReader.Create(filePath, settings);
            try
            {
                while (textReader.Read()) ;
            }
            catch (XmlException e)
            {
                throw new ApplicationException("Could not read the " + filePath + " config file ", e);
            }
            finally
            {
                textReader.Close();
            }
            try
            {
                // If something happened throw an exception
                if (reason != null)
                {
                    throw new ApplicationException("Unable to validate " + filePath + " is invalid: " + reason);
                }
            }
            finally
            {
                reason = null;
            }
        }

        private static void ValidationError(object sender, ValidationEventArgs arguments)
        {
            reason = arguments.Message;
        }

        private static AgentType translateFromOLD(ConfigurationOLD oldConf)
        {

            // Translate general configuration                    
            AgentConfigType config = new AgentConfigType();
            config.proactiveHome = oldConf.agentConfig.proactiveLocation;
            config.javaHome = oldConf.agentConfig.javaHome;
            config.jvmParameters = oldConf.agentConfig.jvmParameters;
            config.memoryLimit = 0; // Not translated
            config.nbRuntimes = (ushort)oldConf.agentConfig.nbProcesses;
            config.protocol = oldConf.agentConfig.runtimeIncomingProtocol.ToString();
            config.portRange.first = (ushort)oldConf.agentConfig.proActiveCommunicationPortInitialValue;
            config.portRange.last = (ushort)(config.portRange.first + config.nbRuntimes);
            config.onRuntimeExitScript = oldConf.agentConfig.onRuntimeExitScript;
            config.processPriority = System.Diagnostics.ProcessPriorityClass.Normal;
            config.maxCpuUsage = 100;

            // Translate events
            CalendarEventType[] events = new CalendarEventType[oldConf.events.Count];
            for (int i = 0; i < oldConf.events.Count; i++)
            {
                CalendarEvent oldEvent = oldConf.events[i];
                CalendarEventType newEvent = new CalendarEventType();

                string day = oldEvent.startDay;
                if (day.Equals("monday"))
                    newEvent.start.day = DayOfWeek.Monday;
                else if (day.Equals("tuesday"))
                    newEvent.start.day = DayOfWeek.Tuesday;
                else if (day.Equals("wednesday"))
                    newEvent.start.day = DayOfWeek.Wednesday;
                else if (day.Equals("thursday"))
                    newEvent.start.day = DayOfWeek.Thursday;
                else if (day.Equals("friday"))
                    newEvent.start.day = DayOfWeek.Friday;
                else if (day.Equals("saturday"))
                    newEvent.start.day = DayOfWeek.Saturday;
                else if (day.Equals("sunday"))
                    newEvent.start.day = DayOfWeek.Sunday;

                newEvent.start.hour = (ushort)oldEvent.startHour;
                newEvent.start.minute = (ushort)oldEvent.startMinute;
                newEvent.start.second = (ushort)oldEvent.startSecond;

                newEvent.duration.days = (ushort)oldEvent.durationDays;
                newEvent.duration.hours = (ushort)oldEvent.durationHours;
                newEvent.duration.minutes = (ushort)oldEvent.durationMinutes;
                newEvent.duration.seconds = (ushort)oldEvent.durationSeconds;

                newEvent.config.processPriority = oldEvent.processPriority;
                newEvent.config.maxCpuUsage = (ushort)oldEvent.maxCpuUsage;

                // Add the new event
                events[i] = newEvent;
            }

            // Translate connections
            ConnectionType[] connections = new ConnectionType[3];

            AdvertAction advertAction = (AdvertAction)oldConf.actions[0];
            LocalBindConnectionType localBind = new LocalBindConnectionType();
            if (advertAction != null)
            {
                localBind.respawnIncrement = 10; // TODO: See the default value
                localBind.javaStarterClass = advertAction.javaStarterClass;
                localBind.nodename = advertAction.nodeName;
                localBind.enabled = advertAction.isEnabled;
            }
            connections[0] = localBind;

            RMAction rmAction = (RMAction)oldConf.actions[1];
            ResoureManagerConnectionType rmConn = new ResoureManagerConnectionType();
            if (rmAction != null)
            {
                rmConn.respawnIncrement = 10; // TODO: See the default value
                rmConn.javaStarterClass = rmAction.javaStarterClass;
                rmConn.nodename = rmAction.nodeName;
                rmConn.enabled = rmAction.isEnabled; // TODO: See the default value                        
                rmConn.url = rmAction.url;
                rmConn.nodeSourceName = rmAction.nodeSourceName;
                if (!rmAction.useDefaultCredential)
                {
                    rmConn.credential = rmAction.credentialLocation;
                }
            }
            connections[1] = rmConn;

            CustomAction customAction = (CustomAction)oldConf.actions[2];
            CustomConnectionType customCon = new CustomConnectionType();
            if (customAction != null)
            {
                customCon.respawnIncrement = 10; // TODO: See the default value
                customCon.javaStarterClass = customAction.javaStarterClass;
                customCon.args = customAction.args;
                customCon.enabled = customAction.isEnabled;
            }
            connections[2] = customCon;

            // Create the new agent conf
            return new AgentType(config, events, connections);
        }
    }
}
