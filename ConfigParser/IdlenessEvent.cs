using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * Idleness event triggers action when CPU usage drops below given begThreshold
 * for a given amount of time (number of begSeconds). 
 * It terminates the action when CPU usage
 * raises above endThreshold for endSecs seconds.
 */

namespace ConfigParser
{
    public class IdlenessEvent : Event
    {
        private int _begSecs;
        private int _endSecs;
        private int _begThreshold;
        private int _endThreshold;
        
        [XmlElement("beginSecs")]
        public int beginSecs
        {
            get
            {
                return _begSecs;
            }

            set
            {
                _begSecs = value;
            }
        }

        [XmlElement("endSecs")]
        public int endSecs
        {
            get
            {
                return _endSecs;
            }

            set
            {
                _endSecs = value;
            }
        }

        [XmlElement("beginThreshold")]
        public int beginThreshold
        {
            get
            {
                return _begThreshold;
            }

            set
            {
                _begThreshold = value;
            }
        }

        [XmlElement("endThreshold")]
        public int endThreshold
        {
            get
            {
                return _endThreshold;
            }

            set
            {
                _endThreshold = value;
            }
        }

    }
}
