/*
 * ################################################################
 *
 * ProActive: The Java(TM) library for Parallel, Distributed,
 *            Concurrent computing with Security and Mobility
 *
 * Copyright (C) 1997-2010 INRIA/University of 
 *                                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
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
 * If needed, contact us to obtain a release under GPL Version 2 
 * or a different license than the GPL.
 *
 *  Initial developer(s):               The ProActive Team
 *                        http://proactive.inria.fr/team_members.htm
 *  Contributor(s): ActiveEon Team - http://www.activeeon.com
 *
 * ################################################################
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
using System;
using System.Diagnostics;
using ConfigParser;

namespace ProActiveAgent
{
    sealed class StartActionInfo
    {
        private readonly ConnectionType _connection;
        private readonly DateTime _stopTime;
        private readonly ProcessPriorityClass _processPriority;
        private readonly uint _maxCpuUsage;

        public StartActionInfo(ConnectionType connection, DateTime stopTime, ProcessPriorityClass processPriority, uint maxCpuUsage)
        {
            this._connection = connection;
            this._stopTime = stopTime;
            this._processPriority = processPriority;
            this._maxCpuUsage = maxCpuUsage;
        }

        public ConnectionType action
        {
            get
            {
                return this._connection;
            }
        }

        public DateTime stopTime
        {
            get
            {
                return this._stopTime;
            }
        }

        public ProcessPriorityClass processPriority
        {
            get
            {
                return this._processPriority;
            }
        }

        public uint maxCpuUsage
        {
            get
            {
                return this._maxCpuUsage;
            }
        }
    }
}
