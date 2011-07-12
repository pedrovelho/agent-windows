/*
 * ################################################################
 *
 * ProActive Parallel Suite(TM): The Java(TM) library for
 *    Parallel, Distributed, Multi-Core Computing for
 *    Enterprise Grids & Clouds
 *
 * Copyright (C) 1997-2011 INRIA/University of
 *                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 *
 * If needed, contact us to obtain a release under GPL Version 2 or 3
 * or a different license than the AGPL.
 *
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################ 
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
package Planning;

/**
 *
 * @author philippe Gouttefarde
 */
public class Duration {
    
    /**
     * Simple informations to get back duration datas of an event
     */
    private int days = 0;
    private int hours = 0;
    private int minutes = 0;
    private int seconds = 0;

    public Duration(int d, int h, int m , int s) { days = d; hours = h; minutes = m; seconds = s; }

    public Duration() { }

    /*
     * getters and setters
     */
    public int getDay() {
        return days;
    }

    public void setDay(int day) {
        this.days = day;
    }

    public int getHour() {
        return hours;
    }

    public void setHour(int hour) {
        this.hours = hour;
    }

    public int getMinute() {
        return minutes;
    }

    public void setMinute(int minute) {
        this.minutes = minute;
    }

    public int getSecond() {
        return seconds;
    }

    public void setSecond(int second) {
        this.seconds = second;
    }
    
    
}
