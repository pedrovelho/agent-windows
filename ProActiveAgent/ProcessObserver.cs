using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace ProActiveAgent
{
    public class ProcessObserver
    {
        private Logger logger;
        private Process monitored;
        private Thread monitoring;

        public ProcessObserver(Logger logger)
        {
            this.logger = logger;
            this.monitoring = new Thread(monitorStdOut);
            this.monitoring.Start();
            this.monitored = null;
        }

        
        public void setMonitorredProcess(Process p)
        {
            this.monitored = p;
        }


        private void monitorStdOut()
        {
            StreamReader outputReader = null;
            for (; ; )
            {
                if (monitored != null)
                {
                    outputReader = monitored.StandardOutput;
                }

                try
                {
                    if (outputReader != null)
                    {
                        string line = outputReader.ReadLine();
                        while (line != null)
                        {
                            logger.log("OUTPUT: " + line, LogLevel.TRACE);
                            line = outputReader.ReadLine();
                        }
                    }
                }
                catch (IOException)
                {
                }

                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
