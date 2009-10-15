/*
* ################################################################
*
* ProActive: The Java(TM) library for Parallel, Distributed,
*            Concurrent computing with Security and Mobility
*
* Copyright (C) 1997-2009 INRIA/University of Nice-Sophia Antipolis
* Contact: proactive@ow2.org
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either version
* 2 of the License, or any later version.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this library; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
*
*  Initial developer(s):               The ProActive Team
*                        http://proactive.inria.fr/team_members.htm
*  Contributor(s): ActiveEon Team - http://www.activeeon.com
*
* ################################################################
* $$ACTIVEEON_CONTRIBUTOR$$
*/
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