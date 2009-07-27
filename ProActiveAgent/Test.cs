using System;
using System.Collections.Generic;
using System.Text;
using ConfigParser;
using System.Collections;
using System.Text.RegularExpressions;


namespace ProActiveAgent
{
    public class Test
    {

        public static void _Main(String[] args)
        {


            Console.WriteLine("1100" + Utils.isTcpPortAvailable(1100));

            Console.WriteLine("1101" + Utils.isTcpPortAvailable(1101));

            Console.WriteLine("1102" + Utils.isTcpPortAvailable(1102));

            Console.WriteLine("1103" + Utils.isTcpPortAvailable(1103));

            //Console.WriteLine("65537" + Utils.isTcpPortAvailable(65535));


           
            Console.ReadKey();
        }
    }
}
