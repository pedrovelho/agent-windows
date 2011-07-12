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
package Utils;

import Connections.Connections;
import Model.ModelManager;
import Planning.Config;
import Planning.Event;
import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.apache.crimson.tree.XmlDocument;
import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Element;

/**
 *
 * @author philippe Gouttefarde
 */
public class XMLMaker {
    
    //MAIN NODE NAME
    private String CONFIGNODENAME = "config";
    private String EVENTSNODENAME = "events";
    private String CONNECTIONSNODENAME = "connections";
    
    public static void main(String[] av) throws IOException {
        
        XMLMaker dw = new XMLMaker();
        dw.saveDocument("text.txt");
  }
    
    /**
     * Save all information in the XML config file.
     * @param fileName : XML config file
     */
    public Boolean saveDocument(String fileName) {
        try {
            //main method
            Document doc = makeDoc();

            //get a file writer
            FileWriter fstream = new FileWriter( fileName );
            BufferedWriter out = new BufferedWriter(fstream);
            
            if(doc == null) {return false;}
            
            ((XmlDocument)doc).write(out);
            return true;
        } catch (IOException ex) {
            Logger.getLogger(XMLMaker.class.getName()).log(Level.SEVERE, null, ex);
            return false;
        }
    }
    
    // CREATE THE ROOT ELEMENT : <agent>
    private Document createRootElement( Document doc ) {
        
        Element rootElement = doc.createElement("agent");
        Attr attrXSI = doc.createAttribute("xmlns:xsi");
        attrXSI.setValue("http://www.w3.org/2001/XMLSchema-instance");
        Attr attrXSD = doc.createAttribute("xmlns:xsd");
        attrXSD.setValue("http://www.w3.org/2001/XMLSchema");
        Attr attrXmlns = doc.createAttribute("xmlns");
        String os = System.getProperty("os.name").toLowerCase();
        if(os.indexOf( "win" ) >= 0) {
             attrXmlns.setValue("urn:proactive:agent:0.90:windows");
        } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {
             attrXmlns.setValue("urn:proactive:agent:0.90:linux");
        }
        
       
        
        rootElement.setAttributeNode(attrXSI);
        rootElement.setAttributeNode(attrXSD);
        rootElement.setAttributeNode(attrXmlns);
        
        doc.appendChild(rootElement);
        
        createConfigElement(doc, rootElement);
        createEventsElement(doc, rootElement);
        createConnectionNode(doc, rootElement);
        
        return doc;
    }
    
