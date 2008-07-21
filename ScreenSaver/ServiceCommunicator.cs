using System;
using System.Collections.Generic;
using System.Text;
using ProActiveAgent;
using System.ServiceProcess;

/**
 * This class is used to communicate with ProActive Agent system service
 * It is using ServiceController class to achieve this goal
 */

namespace ScreenSaver
{
    public class ServiceCommunicator
    {
        private ServiceController sc = new ServiceController("ProActive Agent");

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