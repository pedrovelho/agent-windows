using System;
using System.Collections.Generic;
using System.Text;

namespace ProActiveAgent
{
    public class LoggerComposite : Logger
    {
        private List<Logger> loggers;

        public LoggerComposite()
        {
            this.loggers = new List<Logger>();
        }

        public void addLogger(Logger logger)
        {
            this.loggers.Add(logger);
        }

        public void removeLogger(Logger logger)
        {
            this.loggers.Remove(logger);
        }

        public void log(string txt, LogLevel lvl)
        {
            foreach (Logger l in loggers)
                l.log(txt, lvl);
        }

        public void onStopService()
        {
            foreach (Logger l in loggers)
                l.onStopService();
        }
    }
}
