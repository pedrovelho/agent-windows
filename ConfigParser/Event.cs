using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * An event indicates the entity that can trigger action
 */

namespace ConfigParser
{
    [XmlInclude(typeof(CalendarEvent))]
    public class Event
    {
    }
}
