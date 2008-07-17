using System;
using System.Collections.Generic;
using System.Text;
using ConfigParser;

namespace ProActiveAgent
{
    public class ActionExecuter
    {
        
        //private DaemonCommunicator communicator = new DaemonCommunicator();

        public void sendStartAction(object whatToDo)
        {
            if (whatToDo is P2PAction)
            {
                P2PAction action = (P2PAction)whatToDo;
                communicator.sendStartP2PSignal(MessagePriority.ServicePriority, action.getContacts());
            }
            else if (whatToDo is RMAction)
            {
                communicator.sendStartRMSignal(MessagePriority.ServicePriority);
            }
        }

        public void sendStopAction(object whatToDo)
        {
            if (whatToDo is P2PAction)
            {
                communicator.sendStopP2PSignal(MessagePriority.ServicePriority);
            }
            else if (whatToDo is RMAction)
            {
                communicator.sendStopRMSignal(MessagePriority.ServicePriority);
            }

        }

        // KILL THE DAEMON PROCESS

        public void sendKillAction()
        {
            communicator.sendKillSignal();
        }

    }
}
