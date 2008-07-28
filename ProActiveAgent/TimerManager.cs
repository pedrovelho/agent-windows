using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ConfigParser;

/** TimeManager implements semantics of calendar events
 *  It creates timers in order to start or stop actions
 *  
 * The configuration cannot contain overlapping calendar events!
 */

namespace ProActiveAgent
{
    public class TimerManager
    {

        private static int WEEK_DELAY = 3600 * 24 * 7 * 1000;
        private static int BARRIER_SAFETY_MARGIN = 5000;

        // start - action timers
        private List<Timer> startTimers = new List<Timer>();
        // stop - action timers
        private List<Timer> stopTimers = new List<Timer>();
        // retry timers
        private List<Timer> retryTimers = new List<Timer>();

        private ProActiveExec exec;
        private Configuration configuration;

        private long retryTimeBarrier = 0;

        //private IdlenessDetector idleMgr = null;

        // config - configuration of ProActive Agent Service
        // paExec - implementation of actions

        // The constructor should be called only during starting the service

        public TimerManager(Configuration config, ProActiveExec paExec)
        {
            this.configuration = config;
            bool startNow = false;  // flag that will be set if now we are in the middle of the event
            this.exec = paExec;
            Events events = config.events;
            foreach (Event e in events.events)
            {
                if (e is CalendarEvent)
                {
                    // for each calendar event we calculate remaining time to start and stop service
                    // and according to that register timers 

                    DateTime currentTime = System.DateTime.Now;
                    CalendarEvent cEvent = (CalendarEvent) e;
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
                    exec.getLogger().log(accurateStartTime.ToString(), LogLevel.TRACE);
                    exec.getLogger().log(accurateStartTime.Add(duration).ToString(), LogLevel.TRACE);
                    long dueStart = countDelay((accurateStartTime - currentTime));
                    long dueStop = countDelay((accurateStartTime - currentTime).Add(duration));
                    if (dueStart < 0 && dueStop > 0)
                        startNow = true;
                    if (dueStart < 0)
                        dueStart += WEEK_DELAY;
                    if (dueStop < 0)
                        dueStop += WEEK_DELAY;

                    StartActionInfo startInfo = new StartActionInfo();
                    startInfo.setAction(config.action);
                    startInfo.setStopTime(accurateStartTime.Add(duration).Ticks);

                    // timer registration
                    Timer startT = new Timer(new TimerCallback(mySendStartAction), startInfo, dueStart, WEEK_DELAY);
                    Timer stopT = new Timer(new TimerCallback(mySendStopAction), config.action, dueStop, WEEK_DELAY);

                    startTimers.Add(startT);
                    stopTimers.Add(stopT);

                    if (startNow)
                        mySendStartAction(startInfo);
                }
                else if (e is IdlenessEvent)
                {
                // TODO: implement

                }

            }
        }

        // used to add delayed retry-actions

        public void addDelayedRetry(int delay)
        {
            exec.getLogger().log("RetryTimeBarrier is " + (new DateTime(retryTimeBarrier).ToString()), LogLevel.TRACE);
            exec.getLogger().log("Our time is         " + (new DateTime(System.DateTime.Now.Ticks + delay * 10000L)).ToString(), LogLevel.TRACE);

            if (System.DateTime.Now.Ticks + delay * 10000L < retryTimeBarrier)
            {
                    Timer newTimer = new Timer(new TimerCallback(mySendRestartAction), configuration.action, delay, System.Threading.Timeout.Infinite);
                    retryTimers.Add(newTimer);
                    exec.getLogger().log("Restart action succesfully scheduled", LogLevel.TRACE);
            }
            else
            {
//                exec.getLogger().log(new DateTime(retryTimeBarrier).ToString(), LogLevel.TRACE);
                exec.getLogger().log("Discarding restarting because it would happen outside the job allocated time", LogLevel.TRACE);
            }
        }

        //note : it does not refer to restarted actions, but started according to calendar event
        private void mySendStartAction(object action)
        {
            StartActionInfo actionInfo = (StartActionInfo)action;
            retryTimeBarrier = actionInfo.getStopTime();
            while (retryTimeBarrier < DateTime.Now.Ticks)
                retryTimeBarrier += WEEK_DELAY * 10000;
            retryTimeBarrier -= BARRIER_SAFETY_MARGIN * 10000;
            
            exec.resetRestartDelay();
            exec.sendStartAction(actionInfo.getAction(), ApplicationType.AgentScheduler);
        }

        private void mySendStopAction(object action)
        {
            retryTimeBarrier = 0;
            exec.sendStopAction(action, ApplicationType.AgentScheduler);
        }

        private void mySendRestartAction(object action)
        {
            exec.getLogger().log("Invoking restart action", LogLevel.TRACE);
            exec.sendRestartAction();
        }

        // we count a number of milliseconds in a given timespan
        private long countDelay(TimeSpan timeSpan)
        {
//            Console.WriteLine("Days: " + timeSpan.Days + "Hours: " + timeSpan.Hours + "Minutes: " + timeSpan.Minutes + "Seconds: " + timeSpan.Seconds + "Milis: " + timeSpan.Milliseconds);
            return timeSpan.Days * 86400000L + timeSpan.Hours * 3600000L + timeSpan.Minutes * 60000L + timeSpan.Seconds * 1000L + timeSpan.Milliseconds;
        }

        // called when service is paused
        public void onPause()
        {
        }

        // called when service is resumed
        public void onResume()
        {
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
            return (dayB - dayA) % 7;
        }

        // releasing resources
        public void dispose()
        {
            foreach (Timer t in startTimers)
            {
                t.Dispose();
            }

            foreach (Timer t in stopTimers)
            {
                t.Dispose();
            }

            foreach (Timer t in retryTimers)
            {
                t.Dispose();
            }
            //idleMgr.dispose();
        }

    }
}
