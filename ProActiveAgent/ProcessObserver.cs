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
        private Thread monitoringOut;
        private Thread monitoringError;

        public ProcessObserver(Logger logger)
        {
            this.logger = logger;
            this.monitoringOut = new Thread(monitorStdOut);
            this.monitoringError = new Thread(monitorStdError);
            this.monitoringOut.Start();
            this.monitoringError.Start();
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
                catch (IOException e)
                {//TODO: what to do here?
                 //at least log something ....
                    WindowsService.log(e.StackTrace, LogLevel.TRACE);
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        private void monitorStdError()
        {
            StreamReader outputReader = null;
            for (; ; )
            {
                if (monitored != null)
                {
                    outputReader = monitored.StandardError;
                }

                try
                {
                    if (outputReader != null)
                    {
                        string line = outputReader.ReadLine();
                        while (line != null)
                        {
                            logger.log("ERROR: " + line, LogLevel.TRACE);
                            line = outputReader.ReadLine();
                        }
                    }
                }
                catch (IOException e)
                {//TODO: what to do here?
                    //at least log something ....
                    WindowsService.log(e.StackTrace, LogLevel.TRACE);
                }

                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
