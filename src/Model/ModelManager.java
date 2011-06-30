/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Model;

import Connections.Connections;
import GUI.GUIEditorWindows;
import GUI.mainWindows;
import Planning.Events;
import Utils.FileLocator;
import Utils.XMLMaker;
import Utils.XMLParser;
import java.util.ArrayList;
import javax.swing.JOptionPane;


/**
 *
 * @author pgouttef
 */
public class ModelManager {
    
    /**
     * The Configs Files by default : XML & XSD
     */
    private static String XMLFileName = "PAAgent-config.xml";
    private static String XSDFileName = "agent-windows.xsd";

    /**
     * The XML builder and parser
     */
    private static XMLParser xmlParser = new XMLParser();
    private static XMLMaker xmlBuilder = new XMLMaker();
    
    /**
     * Variables of application's model
     */
    private static String PROACTIVEHOME = "";
    private static String JAVAHOME = "";
    private static String SCRIPTONEXIT = "";
    private static String PROTOCOL = "";
    private static int NBRUNTIMES = 0;
    private static int MEMORYLIMIT = 0;
    private static int MAXCPUUSAGE = 0;
    private static String PROCESSPRIORITY = "";
    
    /**
     * List of NetworkInterfaces
     */
    private static ArrayList<String> NETWORKINTERFACESLIST = null;
    
    /**
     * The Events structure:
     *          ArrayList<Event> : list of events in the Planning panel
     */
    private static Events EVENTS = new Events();
    
    /**
     * The Connections structure:
     *          
     */
    private static Connections CONNECTIONS = new Connections();
    
    /**
     * The JVM Options
     */
    private static ArrayList<String> JVMOPTIONS = new ArrayList<String> ();
    
    
    //GETTERS SETTERS
    public static String getJAVAHOME() { return JAVAHOME; }
    public static void setJAVAHOME(String JAVAHOME) { ModelManager.JAVAHOME = JAVAHOME; }
    public static String getPROACTIVEHOME() { return PROACTIVEHOME; }
    public static void setPROACTIVEHOME(String PROACTIVEHOME) { ModelManager.PROACTIVEHOME = PROACTIVEHOME; }
    public static String getXMLFileName() { return XMLFileName; }
    public static void setXMLFileName(String fileName) { ModelManager.XMLFileName = fileName; }
    public static String getXSDFileName() { return XSDFileName; }
    public static void setXSDFileName(String fileName) { ModelManager.XSDFileName = fileName; }
    public static String getSCRIPTONEXIT() { return SCRIPTONEXIT; }
    public static void setSCRIPTONEXIT(String SCRIPTONEXIT) { ModelManager.SCRIPTONEXIT = SCRIPTONEXIT; }
    public static Events getEVENTS() { return EVENTS; }
    public static void setEVENTS(Events EVENTS) { ModelManager.EVENTS = EVENTS; }
    public static int getMEMORYLIMIT() { return MEMORYLIMIT; }
    public static void setMEMORYLIMIT(int MEMORYLIMIT) { ModelManager.MEMORYLIMIT = MEMORYLIMIT; }
    public static int getNBRUNTIMES() { return NBRUNTIMES; }
    public static void setNBRUNTIMES(int NBRUNTIMES) { ModelManager.NBRUNTIMES = NBRUNTIMES; }
    public static String getPROTOCOL() { return PROTOCOL; }
    public static void setPROTOCOL(String PROTOCOL) { ModelManager.PROTOCOL = PROTOCOL; }
    public static Connections getCONNECTIONS() { return CONNECTIONS; }
    public static void setCONNECTIONS(Connections CONNECTIONS) { ModelManager.CONNECTIONS = CONNECTIONS; }
    public static String getPROCESSPRIORITY() { return PROCESSPRIORITY; }
    public static void setPROCESSPRIORITY(String PROCESSPRIORITY) { ModelManager.PROCESSPRIORITY = PROCESSPRIORITY; }
    public static int getMAXCPUUSAGE() { return MAXCPUUSAGE; }
    public static void setMAXCPUUSAGE(int MAXCPUUSAGE) { ModelManager.MAXCPUUSAGE = MAXCPUUSAGE; }
    public static ArrayList<String> getListInterfaces() { return NETWORKINTERFACESLIST; }
    public static void setListInterfaces(ArrayList<String> list) { ModelManager.NETWORKINTERFACESLIST = list; }
    public static ArrayList<String> getJVMOPTIONS() { return JVMOPTIONS; }
    public static void setJVMOPTIONS(ArrayList<String> JVMOPTIONS) { ModelManager.JVMOPTIONS = JVMOPTIONS; }
    
