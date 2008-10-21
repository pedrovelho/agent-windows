using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * Action that registers the desktop machine
 * (specifically ProActive node created on that machine)
 * in Resource Manager instance
 * 
 * It is configured by specifying URL of Resource Manager instance
 * for example rmi://cheypa.inria.fr:1099
 */

namespace ConfigParser
{
    public class RMAction : Action
    {
        private string myUrl;
        private string myNodeName;

        [XmlAttribute("url")]
        public string url
        {
            get
            {
                return myUrl;
            }
            set
            {
                myUrl = value;
            }
        }


        [XmlElement("nodeName", IsNullable = false)]
        public string nodeName
        {
            get
            {
                return myNodeName;
            }
            set
            {
                myNodeName = value;
            }
        }
    }
}
