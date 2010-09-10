using System.Xml;
using System.Xml.Schema;
using System.IO;
using System;
using ConfigParser;
using System.Diagnostics;
using System.Xml.Serialization;
using ConfigParserOLD;

namespace AgentFirstSetup
{
    //class TestProgram
    //{

    //    public static void Main0(string[] args)
    //    {
    //        Console.WriteLine("----------------> ");
    //        string xmlFile = @"C:\Program Files\ProActiveAgent\config\PAAgent-config-planning-day-only.xml";
    //        string xsd = @"C:\Program Files\ProActiveAgent";

    //        try
    //        {
    //            //ConfigParser.ConfigurationParser.internalValidate(xmlFile, xsd,"agent-windows.xsd");
    //            ConfigurationParser.validateXMLFile(xmlFile, xsd);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("----------------> " + e);
    //        }

    //    }

    //    private static void ValidationError(object sender, ValidationEventArgs arguments)
    //    {
    //        Console.WriteLine("Problem: " + arguments.Message);
    //    }

    //    private static string testSerialize(AgentType a)
    //    {
    //        string path = @"C:\vbodnart\SAVE\agent trunk\windows\utils\xml\testSerialized.xml";
    //        XmlSerializer serializer = new XmlSerializer(typeof(AgentType));
    //        TextWriter tw = new StreamWriter(path);
    //        serializer.Serialize(tw, a);
    //        tw.Close();
    //        return path;
    //    }


    //    // Parse given XML file
    //    // Result: Configuration object representing the contents of file

    //    public static bool validateXMLFile(String filePath)
    //    {
    //        String schemaPath = @"C:\vbodnart\SAVE\agent trunk\windows\utils\xml\agent-windows.xsd";
    //        bool valid = true;
    //        // Schema validation

    //        XmlSchemaSet schemaSet = new XmlSchemaSet();
    //        //schemaSet.Add(null, @"C:\vbodnart\SAVE\agent trunk\windows\utils\xml\agent-common.xsd");
    //        schemaSet.Add("urn:proactive:agent:windows:3.0", schemaPath);



    //        XmlReaderSettings settings = new XmlReaderSettings();
    //        settings.ValidationType = ValidationType.Schema;
    //        settings.Schemas = schemaSet;
    //        settings.ValidationEventHandler += new ValidationEventHandler(ValidationError);

    //        XmlReader textReader = XmlReader.Create(filePath, settings);
    //        try
    //        {
    //            while (textReader.Read()) ;
    //        }
    //        catch (XmlException e)
    //        {
    //            throw new ApplicationException("Could not read the " + filePath + " config file ", e);
    //        }

    //        textReader.Close();


    //        if (!valid)
    //            return false;

    //        return true;
    //    }


    //    /// <summary>
    //    ///
    //    /// </summary>
    //    /// <returns></returns>
    //    private static AgentType createAgentType()
    //    {
    //        AgentType t = new AgentType();

    //        AgentConfigType config = new AgentConfigType();
    //        config.proactiveHome = @"C:\vbodnart\workspace12\scheduling";
    //        config.javaHome = @"C:\Program Files\Java\jdk1.6.0_21";
    //        config.jvmParameters = new string[] { "-Dsome.prop=value" };
    //        config.memoryLimit = 500;
    //        config.nbRuntimes = 10;
    //        config.protocol = "rmi";
    //        config.portRange = new PortRange(1099, 1109);
    //        config.onRuntimeExitScript = @"C:\vbodnart\workspace12\scheduling\bin\windows\init.bat";
    //        config.processPriority = ProcessPriorityClass.Normal;
    //        config.maxCpuUsage = 100;
    //        // Add the config to the agent
    //        t.config = config;

    //        LocalBindConnectionType con1 = new LocalBindConnectionType();
    //        con1.javaStarterClass = "org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter";
    //        con1.nodename = "toto";
    //        con1.respawnIncrement = 10;
    //        ResoureManagerConnectionType con2 = new ResoureManagerConnectionType();
    //        con2.enabled = true;
    //        con2.javaStarterClass = "org.ow2.proactive.resourcemanager.utils.PAAgentServiceRMStarter";
    //        con2.nodename = "toto";
    //        con2.respawnIncrement = 10;
    //        con2.nodeSourceName = "WinAgents";
    //        con2.url = @"rmi://optimus.activeeon.com:8010/";
    //        CustomConnectionType con3 = new CustomConnectionType();
    //        con3.javaStarterClass = "org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter";
    //        con3.nodename = "toto";
    //        con3.respawnIncrement = 10;
    //        // Add connections to the agent
    //        t.connections = new ConnectionType[] { con1, con2, con3 };

    //        CalendarEventType ev1 = new CalendarEventType();
    //        ev1.start = new Start(DayOfWeek.Monday, 5, 30, 15);
    //        ev1.duration = new Duration(0, 2, 0, 0);
    //        ev1.config = null;
    //        // Add event
    //        t.events = new CalendarEventType[] { ev1 };

    //        return t;
    //    }
    //}
}