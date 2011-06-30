/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Connections;

/**
 *
 * @author pgouttef
 */
public class RMConnection {
    
    private int respawnIncrement = 10;
    // Par defaut : 
    private String javaStarterClass = "org.ow2.proactive.resourcemanager.utils.RMNodeStarter";
    private String nodeName;
    private String nodeSourceName;
    private String url;
    private String credential;

    public String getCredential() {
        return credential;
    }

    public void setCredential(String credential) {
        this.credential = credential;
    }
    
    public String getNodeSourceName() {
        return nodeSourceName;
    }

    public void setNodeSourceName(String nodeSourceName) {
        this.nodeSourceName = nodeSourceName;
    }

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

    public String getUrl() {
        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }
}
