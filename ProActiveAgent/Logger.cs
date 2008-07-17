using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ProActiveAgent
{
    public enum LogLevel
    {
        TRACE = 1,
        INFO = 2
    }

    public interface Logger
    {
        void log(string txt, LogLevel level);

        void onStopService();

    }
}
