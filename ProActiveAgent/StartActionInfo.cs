using System;
using System.Diagnostics;

namespace ProActiveAgent
{
    public class StartActionInfo
    {
        private readonly ConfigParser.Action _action;
        private readonly DateTime _stopTime;
        private readonly ProcessPriorityClass _processPriority;
        private readonly uint _maxCpuUsage;        

        public StartActionInfo(ConfigParser.Action action, DateTime stopTime, ProcessPriorityClass processPriority, uint maxCpuUsage)
        {
            this._action = action;
            this._stopTime = stopTime;
            this._processPriority = processPriority;
            this._maxCpuUsage = maxCpuUsage;
        }

        public ConfigParser.Action action
        {
            get
            {
                return this._action;
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
