/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Planning;

/**
 *
 * @author pgouttef
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
    private int maxCpuUsage = 0;

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

    public int getMaxCpuUsage() {
        return maxCpuUsage;
    }

    public void setMaxCpuUsage(int maxCpuUsage) {
        this.maxCpuUsage = maxCpuUsage;
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
