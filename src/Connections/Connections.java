/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Connections;

/**
 *
 * @author pgouttef
 */
public class Connections {
    
    public enum CONNTYPE{LOCAL , RM , CUSTOM };
    private CONNTYPE selected;
    
    private LocalConnection local = new LocalConnection();
    private RMConnection RMConn = new RMConnection();
    private CustomConnection custom = new CustomConnection();

    public RMConnection getRMConn() {
        return RMConn;
    }

    public void setRMConn(RMConnection RMConn) {
        this.RMConn = RMConn;
    }

    public CustomConnection getCustom() {
        return custom;
    }

    public void setCustom(CustomConnection custom) {
        this.custom = custom;
    }

    public LocalConnection getLocal() {
        return local;
    }

    public void setLocal(LocalConnection local) {
        this.local = local;
    }

    public CONNTYPE getSelected() {
        return selected;
    }

    public void setSelected(CONNTYPE selected) {
        this.selected = selected;
    }
}
