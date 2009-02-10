using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.IO;

/**
 * Class which objects are responsible
 * for transformation from XML configuration file
 * into internal object representation of configuration state
 */

namespace ConfigParser
{
    public class ConfigurationParser
    {
        private static bool valid = true;
        private static string reason = "";

        public static void saveXml(String fileName, Configuration configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            TextWriter tw = new StreamWriter(fileName);
            serializer.Serialize(tw, configuration);
            tw.Close();
        }
        
        // Parse given XML file
        // Result: Configuration object representing the contents of file

        public static bool validateXMLFile(String filePath, String agentHomePath)
        {
            String schemaPath = agentHomePath + "\\config.xsd"; 
            valid = true;
            // Schema validation

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, schemaPath);


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

            textReader.Close();


            if (!valid)
                return false;

            return true;
        }

        
        public static Configuration parseXml(String configFilePath, string proActiveAgentDir)
        {
           // String xmlSchemaFilePath = proActiveAgentDir + "\\config.xsd";          
            if (!validateXMLFile(configFilePath, proActiveAgentDir))
                throw new ApplicationException("Reason: " + reason);
            // Deserialization

            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            Configuration res;
            TextReader tr = new StreamReader(configFilePath);
            res = (Configuration)serializer.Deserialize(tr);
            tr.Close();
            return res;
        }

        private static void ValidationError(object sender, ValidationEventArgs arguments)
        {
            reason = arguments.Message; // Display error
            valid = false; //validation failed
        }