    // CREATE THE config's EVENTS ELEMENT : <event>
    //                                        <config>
    //                                      </event>
    private Document createEventConfigElement(Document doc, Element parentElement , int numEvent) {
    
        Element config = doc.createElement( "config" );
        parentElement.appendChild(config);
        Config conf = ModelManager.getEVENTS().get(numEvent).getConfig();
        
        if(!conf.getProActiveHome().equals("")){
          //ProActive Home element
	  Element proActiveHomeNode = doc.createElement("proactiveHome");
          proActiveHomeNode.appendChild(doc.createTextNode(conf.getProActiveHome()));
	  config.appendChild(proActiveHomeNode);
       }
        if(!conf.getJavaHome().equals("")){
          //JAVAHOME element
	  Element javaNode = doc.createElement("javaHome");
          javaNode.appendChild(doc.createTextNode(conf.getJavaHome()));
	  config.appendChild(javaNode);
       }
       if(conf.getMemoryLimit() >= 0){
          //Memory Limit element
	  Element memoryLimitNode = doc.createElement("memoryLimit");
          memoryLimitNode.appendChild(doc.createTextNode(ModelManager.getMEMORYLIMIT() + ""));
	  config.appendChild(memoryLimitNode);
       }
       if(conf.getNbRuntimes() >= 0){
          //Nb Runtimes element
	  Element runtimesNode = doc.createElement("nbRuntimes");
          runtimesNode.appendChild(doc.createTextNode(conf.getNbRuntimes() + ""));
	  config.appendChild(runtimesNode);
       }
       if(!conf.getProtocol().equals("")){
          //Script On Exit element
	  Element protocolNode = doc.createElement("protocol");
          protocolNode.appendChild(doc.createTextNode(conf.getProtocol()));
	  config.appendChild(protocolNode);
       }
       if(conf.getPortRangeFirst() >= 0 && conf.getPortRangeLast() >= 0){
          //Memory Limit element
	  Element portRangeNode = doc.createElement("portRange");
          portRangeNode.setAttribute("first", conf.getPortRangeFirst() + "");
          portRangeNode.setAttribute("last", conf.getPortRangeLast() + "");
	  config.appendChild(portRangeNode);
       }
       if(!ModelManager.getSCRIPTONEXIT().equals("")){
          //Script On Exit element
	  Element scriptOnExitNode = doc.createElement("onRuntimeExitScript");
          scriptOnExitNode.appendChild(doc.createTextNode(ModelManager.getSCRIPTONEXIT()));
	  config.appendChild(scriptOnExitNode);
       }
       //Specifics parameters for CPU
       String os = System.getProperty("os.name").toLowerCase();
       if(os.indexOf( "win" ) >= 0) {
          //processPriority element
          Element processPriorityNode = doc.createElement("processPriority");
          processPriorityNode.appendChild(doc.createTextNode(conf.getProcessPriority()));
          config.appendChild(processPriorityNode);
          //CpuUsage element
          Element maxCpuUsageNode = doc.createElement("maxCpuUsage");
          maxCpuUsageNode.appendChild(doc.createTextNode(conf.getCpuUsage() + ""));
          config.appendChild(maxCpuUsageNode);
       } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {  
          //processPriority element
          Element processPriorityNode = doc.createElement("nice");
          processPriorityNode.appendChild(doc.createTextNode(conf.getCpuUsage() + ""));
          config.appendChild(processPriorityNode);
           //CpuUsage element
	  Element portRangeNode = doc.createElement("ionice");
          portRangeNode.setAttribute("class", conf.getProcessPriority());
          portRangeNode.setAttribute("classdata", conf.getClassdata() + "");
	  config.appendChild(portRangeNode);
       }
        
        return doc;
    }
    
    
    // CREATE THE EVENTS ELEMENT : <events>
    private Document createEventsElement(Document doc, Element parentElement) {
    
        Element events = doc.createElement( EVENTSNODENAME );
        parentElement.appendChild(events);
        
        for (int i = 0 ; i < ModelManager.getEVENTS().getSize() ; i++) {
            //Event element
            Event ev = ModelManager.getEVENTS().get(i);
            Element evNode = doc.createElement("event");
            
            Element startNode = doc.createElement("start");
                      
            startNode.setAttribute("day", ev.getStartTime().getDay());
            startNode.setAttribute("hour", ev.getStartTime().getHour() + "");
            startNode.setAttribute("minute", ev.getStartTime().getMinute()+ "");
            startNode.setAttribute("second", ev.getStartTime().getSecond()+ "");
            
            Element durationNode = doc.createElement("duration");
                      
            durationNode.setAttribute("days", ev.getDuration().getDay() + "");
            durationNode.setAttribute("hours", ev.getDuration().getHour() + "");
            durationNode.setAttribute("minutes", ev.getDuration().getMinute()+ "");
            durationNode.setAttribute("seconds", ev.getDuration().getSecond()+ "");
            
            evNode.appendChild(startNode);
            evNode.appendChild(durationNode);
            
            createEventConfigElement(doc, evNode, i);
            
            events.appendChild(evNode);
        }
        
        return doc;
    }
    
