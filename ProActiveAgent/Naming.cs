using System;
using System.Collections.Generic;
using System.Text;

namespace ProActiveAgent
{
    /// <summary>
    /// Class used for constants definition</summary>
    /// <remarks>
    /// Some of these constants may be used by other projects. The entire solution may need to be regenerated in case of modification.</remarks>
    public static class Constants
    {
        /// <summary>
        /// The name of the ProActive Agent service</summary>
        public const string PROACTIVE_AGENT_SERVICE_NAME = "ProActive Agent";
        /// <summary>
        /// The name of the ProActive Agent executable</summary>
        public const string PROACTIVE_AGENT_EXECUTABLE_NAME = "ProActiveAgent.exe";
        /// <summary>
        /// The name of the windows event logging system used by the ProActive Agent.
        /// This can be Application, System, Security, or custom log name.</summary>
        public const string PROACTIVE_AGENT_WINDOWS_EVENTLOG_LOG = "Application";
        /// <summary>
        /// The default install dir location of the ProActive Agent.</summary>
        public const string PROACTIVE_AGENT_DEFAULT_INSTALL_LOCATION = "C:\\Program Files\\ProActiveAgent";
        /// <summary>
        /// The default config location of the ProActive Agent.</summary>
        public const string PROACTIVE_AGENT_DEFAULT_CONFIG_LOCATION = PROACTIVE_AGENT_DEFAULT_INSTALL_LOCATION + "\\PAAgent-config.xml";
        /// <summary>
        /// The windows registry subkey used by ProActive Agent.</summary>
        public const string PROACTIVE_AGENT_REG_SUBKEY = "Software\\ProActiveAgent";
        /// <summary>
        /// The name of the reg value used for install location.</summary>
        public const string PROACTIVE_AGENT_INSTALL_REG_VALUE_NAME = "AgentDirectory";
        /// <summary>
        /// The name of the reg value used for config location.</summary>
        public const string PROACTIVE_AGENT_CONFIG_REG_VALUE_NAME = "ConfigLocation";
    }

    /// <summary>
    /// Enumeration type of ProActive Agent Service custom commands.
    /// There are following commands:
    ///    Start - trigger action
    ///    Stop  - stops triggered action or do nothing when it has not been triggered
    /// </summary>
    public enum PAACommands
    {
        ScreenSaverStart = 128,
        ScreenSaverStop = 129,
        GlobalStop = 130
        /*AllowRuntime = 131,
        ForbidRuntime = 132*/
    }

    /// <summary>
    /// Enumeration type of ProActive Agent running type.
    /// </summary>
    public enum ApplicationType
    {
        AgentScheduler = 1,
        AgentScreensaver = 2
    }
}
