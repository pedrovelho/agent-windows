/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Planning;

/**
 *
 * @author pgouttef
 */
public class PlanningTime {
    
    /**
     * Simple informations to get back start time of an event
     */
    private String day = null;
    private int hour = 0;
    private int minute = 0;
    private int second = 0;

    public PlanningTime(String d, int h, int m , int s) { day = d; hour = h; minute = m; second = s; }

    public PlanningTime() { }

    public String getDay() {
        return day;
    }

    public void setDay(String day) {
        this.day = day;
    }

    public int getHour() {
        return hour;
    }

    public void setHour(int hour) {
        this.hour = hour;
    }

    public int getMinute() {
        return minute;
    }

    public void setMinute(int minute) {
        this.minute = minute;
    }

    public int getSecond() {
        return second;
    }

    public void setSecond(int second) {
        this.second = second;
    }

    /**
     * @return a PlanningTime variable to convert to string
     */
    @Override
    public String toString() {
        String s = "";
        s += getDay() + " - ";
        if(getHour()<10) s += "0";
        s += getHour() + ":";
        if(getMinute()<10) s += "0";
        s += getMinute() + ":";
        if(getSecond()<10) s += "0";
        s += getSecond();
        return s;
    }
}
