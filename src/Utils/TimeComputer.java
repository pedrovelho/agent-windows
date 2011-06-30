/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Utils;

import Model.ModelManager;
import Planning.PlanningTime;

/**
 *
 * @author pgouttef
 */
public class TimeComputer {
    
    /**
     *      A compute class which operate on time and specialy on PlanningTime and Duration classes.
     */
    
    
    /**
     * The addition between day1 and day2 
     * @param day1 : the week's day
     * @param day2 : the day rang
     * @return 
     */
    public static String addDays(String day1, int day2) {
        
        int day1Number = ModelManager.convertDayToInt(day1);
        
        return ModelManager.convertIntToDay((day1Number+day2)%7);
    }
    
    /**
     * Simple addition of two hours
     * @param h1 : the first hour
     * @param h2 : the second hour
     * @return the addition result
     */
    public static int addHours(int h1, int h2) {
        return (h1+h2)%24;
    }
    
    /**
     * Simple addition of two minutes
     * @param m1 : the first minute
     * @param m2 : the second minute
     * @return the addition result
     */
    public static int addMinutes(int m1, int m2) {
        return (m1+m2)%60;
    }
    
    /**
     * Simple addition of two seconds
     * @param s1 : the first second
     * @param s2 : the second second
     * @return the addition result
     */
    public static int addSeconds(int s1, int s2) {
        return (s1+s2)%60;
    }
    
    /**
     * Main method make addition between a Planning data and some durations datas
     * @param start : the start time
     * @param d : number of days to compute
     * @param h : number of hours to compute
     * @param m : number of minutes to compute
     * @param s : number of seconds to compute 
     * @return the addition result in PlanningTime form
     */
    public static PlanningTime addTime( PlanningTime start , int d , int h , int m , int s ) {
        
        String d2 = addDays( start.getDay() , d );
        int h2 = addHours( start.getHour(), h);
        int m2 = addMinutes( start.getMinute(), m);
        int s2 = addSeconds( start.getSecond(), s);
        
        PlanningTime end = new PlanningTime(d2, h2, m2, s2);
        
        if(s2 < s) { end.setMinute ( end.getMinute() + 1 )   ; } 
        if(m2 < m) { end.setHour ( end.getHour() + 1 )   ; } 
        if(h2 < h) { end.setDay( ModelManager.convertIntToDay( ModelManager.convertDayToInt(end.getDay()) + 1 ))   ; } 
        
        return end;
    }
    
    public static void main(String argv[]) {
        
        String day = "Wednesday";
        int hour = 15;
        int minute = 23;
        int second = 50;
        
        PlanningTime start = new PlanningTime(day, hour, minute, second);
        PlanningTime end = addTime(start, 6, 10, 7, 20);
        System.out.println( end.toString() );
    }
}