        public static Configuration generateSampleConf()
        {
            /*Configuration toSave = new Configuration();
            toSave.agentConfig = new AgentConfig();
            toSave.agentConfig.javaHome = "";
            toSave.agentConfig.jvmParams = "-Dtomek = krol";
            toSave.agentConfig.proactiveLocation = "d:\\tdobek\\tdobek\\proactive-trunk";
            toSave.events = new Events();
            CalendarEvent cEvent = new CalendarEvent();
            cEvent.durationDays = 0;
            cEvent.durationHours = 0;
            cEvent.durationMinutes = 30;
            cEvent.durationSeconds = 0;
            cEvent.startDay = "friday";
            cEvent.startHour = 10;
            cEvent.startMinute = 30;
            cEvent.startSecond = 0;
            toSave.events.events = new Event[] { cEvent };
            P2PAction p2pAction = new P2PAction();
            p2pAction.contacts = new string[] { "rmi://localhost:9876", "rmi://cheypa:1099" };
            p2pAction.priority = "Idle";
            p2pAction.protocol = "RMI";
            toSave.action = p2pAction;
            return toSave;*/
            return null;
        }

/*            Configuration res = new Configuration();
            
            // Parser initialization
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            XmlNodeList xmlNodeList;
            

            // analyzing list of events
            xmlNodeList = xmlDoc.SelectNodes("/agent/events/event");

            Events events = new Events();
            
            // for each event :
            foreach (XmlNode xmlNode in xmlNodeList) {
                Event anEvent = null;
                String type = xmlNode.Attributes.GetNamedItem("type").Value;
                // idleness event
                if (type.Equals("idleness")) {
                    IdlenessEvent idleEvent = new IdlenessEvent();
                    idleEvent.beginSecs = Convert.ToInt32(xmlNode.Attributes.GetNamedItem("beginSecs").Value);
                    idleEvent.endSecs = Convert.ToInt32(xmlNode.Attributes.GetNamedItem("endSecs").Value);
                    idleEvent.beginThreshold = Convert.ToInt32(xmlNode.Attributes.GetNamedItem("beginThreshold").Value);
                    idleEvent.endThreshold = Convert.ToInt32(xmlNode.Attributes.GetNamedItem("endThreshold").Value);                    
                    anEvent = idleEvent;
                // calendar event
                } else if (type.Equals("calendar")) {
                    CalendarEvent calEvent = new CalendarEvent();
                    
                    XmlNode startNode = xmlNode.ChildNodes.Item(0);
                    calEvent.startDay = startNode.Attributes.GetNamedItem("day").Value;
                    calEvent.startHour = Convert.ToInt32(startNode.Attributes.GetNamedItem("hour").Value);
                    calEvent.startMinute = Convert.ToInt32(startNode.Attributes.GetNamedItem("minutes").Value);
                    calEvent.startSecond = Convert.ToInt32(startNode.Attributes.GetNamedItem("seconds").Value);
                    
                    XmlNode durationNode = xmlNode.ChildNodes.Item(1);
                    calEvent.durationDays = Convert.ToInt32(durationNode.Attributes.GetNamedItem("days").Value);
                    calEvent.durationHours = Convert.ToInt32(durationNode.Attributes.GetNamedItem("hours").Value);
                    calEvent.durationMinutes = Convert.ToInt32(durationNode.Attributes.GetNamedItem("minutes").Value);
                    calEvent.durationSeconds = Convert.ToInt32(durationNode.Attributes.GetNamedItem("seconds").Value);
                    
                    anEvent = calEvent;
                }
                events.addEvent(anEvent);
            }

            res.events = events;

            // now we analyze action 
            Action action = new Action();

            XmlNode actionNode = xmlDoc.SelectSingleNode("/agent/action");
            if (actionNode == null)
                throw new IncorrectConfigurationException();

            XmlNode someAction = actionNode.FirstChild;
            if (someAction == null)
                throw new IncorrectConfigurationException();
            // p2p action
            if (someAction.Name.Equals("p2pAction")) {
                P2PAction p2pAction = new P2PAction();
                XmlNode hostList = someAction.FirstChild;
                if (hostList == null)
                    throw new IncorrectConfigurationException();
                foreach (XmlNode hostNode in hostList.ChildNodes) {
                    p2pAction.addContact(hostNode.InnerText);
                }
                action = p2pAction;
            // Resource Manager Registration action
            } else if (someAction.Name.Equals("rmRegistration")) {
                RMAction rmAction = new RMAction();
                XmlNode attrib = someAction.Attributes.GetNamedItem("url");
                if (attrib == null)
                    throw new IncorrectConfigurationException();
                rmAction.setUrl(attrib.Value);
                action = rmAction;
            }
            // resource advertisement action
            else if (someAction.Name.Equals("advert"))
            {
                AdvertAction advertAction = new AdvertAction();
                XmlNode node = someAction.Attributes.GetNamedItem("nodeName");
                if (node != null)
                {
                    advertAction.setNodeName(node.Value);
                }   
                action = advertAction;
            }

            XmlNode actionPriority = actionNode.Attributes.GetNamedItem("priority");
            if (actionPriority != null)
                action.priority = actionPriority.Value;
            else
                action.priority = "";

            res.action = action;

            // runner script configuration

            AgentConfig agentConfig = new AgentConfig();

            XmlNode proActiveConfig = xmlDoc.SelectSingleNode("/agent/proactive");
            if (proActiveConfig == null)
                throw new IncorrectConfigurationException();
            XmlNode pALocation = proActiveConfig.Attributes.GetNamedItem("location");
            if (pALocation == null)
                throw new IncorrectConfigurationException();
            agentConfig.setProActiveLocation(pALocation.Value);

            XmlNode javaHome = xmlDoc.SelectSingleNode("/agent/jre/java_home");
            if (javaHome == null)
                agentConfig.setJavaHome("");
            else
                agentConfig.setJavaHome(javaHome.Value);

            XmlNode javaParams = xmlDoc.SelectSingleNode("/agent/jre/jvm_params");
            if (javaParams == null)
                agentConfig.setJvmParams("");
            else
                agentConfig.setJvmParams(javaParams.Value);

            res.agentConfig = agentConfig;

            return res; 

        } */


    }
}
