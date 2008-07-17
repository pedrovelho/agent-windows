using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using ProActiveAgent;

namespace AgentController
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                printUsage();
                return;
            }
            
            String command = args[0];

            ServiceController sc = new ServiceController("ProActive Agent");

            switch (command) {
                case "P2P_START":
                    sc.ExecuteCommand((int)PAACommands.Start);
                    break;
                case "P2P_STOP":
                    sc.ExecuteCommand((int)PAACommands.Stop);
                    break;
                case "RM_START":
                    sc.ExecuteCommand((int)PAACommands.Start);
                    break;
                case "RM_STOP":
                    sc.ExecuteCommand((int)PAACommands.Stop);
                    break;
                default:
                    printUsage();
                    break;
            }

        }

        static void printUsage()
        {
            Console.WriteLine("Usage: P2P_START | P2P_STOP | RM_START | RM_STOP");
        }
    }
}
