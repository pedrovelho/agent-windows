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
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################
 * $$ACTIVEEON_INITIAL_DEV$$
 */
using System;
using System.Collections.Generic;
using System.Threading;
using ConfigParser;
using log4net;
using Microsoft.Win32;

/** 
 * ExecutorsManager manages scheduled start/stop of executors.
 *    
 * The configuration cannot contain overlapping calendar events!
 */

namespace ProActiveAgent
{
    public class ExecutorsManager
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int WEEK_DELAY = 3600 * 24 * 7 * 1000;
        private const int BARRIER_SAFETY_MARGIN_MS = 15000;
        // The barrier a safety margin interval    
        private static TimeSpan SAFETY_MARGIN_TIMESPAN = new TimeSpan(0, 0, 0, 0, BARRIER_SAFETY_MARGIN_MS);
        // start - action timers
        private readonly List<Timer> startTimers;
        // stop - action timers
        private readonly List<Timer> stopTimers;
        /// <summary>
        /// The list of executors.</summary>                        
        private readonly List<ProActiveRuntimeExecutor> proActiveRuntimeExecutors;

        // The constructor should be called only during starting the service
        public ExecutorsManager(Configuration configuration)
        {
            // Get the runtime common start info shared between all executors
            CommonStartInfo commonStartInfo = new CommonStartInfo(configuration);

            // The configuration specifies the number of executors
            int nbProcesses = configuration.agentConfig.useAllCPUs ? Environment.ProcessorCount : configuration.agentConfig.nbProcesses;
            LOGGER.Info("Creating " + nbProcesses + " executors.");

            this.proActiveRuntimeExecutors = new List<ProActiveRuntimeExecutor>(nbProcesses);

            // Get the initial value for the ProActive Rmi Port specified in the configuration
            int lastProActiveRmiPort = configuration.agentConfig.proActiveCommunicationPortInitialValue;

            // Create as many executors as specified in the configuration
            for (int rank = 0; rank < nbProcesses; rank++)
            {

                // Create new executor with a unique rank and a valid ProActive Rmi Port
                ProActiveRuntimeExecutor executor = new ProActiveRuntimeExecutor(commonStartInfo, rank);
                this.proActiveRuntimeExecutors.Add(executor);
            }

            // Try to create the sub key in registry for executors stats            
            // delete all sub keys of the executors key
            RegistryKey key = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_EXECUTORS_REG_SUBKEY, true);
            if (key != null)
            {
                foreach (string name in key.GetValueNames())
                {
                    key.DeleteValue(name);
                }
                key.Close();
            }
            else
            {
                key = Registry.LocalMachine.CreateSubKey(Constants.PROACTIVE_AGENT_EXECUTORS_REG_SUBKEY);
                if (key != null)
                {
                    key.Close();
                }
            }
            // Create the start/stop timers for scheduled events
            this.startTimers = new List<Timer>();
            this.stopTimers = new List<Timer>();

            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Loading events in the Executors Manager.");
            }

            // Get the current date time
            DateTime currentTime = System.DateTime.Now;

            foreach (CalendarEvent e in configuration.events)
            {
                if (e is CalendarEvent)
                {
                    // for each calendar event we calculate remaining time to start and stop service
                    // and according to that register timers                     
                    CalendarEvent cEvent = (CalendarEvent)e;
                    // we provide the day of the week to present start time
                    // the algorithm to count next start is as follows:
                    // 1. how many days are to start action
                    // 2. we add this amount to the current date
                    // 3. we create time event that will be the exact start time (taking year, month and
                    //    day from the current date and other fields from configuration
                    // 4. we keep duration of task
                    // 5. we count due time for beginning and stopping the task
                    // 6. if time is negative, we move it into next week (to avoid waiting for past events)
                    int daysAhead = dayDifference(resolveDayOfWeek(currentTime.DayOfWeek), cEvent.resolveDay());
                    DateTime startTime = currentTime.AddDays(daysAhead);

                    DateTime accurateStartTime = new DateTime(startTime.Year, startTime.Month, startTime.Day,
                        cEvent.startHour, cEvent.startMinute, cEvent.startSecond);
                    TimeSpan duration = new TimeSpan(cEvent.durationDays, cEvent.durationHours, cEvent.durationMinutes,
                                                        cEvent.durationSeconds);
                    if (LOGGER.IsDebugEnabled)
                    {
                        LOGGER.Debug("Loading CalendardEvent " + accurateStartTime.ToString() + " -> " + accurateStartTime.Add(duration).ToString());
                    }
                    // Get the due time for timers
                    long dueStart = countDelay((accurateStartTime - currentTime));
                    long dueStop = countDelay((accurateStartTime - currentTime).Add(duration));
                    // if now we are in the middle of the event just start
                    bool startNow = (dueStart < 0 && dueStop > 0);
                    if (dueStart < 0)
                        dueStart += WEEK_DELAY;
                    if (dueStop < 0)
                        dueStop += WEEK_DELAY;

                    StartActionInfo startInfo = new StartActionInfo(commonStartInfo.selectedAction, accurateStartTime.Add(duration), cEvent.processPriority, cEvent.maxCpuUsage);

                    // timer registration
                    Timer startT = new Timer(new TimerCallback(mySendStartAction), startInfo, dueStart, WEEK_DELAY);
                    Timer stopT = new Timer(new TimerCallback(mySendStopAction), null, dueStop, WEEK_DELAY);

                    this.startTimers.Add(startT);
                    this.stopTimers.Add(stopT);

                    if (startNow)
                    {
                        this.mySendStartAction(startInfo);
                    }
                }
                // Only a single type of event
            }
        }

        public List<ProActiveRuntimeExecutor> getExecutors()
        {
            return this.proActiveRuntimeExecutors;
        }

        //note : it does not refer to restarted actions, but started according to calendar event
        private void mySendStartAction(object action)
        {
            StartActionInfo actionInfo = (StartActionInfo)action;
            DateTime stopTime = actionInfo.stopTime;

            while (stopTime < DateTime.Now)
            {
                stopTime = stopTime.AddMilliseconds(WEEK_DELAY);
            }

            foreach (ProActiveRuntimeExecutor p in this.proActiveRuntimeExecutors)
            {
                DateTime restartBarrierDateTime = p.restartBarrierDateTime;

                if (restartBarrierDateTime < stopTime)
                {
                    restartBarrierDateTime = stopTime;
                }
                // Substract from the barrier a safety margin interval
                p.restartBarrierDateTime = restartBarrierDateTime.Subtract(SAFETY_MARGIN_TIMESPAN);
                // Send the start
                p.sendStartAction(ApplicationType.AgentScheduler);
                // Apply process priority and max cpu usage
                p.setProcessBehaviour(actionInfo.processPriority, actionInfo.maxCpuUsage);
            }
        }

        private void mySendStopAction(object stateInfo)
        {
            foreach (ProActiveRuntimeExecutor p in this.proActiveRuntimeExecutors)
            {
                p.restartBarrierDateTime = System.DateTime.Now;
                p.sendStopAction(ApplicationType.AgentScheduler);
            }
        }

        // we count a number of milliseconds in a given timespan
        private long countDelay(TimeSpan timeSpan)
        {
            return timeSpan.Days * 86400000L + timeSpan.Hours * 3600000L + timeSpan.Minutes * 60000L + timeSpan.Seconds * 1000L + timeSpan.Milliseconds;
        }

        // resolving from .NET API enumeration to meaningful numbers
        private int resolveDayOfWeek(DayOfWeek dow)
        {
            if (dow == DayOfWeek.Friday)
                return 5;
            if (dow == DayOfWeek.Monday)
                return 1;
            if (dow == DayOfWeek.Saturday)
                return 6;
            if (dow == DayOfWeek.Sunday)
                return 0;
            if (dow == DayOfWeek.Thursday)
                return 4;
            if (dow == DayOfWeek.Tuesday)
                return 2;
            if (dow == DayOfWeek.Wednesday)
                return 3;
            return -1;
        }

        // counting day difference
        private int dayDifference(int dayA, int dayB)
        {
            if ((dayB - dayA) < 0)
            {
                return ((dayB - dayA) % 7);
            }
            else if ((dayB - dayA) > 0)
            {
                return -((7 - (dayB - dayA)) % 7);
            }
            else
                return 0;
        }

        // releasing resources
        public void dispose()
        {
            // Dispose all start timers
            foreach (Timer t in this.startTimers)
            {
                t.Dispose();
            }
            this.startTimers.Clear();
            // Dispose all stop timers
            foreach (Timer t in this.stopTimers)
            {
                t.Dispose();
            }
            this.stopTimers.Clear();
            // Dispose all executors
            foreach (ProActiveRuntimeExecutor p in this.proActiveRuntimeExecutors)
            {
                p.dispose();
            }
            this.proActiveRuntimeExecutors.Clear();
        }
    }
}
