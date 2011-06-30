/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Planning;

/**
 *
 * @author pgouttef
 */
public class Event {
    
    /**
     * the start time of an event, its duration an some specifics confugation datas
     */
    private PlanningTime startTime = new PlanningTime();
    private Duration duration = new Duration();
    private Config config = new Config();

    public Event() {
    }

    public Event(PlanningTime startTime , Duration duration , Config config ) {
        this.startTime = startTime; this.duration = duration; this.config = config;
    }
    
    public Config getConfig() {
        return config;
    }

    public void setConfig(Config config) {
        this.config = config;
    }

    public Duration getDuration() {
        return duration;
    }

    public void setDuration(Duration duration) {
        this.duration = duration;
    }

    public PlanningTime getStartTime() {
        return startTime;
    }

    public void setStartTime(PlanningTime startTime) {
        this.startTime = startTime;
    }
    
    
}
