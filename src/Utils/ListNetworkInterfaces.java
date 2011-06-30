/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
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
 * @author pgouttef
 */
public class ListNetworkInterfaces 
{
    public static void main(String args[]) throws SocketException {
        System.out.println(getNetworkInterfacesList());            
    }

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