    /**
     * Try to locate default XML file at start
     */
    public static void init() {
        ModelManager.setXMLFileName( new FileLocator().getPathOfFile( ModelManager.getXMLFileName() ));
    }
    
    /**
     * Load xml file with validation
     */
    public static Boolean loadXML(mainWindows frame) {
        
        setJVMOPTIONS(  new ArrayList<String>() );
        
        if(xmlParser.loadDocument(XSDFileName,XMLFileName)) {
            return true;
        } else {
            JOptionPane.showMessageDialog(frame,"The XML file is not valid.");
            return false;
        }
    }
    
    /**
     * push all datas into default XML file
     */
    public static Boolean saveXML(GUIEditorWindows frame) {
        if(xmlBuilder.saveDocument(XMLFileName)) {
            JOptionPane.showMessageDialog(frame, "The file \"" + XMLFileName + "\" was saved.");
            return true;
        } else {
            JOptionPane.showMessageDialog(frame,"An error was encountered.");
            return false;
        }
    }
    
    /**
     * push all datas into a specific XML file
     * @param saveAsFile : XML file
     */
    public static Boolean saveXML(GUIEditorWindows frame, String saveAsFile) {
        if(xmlBuilder.saveDocument(saveAsFile)) {
            JOptionPane.showMessageDialog(frame,"The file \"" + saveAsFile + "\" was saved.");
            return true;
        } else {
            JOptionPane.showMessageDialog(frame,"An error was encountered.");
            return false;
        }
    }
    
    public static void main(String[] args)
    {
        ModelManager.init();
        System.out.println("File name : " + XMLFileName);
        ModelManager.loadXML(null);
        System.out.println("ProActive : " + PROACTIVEHOME);
    }

    public static int convertDayToInt(String day) {
        
        if(day.equals("Monday")) { return 0; }
        else if(day.equals("Tuesday")) { return 1; }
        else if(day.equals("Wednesday")) { return 2; }
        else if(day.equals("Thursday")) { return 3; }
        else if(day.equals("Friday")) { return 4; }
        else if(day.equals("Saturday")) { return 5; }
        else if(day.equals("Sunday")) { return 6; }
        
        return 0;
    } 
    
    public static String convertIntToDay(int day) {
        
        if(day == 0) { return "Monday"; }
        else if(day == 1) { return "Tuesday"; }
        else if(day == 2) { return "Wednesday"; }
        else if(day == 3) { return "Thursday"; }
        else if(day == 4) { return "Friday"; }
        else if(day == 5) { return "Saturday"; }
        else if(day == 6) { return "Sunday"; }
        
        return "Monday";
    }
    
    public static int convertPriorityToInt(String priority) {
        
        if(priority.equals("Idle")) { return 0; }
        else if(priority.equals("BelowNormal")) { return 1; }
        else if(priority.equals("Normal")) { return 2; }
        else if(priority.equals("AboveNormal")) { return 3; }
        else if(priority.equals("High")) { return 4; }
        else if(priority.equals("Realtime")) { return 5; }
        
        return 0;
    } 
    
    public static int convertProtocolToInt(String priority) {
        
        if(priority.equals("undefined")) { return 0; }
        else if(priority.equals("rmi")) { return 1; }
        else if(priority.equals("http")) { return 2; }
        else if(priority.equals("parm")) { return 3; }
        else if(priority.equals("pnp")) { return 4; }
        else if(priority.equals("pnps")) { return 5; }
        
        return 3;
    } 

    
}
