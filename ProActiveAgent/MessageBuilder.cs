using System;
using System.Collections.Generic;
using System.Text;

namespace ProActiveAgent
{
    public enum MessageType {
        StartP2PMessage,
        StopP2PMessage,
        RegisterRMMessage,
        UnregisterRMMessage,
        KillMessage
    }
     
    class MessageBuilder
    {

        public String createMessage(MessageType type, MessagePriority priority, object payLoad)
        {
            switch (type)
            {
                case MessageType.StartP2PMessage:
                    return buildP2PStartMsg(priority, payLoad);
                    //break
                case MessageType.StopP2PMessage:
                    return "P2P_STOP;" + priority.ToString();
                    //break
                case MessageType.RegisterRMMessage:
                    return "RM_REG;" + priority.ToString();
                    //break
                case MessageType.UnregisterRMMessage:
                    return "RM_UNREG;" + priority.ToString();
                    //break
                case MessageType.KillMessage:
                    return "KILL;" + priority.ToString();
                    //break
            }
            return "";
        }

        private String buildP2PStartMsg(MessagePriority priority, object payLoad)
        {
            StringBuilder bld = new StringBuilder();
            bld.Append("P2P_START;");
            bld.Append(priority.ToString());
            List<String> hosts = (List<String>)payLoad;
            foreach (String host in hosts)
            {
                bld.Append(";" + host);
            }
            return bld.ToString();
        }

    }
}
