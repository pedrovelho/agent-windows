/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Connections;

/**
 *
 * @author pgouttef
 */
public class LocalConnection {
    
    private int respawnIncrement = 10;
    // Par defaut : 
    private String javaStarterClass = "org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter";
    private String nodeName;

    public String getJavaStarterClass() {
        return javaStarterClass;
    }

    public void setJavaStarterClass(String javaStarterClass) {
        this.javaStarterClass = javaStarterClass;
    }

    public String getNodeName() {
        return nodeName;
    }

    public void setNodeName(String nodeName) {
        this.nodeName = nodeName;
    }

    public int getRespawnIncrement() {
        return respawnIncrement;
    }

    public void setRespawnIncrement(int respawnIncrement) {
        this.respawnIncrement = respawnIncrement;
    }
    
    
    
}
