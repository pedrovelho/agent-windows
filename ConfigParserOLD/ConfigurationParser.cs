/*
 * ################################################################
 *
 * ProActive Parallel Suite(TM): The Java(TM) library for
 *    Parallel, Distributed, Multi-Core Computing for
 *    Enterprise Grids & Clouds
 *
 * Copyright (C) 1997-2011 INRIA/University of
 *                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 *
 * If needed, contact us to obtain a release under GPL Version 2 or 3
 * or a different license than the AGPL.
 *
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

/**
 * Class which objects are responsible
 * for transformation from XML configuration file
 * into internal object representation of configuration state
 */

namespace ConfigParserOLD
{
    public class ConfigurationParserOLD
    {
        private static bool valid = true;
        private static string reason = "";

        public static void saveXml(String fileName, ConfigurationOLD configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationOLD));
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


        public static ConfigurationOLD parseXml(String configFilePath, string proActiveAgentDir)
        {
            // String xmlSchemaFilePath = proActiveAgentDir + "\\config.xsd";          
            if (!validateXMLFile(configFilePath, proActiveAgentDir))
                throw new ApplicationException("Reason: " + reason);
            // Deserialization

            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationOLD));            
            ConfigurationOLD res;
            TextReader tr = new StreamReader(configFilePath);
            res = (ConfigurationOLD)serializer.Deserialize(tr);
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
