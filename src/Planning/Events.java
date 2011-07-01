/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Planning;

import java.util.ArrayList;

/**
 *
 * @author pgouttef
 */
public class Events {
    
    /**
     * The events list zhich we can see in the planning tab
     */
    private ArrayList<Event> listEvents = new ArrayList<Event>();

    public ArrayList<Event> getListEvents() {
        return listEvents;
    }

    public void setListEvents(ArrayList<Event> listEvents) {
        this.listEvents = listEvents;
    }
    
    public void addEvent(Event ev) {
        
        listEvents.add(ev);
    }
    
    public void freeEventsList() {
        listEvents = new ArrayList<Event>();
    }
    
    public int getSize() {
        return listEvents.size();
    }
    
    public Event get(int i) {
        return listEvents.get(i);
    }
    
    public Event getLastEvent() {
        if(getSize() > 0) {
            return listEvents.get( getSize() - 1 );
        }
        return null;
    }
    
    public void removeEventById(int id) {
        if(id >= 0 && id < getSize()) {
            listEvents.remove(id);
        }
    }
    
    public void setEventById(int id, Event ev) {
        if(id >= 0 && id < getSize()) {
            listEvents.set(id, ev);
        }
    }
    
}
