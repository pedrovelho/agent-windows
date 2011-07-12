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
package Planning;

/**
 *
 * @author philippe Gouttefarde
 */
public class Config {
    
    /**
     * Some configuration datas for an event
     */
    private String ProActiveHome = "";
    private String JavaHome = "";
    private String JVMParameters = "";
    private int memoryLimit = 0;
    private int nbRuntimes = 0;
    private String protocol = "";
    private int portRangeFirst = 0;
    private int portRangeLast = 65534;
    private String processPriority = "";
    private int cpuUsage = 1;
    private int classdata = 1;

    /*
     * getters and setters 
     */
    public int getClassdata() {
        return classdata;
    }

    public void setClassdata(int classdata) {
        this.classdata = classdata;
    }

    public String getJVMParameters() {
        return JVMParameters;
    }

    public void setJVMParameters(String JVMParameters) {
        this.JVMParameters = JVMParameters;
    }

    public String getJavaHome() {
        return JavaHome;
    }

    public void setJavaHome(String JavaHome) {
        this.JavaHome = JavaHome;
    }

    public String getProActiveHome() {
        return ProActiveHome;
    }

    public void setProActiveHome(String ProActiveHome) {
        this.ProActiveHome = ProActiveHome;
    }

    public String getProtocol() {
        return protocol;
    }

    public void setProtocol(String protocol) {
        this.protocol = protocol;
    }

    public int getCpuUsage() {
        return cpuUsage;
    }

    public void setCpuUsage(int maxCpuUsage) {
        this.cpuUsage = maxCpuUsage;
    }

    public int getMemoryLimit() {
        return memoryLimit;
    }

    public void setMemoryLimit(int memoryLimit) {
        this.memoryLimit = memoryLimit;
    }

    public int getNbRuntimes() {
        return nbRuntimes;
    }

    public void setNbRuntimes(int nbRuntimes) {
        this.nbRuntimes = nbRuntimes;
    }

    public int getPortRangeFirst() {
        return portRangeFirst;
    }

    public void setPortRangeFirst(int portRangeFirst) {
        this.portRangeFirst = portRangeFirst;
    }

    public int getPortRangeLast() {
        return portRangeLast;
    }

    public void setPortRangeLast(int portRangeLast) {
        this.portRangeLast = portRangeLast;
    }

    public String getProcessPriority() {
        return processPriority;
    }

    public void setProcessPriority(String processPriority) {
        this.processPriority = processPriority;
    }
    
    
}
