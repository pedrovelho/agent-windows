import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.Collections;
import java.util.Enumeration;

public class ListNetworkInterfaces 
{
    public static void main(String args[]) throws SocketException {
        Enumeration<NetworkInterface> nets = NetworkInterface.getNetworkInterfaces();
        for (NetworkInterface netint : Collections.list(nets))
            displayInterfaceInformation(netint);        
    }

    static void displayInterfaceInformation(NetworkInterface netint) throws SocketException {               
        final StringBuilder netintDescription = new StringBuilder();
        netintDescription.append(netint.getName() + " ");
        final Enumeration<InetAddress> inetAddresses = netint.getInetAddresses();
        for (InetAddress inetAddress : Collections.list(inetAddresses)) {
            netintDescription.append(inetAddress + " ");
        }
        netintDescription.append(netint.getDisplayName());
        System.out.println(netintDescription);
     }
}  