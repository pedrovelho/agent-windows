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
package Connections;

/**
 *
 * @author philippe Gouttefarde
 */
public class Connections {
    
    /*
     * The main representation of the connections parameters. One instance for each connection with
     * an enum to get the selected one.
     */
    
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
