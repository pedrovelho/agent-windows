/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Planning;

/**
 *
 * @author pgouttef
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
