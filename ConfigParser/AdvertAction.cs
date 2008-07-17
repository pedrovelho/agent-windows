using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * AdvertAction is a description of advertisement action.
 * In this scenario a new ProActive node will be created and
 * advertised in RMI registry
 */

namespace ConfigParser
{
    
    public class AdvertAction : Action
    {
        // Name of the node to create
        // If the name is null, a default name will be used
        private string myNodeName;

        [XmlElement("nodeName")]
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
