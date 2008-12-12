using System;
using System.Collections.Generic;
using System.Text;
using ConfigParser;

namespace ProActiveAgent
{
    public class StartActionInfo
    {
        private ConfigParser.Action action;
        private long stopTime; // in ticks

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

    }
}
