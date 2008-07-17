using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace ProActiveAgent
{
    public class EventLogger : Logger
    {
        private bool disabled = false;
        private EventLog eventLog;

        public EventLogger()
        {
            if (!EventLog.SourceExists("ProActive Agent"))
                disabled = true;
            else
            {
                this.eventLog = new EventLog();
                this.eventLog.Source = "ProActive Agent";
            }
        }

        public void log(string what, LogLevel level)
        {
            if (!disabled)
            {
                if (((int)level) >= ((int) LogLevel.INFO))
                    eventLog.WriteEntry(what);
            }
        }
        
        public void onStopService()
        {
        }

    }
}
