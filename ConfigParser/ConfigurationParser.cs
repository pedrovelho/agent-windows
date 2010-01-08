/*
* ################################################################
*
* ProActive: The Java(TM) library for Parallel, Distributed,
*            Concurrent computing with Security and Mobility
*
* Copyright (C) 1997-2009 INRIA/University of Nice-Sophia Antipolis
* Contact: proactive@ow2.org
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
* If needed, contact us to obtain a release under GPL Version 2. 
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
    }
}
