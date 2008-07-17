using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ProActiveAgent
{
    public enum MessagePriority
    {
        ServicePriority,
        ScreenSaverPriority
    }

    public class DaemonCommunicator
    {
        public static string REPLY_OK = "OK";

        public static string DAEMON_URL = "localhost";
        public static int PORT = 12345;

        private MessageBuilder msgBuilder = new MessageBuilder();
        
        public bool sendStartP2PSignal(MessagePriority priority, List<String> hosts)
        {
            // plain text temporary implementation of message
            String textToSend = msgBuilder.createMessage(MessageType.StartP2PMessage, priority, hosts);
            return doSendMessage(textToSend);
        }

        public bool sendStopP2PSignal(MessagePriority priority)
        {
            String textToSend = msgBuilder.createMessage(MessageType.StopP2PMessage, priority, null);
            return doSendMessage(textToSend);
        }

        public bool sendStartRMSignal(MessagePriority priority)
        {
            String textToSend = msgBuilder.createMessage(MessageType.RegisterRMMessage, priority, null);
            return doSendMessage(textToSend);
        }

        public bool sendStopRMSignal(MessagePriority priority)
        {
            String textToSend = msgBuilder.createMessage(MessageType.UnregisterRMMessage, priority, null);
            return doSendMessage(textToSend);
        }

        public bool sendKillSignal()
        {
            String textToSend = msgBuilder.createMessage(MessageType.KillMessage, MessagePriority.ServicePriority, null);
            return doSendMessage(textToSend);
        }

        private bool doSendMessage(String content)
        {
            try
            {
                TcpClient socket = new TcpClient(DAEMON_URL, PORT);
                NetworkStream nStream = socket.GetStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(nStream);
                System.IO.StreamWriter writer = new System.IO.StreamWriter(nStream);
                writer.WriteLine(content);
                writer.Flush();
                string reply = reader.ReadLine();
                nStream.Close();
                socket.Close();
                if (reply == null)
                {
                    Console.WriteLine("No reply from daemon!");
                    return false;
                }
                else if (reply.Equals(REPLY_OK))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (System.IO.IOException)
            {
                return false;
            }

        }
    }
}
