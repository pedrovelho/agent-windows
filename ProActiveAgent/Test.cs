using System;
using System.Collections.Generic;
using System.Text;
using ConfigParser;

namespace ProActiveAgent
{
    public class Test
    {

        public static void _Main (String[] args)
        {
            String CONFIG_LOCATION = "c:\\PAAgent-config.xml";

            Configuration configuration = ConfigurationParser.parseXml(CONFIG_LOCATION, "d:\\ProActiveAgent");
            FileLogger logger = new FileLogger("d:\\tdobek");
            ProActiveExec runner = new ProActiveExec(logger, "d:\\tdobek", "d:\\tdobek", "d:\\tdobek", "d:\\tdobek", "Idle");
/*            runner.setCommand(configuration.daemonConfig.daemonScript);
            runner.setWorkingDir(configuration.daemonConfig.daemonWorkingDir); */
            runner.sendStartAction(configuration.action, ApplicationType.AgentScreensaver);
            //TimerManager timerManager = new TimerManager(configuration);
            
            for (; ; )
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

    }
}
