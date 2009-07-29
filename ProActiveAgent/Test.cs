using System;


namespace ProActiveAgent
{
    public class Test
    {

        public static void _Main(String[] args)
        {


            // Console.WriteLine("1100" + Utils.isTcpPortAvailable(1100));

            // Console.WriteLine("1101" + Utils.isTcpPortAvailable(1101));

            string s = "-Dproactive.net.interface=${rank}0000000";

            Console.WriteLine("--> " + s.Replace("${rank}", "1"));
           
            Console.ReadKey();
        }
    }
}
