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

import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Enumeration;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author philippe Gouttefarde
 */
public class ListNetworkInterfaces 
{
    public static void main(String args[]) throws SocketException {
        System.out.println(getNetworkInterfacesList());            
    }

    /**
     * 
     * @return an ArrayList of the network interface of the current computer. 
     */
    public static ArrayList<String> getNetworkInterfacesList() {
        
        ArrayList<String> interfacesList = null;
        
        try {
            interfacesList = new ArrayList<String>();
            Enumeration<NetworkInterface> nets = NetworkInterface.getNetworkInterfaces();
            for (NetworkInterface netint : Collections.list(nets))
                interfacesList.add(getInterfaceInformation(netint)); 
            
            return interfacesList;
        } catch (SocketException ex) {
            Logger.getLogger(ListNetworkInterfaces.class.getName()).log(Level.SEVERE, null, ex);
        }
        return interfacesList;
    }
    
    /**
     * 
     * @param netint : id of the interface
     * @return the complete name of the interface
     * @throws SocketException 
     */
    public static String getInterfaceInformation(NetworkInterface netint) throws SocketException {               
        final StringBuilder netintDescription = new StringBuilder();
        netintDescription.append(netint.getName() + " ");
        final Enumeration<InetAddress> inetAddresses = netint.getInetAddresses();
        for (InetAddress inetAddress : Collections.list(inetAddresses)) {
            netintDescription.append(inetAddress + " ");
        }
        netintDescription.append(netint.getDisplayName());
        return netintDescription.toString();
     }
} 