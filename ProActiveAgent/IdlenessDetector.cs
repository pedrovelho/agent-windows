using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using ConfigParser;

namespace ProActiveAgent
{
    enum State
    {
        Stopped,
        Started
    }

    public class IdlenessDetector
    {
        private int begSeconds;
        private int endSeconds;
        private int begThreshold;
        private int endThreshold;

        private int predicateSuccesses = 0;

        private State state;
        private ActionExecuter exec;

        private Timer internalTimer;

        private PerformanceCounter cpuLoadCounter;

        public IdlenessDetector(int begSeconds, int endSeconds, int begThreshold, int endThreshold, Action action, ActionExecuter exec) {
            state = State.Stopped;
            this.begThreshold = begThreshold;
            this.endThreshold = endThreshold;
            this.begSeconds = begSeconds;
            this.endSeconds = endSeconds;

            this.cpuLoadCounter = new PerformanceCounter();
            this.cpuLoadCounter.CategoryName = "Processor";
            this.cpuLoadCounter.CounterName = "% Processor Time";
            this.cpuLoadCounter.InstanceName = "_Total";

            this.exec = exec;

            this.internalTimer = new Timer(new TimerCallback(cpuCheck), action, 0, 1000);
        }

        public void cpuCheck(object objAction) {
            int cpuLoad = Convert.ToInt32(cpuLoadCounter.NextValue());
            Console.WriteLine("CPU LOAD: " + cpuLoad);
            switch (state)
            {
                case State.Stopped:
                    if (cpuLoad <= begThreshold)
                        predicateSuccesses++;
                    else
                        predicateSuccesses = 0;
                    if (predicateSuccesses >= begSeconds)
                    {
                        predicateSuccesses = 0;
                        state = State.Started;
                        exec.sendStartAction(objAction);
                    }
                    break;
                case State.Started:
                    if (cpuLoad >= endThreshold)
                        predicateSuccesses++;
                    else
                        predicateSuccesses = 0;
                    if (predicateSuccesses >= endSeconds)
                    {
                        predicateSuccesses = 0;
                        state = State.Stopped;
                        exec.sendStopAction(objAction);
                    }
                    break;
            }
        }

        public void dispose()
        {
            begSeconds = -1;
            endSeconds = -1;
            begThreshold = -1;
            endThreshold = -1;
            predicateSuccesses = -1;
            exec = null;
            internalTimer.Dispose();
            cpuLoadCounter.Dispose();
        }
    }
}
