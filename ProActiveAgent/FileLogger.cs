using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ProActiveAgent
{
    public class FileLogger : Logger
    {
        private StreamWriter sw;

        public FileLogger(string workDir)
        {
            if (File.Exists(workDir + "\\ProActiveAgent-log.txt"))
                this.sw = File.AppendText(workDir + "\\ProActiveAgent-log.txt");
            else
                this.sw = File.CreateText(workDir + "\\ProActiveAgent-log.txt");
        }

        public void log(string what, LogLevel level)
        {
            sw.WriteLine(DateTime.Now + " ::" + what);
            sw.Flush();
        }

        public void onStopService()
        {
            sw.Close();
        }

    }
}
