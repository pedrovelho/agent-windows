using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * An event type that allows to start actions based on time. 
 * Such events last specified amount of time.
 */

namespace ConfigParser
{
    public class CalendarEvent : Event
    {
        // START CONFIGURATION
        // day of week
        private String myStartDay;

        [XmlElement("startDay", IsNullable = false)]
        public String startDay
        {
            set
            {
                myStartDay = value;
            }

            get
            {
                return myStartDay;
            }
        }

        private int myStartHour;

        [XmlElement("startHour", IsNullable = false)]
        public int startHour
        {
            set
            {
                myStartHour = value;
            }
            get
            {
                return myStartHour;
            }
        }

        // minute
        private int myStartMinute;

        [XmlElement("startMinute", IsNullable = false)]
        public int startMinute
        {
            set
            {
                myStartMinute = value;
            }
            get
            {
                return myStartMinute;
            }
        }

        // second
        private int myStartSecond;

        [XmlElement("startSecond", IsNullable = false)]
        public int startSecond
        {
            set
            {
                myStartSecond = value;
            }
            get
            {
                return myStartSecond;
            }
        }

        // DURATION OF EVENT

        // number of days
        private int myDurationDays;

        [XmlElement("durationDays", IsNullable = false)]
        public int durationDays
        {
            set
            {
                myDurationDays = value;
            }
            get
            {
                return myDurationDays;
            }
        }

        // number of hours
        private int myDurationHours;

        [XmlElement("durationHours", IsNullable = false)]
        public int durationHours
        {
            set
            {
                myDurationHours = value;
            }
            get
            {
                return myDurationHours;
            }
        }
        
        // number of minutes
        private int myDurationMinutes;

        [XmlElement("durationMinutes", IsNullable = false)]
        public int durationMinutes
        {
            set
            {
                myDurationMinutes = value;
            }
            get
            {
                return myDurationMinutes;
            }
        }

        // number of seconds
        private int myDurationSeconds;

        [XmlElement("durationSeconds", IsNullable = false)]
        public int durationSeconds
        {
            set
            {
                myDurationSeconds = value;
            }
            get
            {
                return myDurationSeconds;
            }
        }

        // textual to number representation of day
        // used in computations with dates
        public int resolveDay()
        {
            if (this.myStartDay.Equals("monday"))
                return 1;
            if (this.myStartDay.Equals("tuesday"))
                return 2;
            if (this.myStartDay.Equals("wednesday"))
                return 3;
            if (this.myStartDay.Equals("thursday"))
                return 4;
            if (this.myStartDay.Equals("friday"))
                return 5;
            if (this.myStartDay.Equals("saturday"))
                return 6;
            if (this.myStartDay.Equals("sunday"))
                return 0;
            return -1;
        }

    }
}
