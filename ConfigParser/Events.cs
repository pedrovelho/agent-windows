using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * Collection of events on which ProActive agent will react
 */

namespace ConfigParser
{
    public class Events
    {
        private List<Event> myEvents = new List<Event>();

        [XmlElement("event")]
        public Event[] events
        {
            get
            {
                Event[] events = new Event[myEvents.Count];
                myEvents.CopyTo(events);
                return events;
            }
            set
            {
                if (value == null) return;
                Event[] events = (Event[])value;
                myEvents.Clear();
                foreach (Event aEvent in events)
                    myEvents.Add(aEvent);
            }
        }

        public void removeEvent(int index)
        {
            myEvents.RemoveAt(index);
        }

        public void addEvent(Event eventt)
        {
            myEvents.Add(eventt);
        }

        public void modifyEvent(int index, Event eventt)
        {
            myEvents[index] = eventt;
        }

// THE FOLLOWING CODE IS DEPRECATED

/*
        public void addEvent(Event anEvent) {
            this.events.Add(anEvent);
        }

        public List<Event> getEntries()
        {
            return events;
        }
*/
    }
}
