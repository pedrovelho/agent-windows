/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Utils;

//import javax.xml.validation.SchemaFactory;
//import javax.xml.XMLConstants;
//import javax.xml.parsers.DocumentBuilder;
//import javax.xml.validation.Schema;
//import java.io.File;
//import java.io.IOException;
//import javax.xml.parsers.DocumentBuilderFactory;
//import javax.xml.parsers.ParserConfigurationException;
//import org.w3c.dom.Document;
//import org.w3c.dom.Node;
//import org.w3c.dom.NodeList;
//import org.xml.sax.SAXException;


import javax.xml.transform.stream.StreamSource;
import javax.xml.validation.Validator;
import Connections.Connections.CONNTYPE;
import Model.ModelManager;
import Planning.Config;
import Planning.Duration;
import Planning.PlanningTime;
import Planning.Event;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import java.io.IOException;
import org.xml.sax.SAXException;
import javax.xml.validation.SchemaFactory;
import javax.xml.XMLConstants;
import java.io.File;
import java.util.ArrayList;
import org.w3c.dom.Document;
import javax.xml.validation.Schema;
import org.w3c.dom.NodeList;
import org.w3c.dom.Node;

import static org.w3c.dom.Node.ATTRIBUTE_NODE;
import static org.w3c.dom.Node.CDATA_SECTION_NODE;
import static org.w3c.dom.Node.COMMENT_NODE;
import static org.w3c.dom.Node.DOCUMENT_TYPE_NODE;
import static org.w3c.dom.Node.ELEMENT_NODE;
import static org.w3c.dom.Node.ENTITY_NODE;
import static org.w3c.dom.Node.ENTITY_REFERENCE_NODE;
import static org.w3c.dom.Node.NOTATION_NODE;
import static org.w3c.dom.Node.PROCESSING_INSTRUCTION_NODE;
import static org.w3c.dom.Node.TEXT_NODE;


/**
 *
 * @author pgouttef
 */
public class XMLParser {
    
    
    private Document doc = null;
    
    /**
     * Start point to load file with associate XSD
     * @param XSDFile
     * @param XMLFile 
     */
    public Boolean loadDocument(String XSDFile, String XMLFile) {
        
        try
        {       //Parsing method with validation schema
                doc = parserXML(new File( XMLFile ) , XSDFile);

                if(doc != null) {
                    System.out.println("validation of " + XSDFile + " [OK]");
                    
                    //If the document is valid, generate Model 
                    visitRootElement(doc.getFirstChild());
                    return true;
                } else {
                    System.out.println("validation of " + XSDFile + " [FAILED]");
                    return false;
                }
        }
        catch(Exception error)
        {
                error.printStackTrace();
                return false;
        }
    }
    
    /**
     * return 
     * @param node
     * @return 
     */
    private String getChildText(Node node) {

        Node child = node.getFirstChild();

        if(child == null) return "";

        return child.getNodeValue();
    }

