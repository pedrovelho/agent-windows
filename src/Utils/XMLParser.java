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

import static org.w3c.dom.Node.ELEMENT_NODE;


/**
 *
 * @author philippe Gouttefarde
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
                doc = parserXML( XMLFile , XSDFile);

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
           }
           else if(nl.item(i).getNodeName().equals("onRuntimeExitScript")) {
               ModelManager.setSCRIPTONEXIT(getChildText(nl.item(i)));
           }
           
           //Specifics parameters for CPU
           String os = System.getProperty("os.name").toLowerCase();
           if(os.indexOf( "win" ) >= 0) {
        
               if(nl.item(i).getNodeName().equals("processPriority")) {
               ModelManager.setPROCESSPRIORITY(getChildText(nl.item(i)));
               }
               else if(nl.item(i).getNodeName().equals("maxCpuUsage")) {
                   ModelManager.setCPUUSAGE(Integer.parseInt(getChildText(nl.item(i))));
               }
               
           } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {  

               if(nl.item(i).getNodeName().equals("nice")) {
               ModelManager.setCPUUSAGE(Integer.parseInt(getChildText(nl.item(i))));
               }
               else if(nl.item(i).getNodeName().equals("ionice")) {
                   ModelManager.setPROCESSPRIORITY(
                           nl.item(i).getAttributes().getNamedItem("class").getNodeValue());
                   ModelManager.setCLASSDATA(Integer.parseInt(
                           nl.item(i).getAttributes().getNamedItem("classdata").getNodeValue()));
               }
               
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
                   
                   //Specifics parameters for CPU
                   String os = System.getProperty("os.name").toLowerCase();
                   if(os.indexOf( "win" ) >= 0) {
                       if(nl2.item(j).getNodeName().equals("processPriority")) {
                           conf.setProcessPriority(getChildText(nl2.item(j)));
                       }
                       else if(nl2.item(j).getNodeName().equals("maxCpuUsage")) {
                           conf.setCpuUsage(Integer.parseInt(getChildText(nl2.item(j))));
                       }

                   } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {  
                       if(nl2.item(j).getNodeName().equals("nice")) {
                           conf.setCpuUsage(Integer.parseInt(getChildText(nl2.item(j))));
                       }
                       else if(nl2.item(j).getNodeName().equals("ionice")) {
                           conf.setProcessPriority(
                                   nl2.item(j).getAttributes().getNamedItem("class").getNodeValue());
                           conf.setClassdata(Integer.parseInt(
                                   nl2.item(j).getAttributes().getNamedItem("classdata").getNodeValue()));
                       }

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

    public Boolean checkValidation(String XMLFile , String XSDFile) {
        try {
            Schema schema = compileSchema(XSDFile);
            Validator validator = schema.newValidator();
            validator.validate(new StreamSource(new File(XMLFile)));
        } catch (IOException ex) {
            System.out.println( ex.getMessage() );
            return false;
        } catch (SAXException ex) {
            System.out.println( ex.getMessage() );
            return false;
        }
        return true;
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
    public Document parserXML(String XMLFile , String XSDFile) 
        throws SAXException, IOException, ParserConfigurationException
    {
        Document document = null;
        
        if (checkValidation(XMLFile,XSDFile) ) {
            
            try {
            // parse an XML document into a DOM tree
            DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance(); 
            DocumentBuilder parser = dbf.newDocumentBuilder();
            document = parser.parse(new File(XMLFile));
            
            } catch (Exception ex) {
              System.out.println("validation with ERRORS :");
              System.out.println( ex.getMessage() );  
            }
        } else {
            System.out.println("validation with ERRORS :");
        }
        return document;
    }

    public static void main(String[] args)
    {
    }
}