    // CREATE THE CONFIG ELEMENT : <config>
    private Document createConfigElement(Document doc, Element parentElement) {
        
       Element config = doc.createElement( CONFIGNODENAME );
       parentElement.appendChild(config);
       
      //ProActive Home element
      Element proActiveHomeNode = doc.createElement("proactiveHome");
      proActiveHomeNode.appendChild(doc.createTextNode(ModelManager.getPROACTIVEHOME()));
      config.appendChild(proActiveHomeNode);
      //JAVAHOME element
      Element javaNode = doc.createElement("javaHome");
      javaNode.appendChild(doc.createTextNode(ModelManager.getJAVAHOME()));
      config.appendChild(javaNode);
       
      if(ModelManager.getJVMOPTIONS().size() > 0){
          //JVM Options element
	  Element jvmOptionLimit = doc.createElement("jvmParameters");
          
           for (String param : ModelManager.getJVMOPTIONS()) {
              Element paramNode = doc.createElement("param");
              paramNode.appendChild(doc.createTextNode(param));
              jvmOptionLimit.appendChild(paramNode);
           }
          
	  config.appendChild(jvmOptionLimit);
       }
       if(ModelManager.getMEMORYLIMIT() >= 0){
          //Memory Limit element
	  Element scriptMemoryLimit = doc.createElement("memoryLimit");
          scriptMemoryLimit.appendChild(doc.createTextNode(ModelManager.getMEMORYLIMIT() + ""));
	  config.appendChild(scriptMemoryLimit);
       }
       if(ModelManager.getNBRUNTIMES() >= 0){
          //Script On Exit element
	  Element nbRuntimesNode = doc.createElement("nbRuntimes");
          nbRuntimesNode.appendChild(doc.createTextNode(ModelManager.getNBRUNTIMES() + ""));
	  config.appendChild(nbRuntimesNode);
       }
       if(!ModelManager.getPROTOCOL().equals("")){
          //Script On Exit element
	  Element protocolNode = doc.createElement("protocol");
          protocolNode.appendChild(doc.createTextNode(ModelManager.getPROTOCOL()));
	  config.appendChild(protocolNode);
       }
       if(ModelManager.getPORTMIN() >= 0 && ModelManager.getPORTMAX() >= 0){
          //Memory Limit element
	  Element portRangeNode = doc.createElement("portRange");
          portRangeNode.setAttribute("first", ModelManager.getPORTMIN() + "");
          portRangeNode.setAttribute("last", ModelManager.getPORTMAX() + "");
	  config.appendChild(portRangeNode);
       }
       if(!ModelManager.getSCRIPTONEXIT().equals("")){
          //Script On Exit element
	  Element scriptOnExitNode = doc.createElement("onRuntimeExitScript");
          scriptOnExitNode.appendChild(doc.createTextNode(ModelManager.getSCRIPTONEXIT()));
	  config.appendChild(scriptOnExitNode);
       }
       //Specifics parameters for CPU
       String os = System.getProperty("os.name").toLowerCase();
       if(os.indexOf( "win" ) >= 0) {
          //processPriority element
          Element processPriorityNode = doc.createElement("processPriority");
          processPriorityNode.appendChild(doc.createTextNode(ModelManager.getPROCESSPRIORITY()));
          config.appendChild(processPriorityNode);
          //CpuUsage element
          Element maxCpuUsageNode = doc.createElement("maxCpuUsage");
          maxCpuUsageNode.appendChild(doc.createTextNode(ModelManager.getCPUUSAGE() + ""));
          config.appendChild(maxCpuUsageNode);
       } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {  
          //processPriority element
          Element processPriorityNode = doc.createElement("nice");
          processPriorityNode.appendChild(doc.createTextNode(ModelManager.getCPUUSAGE() + ""));
          config.appendChild(processPriorityNode);
           //CpuUsage element
	  Element portRangeNode = doc.createElement("ionice");
          portRangeNode.setAttribute("class", ModelManager.getPROCESSPRIORITY());
          portRangeNode.setAttribute("classdata", ModelManager.getCLASSDATA() + "");
	  config.appendChild(portRangeNode);
       }
       
       return doc;
    }

