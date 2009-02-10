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
