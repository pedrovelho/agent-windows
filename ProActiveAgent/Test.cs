using System;
using System.Collections.Generic;
using System.Text;
using ConfigParser;
using System.Collections;

namespace ProActiveAgent
{
    public class Test
    {

        public static void _Main(String[] args)
        {
            /*ArrayList agregations = new ArrayList();

            Console.WriteLine("TEST");
            String CONFIG_LOCATION = "c:\\PAAgent-config.xml";

            Configuration configuration = ConfigurationParser.parseXml(CONFIG_LOCATION, @"C:\Documents and Settings\ohelin\workspace\AgentWIN\bin\Release");
//            FileLogger logger = new FileLogger("c:\\test");


            ProActiveExec.setRegistryIsRuntimeStarted(false);
            //set allowRuntime registry to true
            //ProActiveExec.setRegistryAllowRuntime(true);

            //this.configuration = ConfigurationParser.parseXml(configLocation, agentLocation);
            LoggerComposite composite = new LoggerComposite();
            composite.addLogger(new FileLogger("c:\\test"));
            composite.addLogger(new EventLogger());
            Logger logger = composite;
            WindowsService.log("--- Starting ProActiveAgent Service", LogLevel.TRACE);
            //--Foreach action
            ProActiveExec exe;
            TimerManager tim;
            Agregation agre;
           
            foreach (Action action in configuration.actions.actions)
            {
                    WindowsService.log("Starting action " + action.GetType().Name, LogLevel.TRACE);

                    exe = new ProActiveExec(logger, "c:\\test", configuration.agentConfig.jvmParams,
                    configuration.agentConfig.javaHome, configuration.agentConfig.proactiveLocation,
                    action.priority, action.initialRestartDelay);


                    tim = new TimerManager(configuration, action, exe);

                    exe.setTimerMgr(tim);

                    //--Save in collection
                    agre = new Agregation(exe, tim, action);
                    agregations.Add(agre);
            }
            WindowsService.log("All tasks are scheduling", LogLevel.TRACE);

           
            Console.ReadKey();*/
        }
    }
}