    private Document createLocalNode(Document doc, Element parentElement) {
        
        if(ModelManager.getCONNECTIONS().getLocal().getRespawnIncrement() >= 0 ){
          //ProActive Home element
	  Element respawn = doc.createElement("respawnIncrement");
          respawn.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getLocal().getRespawnIncrement() + ""));
	  parentElement.appendChild(respawn);
        }
        if(!ModelManager.getCONNECTIONS().getLocal().getJavaStarterClass().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("javaStarterClass");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getLocal().getJavaStarterClass()));
	  parentElement.appendChild(nodeName);
        }
        if(!ModelManager.getCONNECTIONS().getLocal().getNodeName().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("nodename");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getLocal().getNodeName()));
	  parentElement.appendChild(nodeName);
        }
        if(ModelManager.getCONNECTIONS().getSelected() == Connections.CONNTYPE.LOCAL) {
            parentElement.setAttribute("enabled", "true");
        } else {
            parentElement.setAttribute("enabled", "false");
        }
        
        return doc;
    }
    
    private Document createRMNode(Document doc, Element parentElement) {
        
        if(ModelManager.getCONNECTIONS().getRMConn().getRespawnIncrement() >= 0 ){
          //ProActive Home element
	  Element respawn = doc.createElement("respawnIncrement");
          respawn.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getRMConn().getRespawnIncrement() + ""));
	  parentElement.appendChild(respawn);
        }
        if(!ModelManager.getCONNECTIONS().getRMConn().getJavaStarterClass().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("javaStarterClass");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getRMConn().getJavaStarterClass()));
	  parentElement.appendChild(nodeName);
        }
        if(!ModelManager.getCONNECTIONS().getRMConn().getNodeName().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("nodename");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getRMConn().getNodeName()));
	  parentElement.appendChild(nodeName);
        }
        if(!ModelManager.getCONNECTIONS().getRMConn().getUrl().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("url");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getRMConn().getUrl()));
	  parentElement.appendChild(nodeName);
        }
        if(!ModelManager.getCONNECTIONS().getRMConn().getNodeSourceName().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("nodeSourceName");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getRMConn().getNodeSourceName()));
	  parentElement.appendChild(nodeName);
        }
        if(!ModelManager.getCONNECTIONS().getRMConn().getCredential().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("credential");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getRMConn().getCredential()));
	  parentElement.appendChild(nodeName);
        }
        if(ModelManager.getCONNECTIONS().getSelected() == Connections.CONNTYPE.RM) {
            parentElement.setAttribute("enabled", "true");
        } else {
            parentElement.setAttribute("enabled", "false");
        }
        
        return doc;
    }
    
    private Document createCustomNode(Document doc, Element parentElement) {
        
        if(ModelManager.getCONNECTIONS().getCustom().getRespawnIncrement() >= 0 ){
          //ProActive Home element
	  Element nodeRespawn = doc.createElement("respawnIncrement");
          nodeRespawn.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getCustom().getRespawnIncrement() + ""));
	  parentElement.appendChild(nodeRespawn);
        }
        if(!ModelManager.getCONNECTIONS().getCustom().getJavaStarterClass().equals("") ){
          //ProActive Home element
	  Element nodeName = doc.createElement("javaStarterClass");
          nodeName.appendChild(doc.createTextNode(ModelManager.getCONNECTIONS().getCustom().getJavaStarterClass()));
	  parentElement.appendChild(nodeName);
        }
        if(ModelManager.getCONNECTIONS().getCustom().getArgs().size() > 0) {
            Element nodeArgs = doc.createElement("args");
            parentElement.appendChild(nodeArgs);
            for (String arg : ModelManager.getCONNECTIONS().getCustom().getArgs()) {
                Element nodeArg = doc.createElement("arg");
                nodeArg.appendChild(doc.createTextNode(arg));
                nodeArgs.appendChild(nodeArg);
            }
        }
        if(ModelManager.getCONNECTIONS().getSelected() == Connections.CONNTYPE.CUSTOM) {
            parentElement.setAttribute("enabled", "true");
        } else {
            parentElement.setAttribute("enabled", "false");
        }
        
        return doc;
    }
    
    private Document createConnectionNode(Document doc, Element parentElement) {
        
        Element connection = doc.createElement( CONNECTIONSNODENAME );
        parentElement.appendChild(connection);
        
        /*
         * LOCAL NODE
         */
        Element localBindNode = doc.createElement("localBind");
        connection.appendChild(localBindNode);
        
        createLocalNode(doc, localBindNode);
        
        /*
         * RM NODE
         */
        Element RMBindNode = doc.createElement("rmConnection");
        connection.appendChild(RMBindNode);
        
        createRMNode(doc, RMBindNode);
        
        /*
         * CUSTOM NODE
         */
        Element customBindNode = doc.createElement("customConnection");
        connection.appendChild(customBindNode);
        
        createCustomNode(doc, customBindNode);
              
        return doc;
    }
    
  /** Generate the XML document */
  protected Document makeDoc() {
    try {
        // get doc variable
        DocumentBuilderFactory fact = DocumentBuilderFactory.newInstance();
        DocumentBuilder parser = fact.newDocumentBuilder();
        Document doc = parser.newDocument();
        
        
      // Start point of the conversion in XML format
      return createRootElement( doc );

    } catch (Exception ex) {
      System.err.println("+============================+");
      System.err.println("|        XML Error           |");
      System.err.println("+============================+");
      System.err.println(ex.getClass());
      System.err.println(ex.getMessage());
      System.err.println("+============================+");
      return null;
    }
  }
}
