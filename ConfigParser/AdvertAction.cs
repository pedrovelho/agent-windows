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
        /** The string description of this action **/
        public const string DESCRIPTION = "RMI Registration";

        // Name of the node to create
        // If the name is null, a default name will be used
        private string myNodeName;

        [XmlElement("nodeName", IsNullable=false)]
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
