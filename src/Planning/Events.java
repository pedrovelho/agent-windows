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

import java.util.ArrayList;

/**
 *
 * @author philippe Gouttefarde
 */
public class Events {
    
    /**
     * The events list which we can see in the planning tab
     */
    private ArrayList<Event> listEvents = new ArrayList<Event>();

    /*
     * getters and setters
     */
    public ArrayList<Event> getListEvents() {
        return listEvents;
    }

    public void setListEvents(ArrayList<Event> listEvents) {
        this.listEvents = listEvents;
    }
    
    /**
     * add a new avent to the end of the list
     */
    public void addEvent(Event ev) {
        
        listEvents.add(ev);
    }
    
    /**
     * remove all events in the list
     */
    public void freeEventsList() {
        listEvents = new ArrayList<Event>();
    }
    
    /**
     * 
     * @return the size of the list 
     */
    public int getSize() {
        return listEvents.size();
    }
    
    /**
     * 
     * @param i number of the event we want
     * @return the correspondant event
     */
    public Event get(int i) {
        if(i > getSize()) {
            return null;
        }
        return listEvents.get(i);
    }
    
    /**
     * 
     * @return the last event if it exists 
     */
    public Event getLastEvent() {
        if(getSize() > 0) {
            return listEvents.get( getSize() - 1 );
        }
        return null;
    }
    
    /**
     * Remove a specific event
     * @param id of the event 
     */
    public void removeEventById(int id) {
        if(id >= 0 && id < getSize()) {
            listEvents.remove(id);
        }
    }
    
    /**
     * Set a specific event
     * @param id of the event 
     */
    public void setEventById(int id, Event ev) {
        if(id >= 0 && id < getSize()) {
            listEvents.set(id, ev);
        }
    }
    
}
