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
public class LocalConnection {
    
    /*
     * The reprensentation of a Local connection
     */
    
    private int respawnIncrement = 10;
    // By default : 
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
