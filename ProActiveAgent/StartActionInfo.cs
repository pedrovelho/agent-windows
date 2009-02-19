using System.Diagnostics;

namespace ProActiveAgent
{
    public class StartActionInfo
    {
        private ConfigParser.Action action;
        private long stopTime; // in ticks
        private ProcessPriorityClass processPriority;
        private uint maxCpuUsage;

        public long getStopTime()
        {
            return stopTime;
        }

        public ConfigParser.Action getAction()
        {
            return action;
        }

        public void setAction(ConfigParser.Action action)
        {
            this.action = action;
        }

        public void setStopTime(long ticks)
        {
            this.stopTime = ticks;
        }

        public void setProcessPriority(ProcessPriorityClass processPriority)
        {
            this.processPriority = processPriority;
        }

        public ProcessPriorityClass getProcessPriority()
        {
            return this.processPriority;
        }

        public void setMaxCpuUsage(uint maxCpuUsage)
        {
            this.maxCpuUsage = maxCpuUsage;
        }

        public uint getMaxCpuUsage()
        {
            return this.maxCpuUsage;
        }

    }
}
