using System;
using System.Collections.Generic;
using System.Text;
using ProActiveAgent;
using System.ServiceProcess;
using System.Windows.Forms;

/**
 * This class is used to communicate with ProActive Agent system service
 * It is using ServiceController class to achieve this goal
 */

namespace ScreenSaver
{
    public class ServiceCommunicator
    {
        private readonly ServiceController sc = new ServiceController(Constants.PROACTIVE_AGENT_SERVICE_NAME);

        public void sendStartAction()
        {
            try
            {             
                sc.ExecuteCommand((int)PAACommands.ScreenSaverStart);                
            }
            catch (InvalidOperationException)
            {                
            }            
        }

        public void sendStopAction()
        {
            try
            {
                sc.ExecuteCommand((int)PAACommands.ScreenSaverStop);
            }
            catch (InvalidOperationException)
            {             
            }
        }

    }
}