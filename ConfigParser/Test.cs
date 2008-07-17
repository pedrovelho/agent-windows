using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigParser
{
    public class Test
    {
        static void Main()
        {
            Configuration config = ConfigurationParser.generateSampleConf();
            ConfigurationParser.saveXml("test.xml", config);
        }
    }
}
