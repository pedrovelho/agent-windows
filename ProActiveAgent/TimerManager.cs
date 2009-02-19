using System;
using System.Collections.Generic;
using System.Threading;
using ConfigParser;
using log4net;

/** TimeManager implements semantics of calendar events
 *  It creates timers in order to start or stop actions
 *  
 * The configuration cannot contain overlapping calendar events!
 */

namespace ProActiveAgent
{
    public class TimerManager
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int WEEK_DELAY = 3600 * 24 * 7 * 1000;
        private static int BARRIER_SAFETY_MARGIN = 15000;

        // start - action timers
        private readonly List<Timer> startTimers;
        // stop - action timers
        private readonly List<Timer> stopTimers;
        // retry timers
        private readonly List<Timer> retryTimers;

        private readonly ProActiveExec exec;
        private readonly ConfigParser.Action action;
        private long retryTimeBarrier = 0;

        // The constructor should be called only during starting the service

        public TimerManager(ProActiveExec paExec,ConfigParser.Action action)
        {                                
            this.startTimers = new List<Timer>();
            this.stopTimers = new List<Timer>();
            this.retryTimers = new List<Timer>();

            this.exec = paExec;
            this.action = action;
        }

        public ConfigParser.Action getAssociatedAction()
        {
            return this.action;
        }

        public void loadEvents(List<Event> events)
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Loading events in the Time Manager.");
            }            

            // Get the current date time
            DateTime currentTime = System.DateTime.Now;

            foreach (Event e in events)
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
                    long dueStart = countDelay((accurateStartTime - currentTime));
                    long dueStop = countDelay((accurateStartTime - currentTime).Add(duration));
                    // if now we are in the middle of the event just start
                    bool startNow = (dueStart < 0 && dueStop > 0);
                    if (dueStart < 0)
                        dueStart += WEEK_DELAY;
                    if (dueStop < 0)
                        dueStop += WEEK_DELAY;

                    StartActionInfo startInfo = new StartActionInfo();
                    startInfo.setAction(this.action);
                    startInfo.setStopTime(accurateStartTime.Add(duration).Ticks);
                    startInfo.setProcessPriority(cEvent.processPriority);
                    startInfo.setMaxCpuUsage(cEvent.maxCpuUsage);

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

        // used to add delayed retry-actions
        public void addDelayedRetry(int delay)
        {
            long absoluteDelay = System.DateTime.Now.Ticks + delay * 10000L;
            DateTime absoluteDelayDateTime = new DateTime(absoluteDelay);

            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Trying to schedule restart at "
                    + absoluteDelayDateTime.ToString()
                    + " Limit time barrier is " + new DateTime(retryTimeBarrier).ToString()+ " (we cannot restart after this time point).");
            }   

            if (absoluteDelay < retryTimeBarrier)
            {
                Timer newTimer = new Timer(new TimerCallback(mySendRestartAction), null, delay, System.Threading.Timeout.Infinite);
                retryTimers.Add(newTimer);
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("Restart action " + absoluteDelayDateTime.ToString() + " succesfully scheduled.");
                }                
            }
            else
            {                
                if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("Discarding restarting of " + absoluteDelayDateTime.ToString() + " because it would happen outside the job allocated time.");
                }                
            }
        }

        //note : it does not refer to restarted actions, but started according to calendar event
        private void mySendStartAction(object action)
        {            
            StartActionInfo actionInfo = (StartActionInfo)action;
            long stopTime = actionInfo.getStopTime();

            while (stopTime < DateTime.Now.Ticks)
            {
                stopTime += WEEK_DELAY * 10000;
            }

            if (retryTimeBarrier < stopTime)
            {
                retryTimeBarrier = stopTime;
            }

            retryTimeBarrier -= BARRIER_SAFETY_MARGIN * 10000;
            exec.resetRestartDelay();
            exec.sendStartAction(ApplicationType.AgentScheduler);
            // Apply process priority and max cpu usage
            exec.setProcessBehaviour(actionInfo.getProcessPriority(), actionInfo.getMaxCpuUsage());            
        }

        private void mySendStopAction(object stateInfo)
        {            
            retryTimeBarrier = 0;
            exec.sendStopAction(ApplicationType.AgentScheduler);
        }

        private void mySendRestartAction(object stateInfo)
        {
            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Invoking restart action.");
            } 
            exec.sendRestartAction();
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
            foreach (Timer t in this.startTimers)
            {
                t.Dispose();
            }
            this.startTimers.Clear();

            foreach (Timer t in this.stopTimers)
            {
                t.Dispose();
            }
            this.startTimers.Clear();

            foreach (Timer t in this.retryTimers)
            {
                t.Dispose();
            }
            this.retryTimers.Clear();
        }
    }
}
