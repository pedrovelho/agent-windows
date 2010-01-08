/*
* ################################################################
*
* ProActive: The Java(TM) library for Parallel, Distributed,
*            Concurrent computing with Security and Mobility
*
* Copyright (C) 1997-2009 INRIA/University of Nice-Sophia Antipolis
* Contact: proactive@ow2.org
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; version 3 of
* the License.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this library; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
* 
* If needed, contact us to obtain a release under GPL Version 2. 
*
*  Initial developer(s):               The ProActive Team
*                        http://proactive.inria.fr/team_members.htm
*  Contributor(s): ActiveEon Team - http://www.activeeon.com
*
* ################################################################
* $$ACTIVEEON_CONTRIBUTOR$$
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;

/**
 * An event type that allows to start actions based on time. 
 * Such events last specified amount of time.
 */

namespace ConfigParser
{
    public class CalendarEvent : Event
    {
        // day of week
        private String myStartDay;
        private int myStartHour;
        // minute
        private int myStartMinute;
        // second
        private int myStartSecond;
        // number of days
        private int myDurationDays;
        // number of hours
        private int myDurationHours;
        // number of minutes
        private int myDurationMinutes;
        // number of seconds
        private int myDurationSeconds;
        // The process priority        
        private ProcessPriorityClass myProcessPriority;
        // The max cpu usage
        private uint myMaxCpuUsage;

        public CalendarEvent()
        {
            this.myStartDay = "monday";
            this.processPriority = ProcessPriorityClass.Normal;
            this.maxCpuUsage = 100;
        }

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

        [XmlElement("processPriority", IsNullable = false)]
        public System.Diagnostics.ProcessPriorityClass processPriority
        {
            set
            {
                this.myProcessPriority = value;
            }
            get
            {
                return this.myProcessPriority;
            }
        }

        [XmlElement("maxCpuUsage", IsNullable = false)]
        public uint maxCpuUsage
        {
            set
            {
                this.myMaxCpuUsage = value;
            }
            get
            {
                return this.myMaxCpuUsage;
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

        public string resolveDayIntToString(int day)
        {
            if (day == 1)
                return "monday";
            if (day == 2)
                return "tuesday";
            if (day == 3)
                return "wednesday";
            if (day == 4)
                return "thursday";
            if (day == 5)
                return "friday";
            if (day == 6)
                return "saturday";
            if (day == 0)
                return "sunday";
            return "";
        }

        public bool isAlwaysAvailable()
        {
            return this.durationDays == 6 && this.durationHours == 23 && this.durationMinutes == 59 && this.durationSeconds == 59;
        }

        public override string ToString()
        {
            if (this.isAlwaysAvailable())
            {
                return "Always available";
            }

            //--Compute after duration
            int finishSecond = this.startSecond;
            int finishMinute = this.startMinute;
            int finishHour = this.startHour;
            string finishDay = "";

            finishSecond += this.durationSeconds;
            if (finishSecond >= 60)
            {
                finishMinute += finishSecond - 60;
                finishSecond -= 60;
            }

            finishMinute += this.durationMinutes;
            if (finishMinute >= 60)
            {
                finishHour += finishMinute - 60;
                finishMinute -= 60;
            }

            finishHour += this.durationHours;
            if (finishHour >= 24)
            {
                finishDay = resolveDayIntToString((int)(((this.resolveDay() + this.durationDays) + 1) % 7));
                finishHour -= 24;
            }
            else
            {
                finishDay = resolveDayIntToString((int)((this.resolveDay() + this.durationDays) % 7));
            }

            //return this.startDay.Substring(0, 3) + "/" + this.startHour + "/" + this.startMinute + "/" + this.startSecond;
            return this.startDay + " - " + formatDate(this.startHour) + ":" + formatDate(this.startMinute) + ":" + formatDate(this.startSecond) + " => " + finishDay + " - " + formatDate(finishHour) + ":" + formatDate(finishMinute) + ":" + formatDate(finishSecond);
        }

        private static string formatDate(int num)
        {
            if (num < 10)
                return "0" + num.ToString();
            return num.ToString();
        }
    }
}