    /**
     * 
     * @param node : the GlobalAgentConfigType Node
     */
    private void visitConfigNode(Node node) {

        NodeList nl = node.getChildNodes();
        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
           if(nl.item(i).getNodeName().equals("proactiveHome")) {
               ModelManager.setPROACTIVEHOME(getChildText(nl.item(i)));
           } 
           else if(nl.item(i).getNodeName().equals("javaHome")) {
               ModelManager.setJAVAHOME(getChildText(nl.item(i)));
           }
           else if(nl.item(i).getNodeName().equals("jvmParameters")) {
               
                NodeList nlist = nl.item(i).getChildNodes();
                for(int j=0, cnt2=nlist.getLength(); j<cnt2; j++)
                {
                    if( nlist.item(j).getNodeName().equals("param") ) {
                        ModelManager.getJVMOPTIONS().add(getChildText(nlist.item(j)));
                    }
                }
           }
           else if(nl.item(i).getNodeName().equals("memoryLimit")) {
               ModelManager.setMEMORYLIMIT( Integer.parseInt(getChildText(nl.item(i)) ));
           }
           else if(nl.item(i).getNodeName().equals("nbRuntimes")) {
               ModelManager.setNBRUNTIMES(Integer.parseInt(getChildText(nl.item(i))));
           }
           else if(nl.item(i).getNodeName().equals("protocol")) {
               ModelManager.setPROTOCOL(getChildText(nl.item(i)));
           }
           else if(nl.item(i).getNodeName().equals("portRange")) {
               System.out.print("Noeud PortRange : ");
               System.out.print("(first)" + nl.item(i).getAttributes().getNamedItem("first").getNodeValue());
               System.out.println("(last)" + nl.item(i).getAttributes().getNamedItem("last").getNodeValue());
           }
           else if(nl.item(i).getNodeName().equals("onRuntimeExitScript")) {
               ModelManager.setSCRIPTONEXIT(getChildText(nl.item(i)));
           }
           else if(nl.item(i).getNodeName().equals("processPriority")) {
               ModelManager.setPROCESSPRIORITY(getChildText(nl.item(i)));
           }
           else if(nl.item(i).getNodeName().equals("maxCpuUsage")) {
               ModelManager.setMAXCPUUSAGE(Integer.parseInt(getChildText(nl.item(i))));
           }
        }
    }

    /**
     * 
     * @param node : Event Node 
     */
    private void visitSingleEventNode(Node node) {

        PlanningTime startTime = null;
        Duration duration = null;
        Config conf = null;
        
        NodeList nl = node.getChildNodes();
        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
           if(nl.item(i).getNodeName().equals("start")) {
               //StartTime Planning
               startTime = new PlanningTime(
                    nl.item(i).getAttributes().getNamedItem("day").getNodeValue(),
                    Integer.parseInt(nl.item(i).getAttributes().getNamedItem("hour").getNodeValue()),
                    Integer.parseInt(nl.item(i).getAttributes().getNamedItem("minute").getNodeValue()),
                    Integer.parseInt(nl.item(i).getAttributes().getNamedItem("second").getNodeValue())
                );
               
           } 
           else if(nl.item(i).getNodeName().equals("duration")) {
               //Duration Planning
               duration = new Duration(
                    Integer.parseInt(nl.item(i).getAttributes().getNamedItem("days").getNodeValue()),
                    Integer.parseInt(nl.item(i).getAttributes().getNamedItem("hours").getNodeValue()),
                    Integer.parseInt(nl.item(i).getAttributes().getNamedItem("minutes").getNodeValue()),
                    Integer.parseInt(nl.item(i).getAttributes().getNamedItem("seconds").getNodeValue())
                );
           
           }
           else if(nl.item(i).getNodeName().equals("config")) {
               //Configurations
               conf = new Config();
               
               NodeList nl2 = nl.item(i).getChildNodes();
               for(int j=0, cnt2=nl2.getLength(); j<cnt2; j++)
               {
                   if(nl2.item(j).getNodeName().equals("proactiveHome")) {
                        conf.setProActiveHome( getChildText(nl2.item(j)) );
                    }
                   else if(nl2.item(j).getNodeName().equals("javaHome")) {
                        conf.setJavaHome( getChildText(nl2.item(j)) );
                    }
                   else if(nl2.item(j).getNodeName().equals("memoryLimit")) {
                        conf.setMemoryLimit( Integer.parseInt(getChildText(nl2.item(j))) );
                    }
                   else if(nl2.item(j).getNodeName().equals("nbRuntimes")) {
                        conf.setNbRuntimes(Integer.parseInt(getChildText(nl2.item(j))));
                    }
                   else if(nl2.item(j).getNodeName().equals("protocol")) {
                        conf.setProtocol(getChildText(nl2.item(j)));
                    }
                   else if(nl2.item(j).getNodeName().equals("portRange")) {
                       conf.setPortRangeFirst(Integer.parseInt(nl2.item(j).getAttributes().getNamedItem("first").getNodeValue()));
                       conf.setPortRangeLast(Integer.parseInt(nl2.item(j).getAttributes().getNamedItem("last").getNodeValue()));
                   }
                   else if(nl2.item(j).getNodeName().equals("processPriority")) {
                        conf.setProcessPriority(getChildText(nl2.item(j)));
                    }
                   else if(nl2.item(j).getNodeName().equals("maxCpuUsage")) {
                        conf.setMaxCpuUsage(Integer.parseInt(getChildText(nl2.item(j))));
                    }
                }
            }
        }
        
        Event ev = new Event(startTime, duration, conf);
        ModelManager.getEVENTS().addEvent(ev);
    }

    /**
     * Browse events list
     * @param node : the Events Node 
     */
    private void visitEventsNode(Node node) {

        ModelManager.getEVENTS().freeEventsList();
        
        NodeList nl = node.getChildNodes();
        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
            if( nl.item(i).getNodeType() == ELEMENT_NODE ) {
                visitSingleEventNode(nl.item(i));
            }
        }
    }

    /**
     * 
     * @param node : the LocalBind Node
     */
    private void visitLocalConnectionNode(Node node) {

        if(node.getAttributes().getNamedItem("enabled").getNodeValue().equals("true")) {
            ModelManager.getCONNECTIONS().setSelected(CONNTYPE.LOCAL);
        }

        NodeList nl = node.getChildNodes();
        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
            if(nl.item(i).getNodeName().equals("respawnIncrement")) {
                ModelManager.getCONNECTIONS().getLocal().setRespawnIncrement(
                        Integer.parseInt(getChildText(nl.item(i))));
            }
            else if(nl.item(i).getNodeName().equals("javaStarterClass")) {
                ModelManager.getCONNECTIONS().getLocal().setJavaStarterClass(
                        getChildText(nl.item(i)) );
            }
            else if(nl.item(i).getNodeName().equals("nodename")) {
                ModelManager.getCONNECTIONS().getLocal().setNodeName(
                        getChildText(nl.item(i)) );
            }
        }
    }

    /**
     * 
     * @param node : the rmConnection Node
     */
    private void visitRMConnectionNode(Node node) {

        if(node.getAttributes().getNamedItem("enabled").getNodeValue().equals("true")) {
            ModelManager.getCONNECTIONS().setSelected(CONNTYPE.RM);
        }

        NodeList nl = node.getChildNodes();
        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
            if(nl.item(i).getNodeName().equals("respawnIncrement")) {
                ModelManager.getCONNECTIONS().getRMConn().setRespawnIncrement(
                        Integer.parseInt(getChildText(nl.item(i))));
            }
            else if(nl.item(i).getNodeName().equals("javaStarterClass")) {
                ModelManager.getCONNECTIONS().getRMConn().setJavaStarterClass(
                        getChildText(nl.item(i)) );
            }
            else if(nl.item(i).getNodeName().equals("nodename")) {
                ModelManager.getCONNECTIONS().getRMConn().setNodeName(
                        getChildText(nl.item(i)) );
            }
            else if(nl.item(i).getNodeName().equals("url")) {
                ModelManager.getCONNECTIONS().getRMConn().setUrl(
                        getChildText(nl.item(i)) );
            }
            else if(nl.item(i).getNodeName().equals("nodeSourceName")) {
                ModelManager.getCONNECTIONS().getRMConn().setNodeSourceName(
                        getChildText(nl.item(i)) );
            }
            else if(nl.item(i).getNodeName().equals("credential")) {
                ModelManager.getCONNECTIONS().getRMConn().setCredential(
                        getChildText(nl.item(i)) );
            }
        }
    }

    /**
     * 
     * @param node : the customConnection Node
     */
    private void visitCustomConnectionNode(Node node) {

        if(node.getAttributes().getNamedItem("enabled").getNodeValue().equals("true")) {
            ModelManager.getCONNECTIONS().setSelected(CONNTYPE.CUSTOM);
        }

        NodeList nl = node.getChildNodes();
        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
            if(nl.item(i).getNodeName().equals("respawnIncrement")) {
                ModelManager.getCONNECTIONS().getCustom().setRespawnIncrement(
                        Integer.parseInt(getChildText(nl.item(i))));
            }
            else if(nl.item(i).getNodeName().equals("javaStarterClass")) {
                ModelManager.getCONNECTIONS().getCustom().setJavaStarterClass(
                        getChildText(nl.item(i)) );
            }
            else if(nl.item(i).getNodeName().equals("nodename")) {
                ModelManager.getCONNECTIONS().getCustom().setNodeName(
                        getChildText(nl.item(i)) );
            }
            else if(nl.item(i).getNodeName().equals("nodename")) {
                
                //Configurations
               ArrayList<String> args = new ArrayList<String>();
               
               NodeList nl2 = nl.item(i).getChildNodes();
               for(int j=0, cnt2=nl2.getLength(); j<cnt2; j++)
               {
                   if(nl2.item(j).getNodeName().equals("arg")) {
                        args.add( getChildText(nl2.item(j)) );
                    }
                }
               
               ModelManager.getCONNECTIONS().getCustom().setArgs(args);
            }
        }
    }

    /**
     * 
     * @param node : the Connection Node
     */
    private void visitConnectionsNode(Node node) {

        NodeList nl = node.getChildNodes();
        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
            if(nl.item(i).getNodeName().equals("localBind")) {
                visitLocalConnectionNode(nl.item(i));
            }
            else if(nl.item(i).getNodeName().equals("rmConnection")) {
                visitRMConnectionNode(nl.item(i));
            }
            else if(nl.item(i).getNodeName().equals("customConnection")) {
                visitCustomConnectionNode(nl.item(i));
            }
        }
    }

    /**
     * 
     * @param node : 1st node of XML file
     */
    private void visitRootElement(Node node) {

        NodeList nl = node.getChildNodes();

        for(int i=0, cnt=nl.getLength(); i<cnt; i++)
        {
            if(nl.item(i).getNodeName().equals("config")){
                visitConfigNode(nl.item(i));
            }
            else if(nl.item(i).getNodeName().equals("events")){
                visitEventsNode(nl.item(i));
            }
            else if(nl.item(i).getNodeName().equals("connections")){
                visitConnectionsNode(nl.item(i));
            }
        }
    }
   
    /**
     * Convert the XSD file to Schema variable 
     * @param XSDFileName : the name of XSD file
     * @return 
     */
    public static Schema compileSchema(String XSDFileName) throws SAXException {
        // Get the SchemaFactory instance which understands W3C XML Schema language
        SchemaFactory sf = SchemaFactory
                .newInstance(XMLConstants.W3C_XML_SCHEMA_NS_URI);
        return sf.newSchema(new File(XSDFileName));
    }

    /**
     * 
     * @param XMLFile : the XML file
     * @param XSDFile : the XSD file name witch get XMl descriptor
     * @return : the document if XML valid, else null
     * @throws SAXException
     * @throws IOException
     * @throws ParserConfigurationException 
     */
    public Document parserXML(File XMLFile , String XSDFile) 
        throws SAXException, IOException, ParserConfigurationException
    {
        Schema schema = compileSchema(XSDFile);
        
        Document document = null;
        
        try {
            Validator validator = schema.newValidator();
            validator.validate(new StreamSource(XMLFile));
            
            // parse an XML document into a DOM tree
            DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance(); 
            DocumentBuilder parser = dbf.newDocumentBuilder();
            document = parser.parse(XMLFile);
            
        } catch (Exception ex) {
            System.out.println("validation with ERRORS :");
            System.out.println( ex.getMessage() );
        }

        return document;
    }

    public static void main(String[] args)
    {
    }
}
