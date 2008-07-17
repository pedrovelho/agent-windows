using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ProActiveAgent
{
    class DaemonRunner
    {
//      private static string DEFAULT_CMD = "..\\..\\scripts\\windows\\daemon2.bat";
        private static string DEFAULT_CMD = "d:\\tdobek\\proactive-trunk\\scripts\\windows\\p2p\\daemon.bat";
        private static string WORKING_DIR = "d:\\tdobek\\proactive-trunk\\scripts\\windows\\p2p";

        private string cmd;
        private string workingDir;
        private Process process = null;

        public DaemonRunner()
        {
            this.cmd = DEFAULT_CMD;
            this.workingDir = WORKING_DIR;
        }

        public void setCommand(String command)
        {
            this.cmd = command;
        }

        public void setWorkingDir(String workingDir)
        {
            this.workingDir = workingDir;
        }

        public void startDaemon()
        {
            if (process != null)
                stopDaemon();

            process = new Process();

            process.StartInfo.Arguments = "";
            process.StartInfo.FileName = cmd;
            process.StartInfo.WorkingDirectory = workingDir;

            process.Start();
        }

        public void stopDaemon()
        {
            if (process == null)
                return;

            process.Kill();
            process = null;
        }
    }
}
