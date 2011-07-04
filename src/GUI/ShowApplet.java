/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package GUI;

import Model.ModelManager;
import Planning.Event;
import Planning.Events;
import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.HeadlessException;
import java.awt.RenderingHints;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import javax.swing.JApplet;
import javax.swing.JFrame;

/**
 *
 * @author pgouttef
 */
public class ShowApplet extends JApplet {

    private Events evList = null;
    
    
    private int GRAPHIC_LENGTH = 350;
    private int GRAPHIC_HEIGHT = 350 - 35; // Windows - The header of the frame [-] [ ] [X]
    private int BORDER = 30;
    private int AxesX = 0 + BORDER;
    private int AxesY = GRAPHIC_HEIGHT - BORDER;
    
    private Color colorLine = Color.gray ;
    private Color colorBackground = Color.white;
    private Color colorText = Color.black ;
    
    //Single Color :
//    private Color[] colorBlock = {new Color(255, 203, 96)};
    //Several Color :
    private Color[] colorBlock = {  new Color(230, 126, 48) ,
                                    new Color(145, 40 , 59) , 
                                    new Color(157, 62 , 12) ,
                                    new Color(255, 203, 96) ,
                                    new Color(173, 57 , 14) };
    private int currentColor = 0;
    
    private int hourSize = (GRAPHIC_HEIGHT - 2*BORDER) / 24;
    private int daySize  = (GRAPHIC_LENGTH - 2*BORDER) / 7; 
    private final int nbMinutesByDay = 60*24;
    private final double minuteSize = (double)((double)hourSize * 24) / nbMinutesByDay;
    private final int decalHour = 4;
    private int nbHourLine = 24;
    private String[] weekDays = {"Sun" , "Mon" , "Tue", "Wed", "Thu", "Fri", "Sat"};
    private int widthBlock = daySize-25;
    private int decalDay = 5;
    private int debug = 0;

    public ShowApplet() throws HeadlessException {
        setBackground(colorBackground);
        setForeground(colorBackground);
    }

    public Events getEvList() {
        return evList;
    }

    public void setEvList(Events evList) {
        this.evList = evList;
    }
    
    private Color nextColor() {
        currentColor = (currentColor + 1)%colorBlock.length;
        return colorBlock[currentColor];
    }
    
    private Color getCurrentColor() {
        return colorBlock[currentColor];
    }
    
    private void drawEvents(Graphics2D g) {
        debug = (debug+1)%2;
        if(evList != null && debug==0) {
            for (Event ev : evList.getListEvents()) {
                int startDay = ModelManager.convertDayToInt(ev.getStartTime().getDay());
                drawTime(g,startDay, ev.getStartTime().getHour(), ev.getStartTime().getMinute(),
                        ev.getDuration().getDay(), ev.getDuration().getHour(), ev.getDuration().getMinute());
            }
        }
    }
    
    private void drawAxis(Graphics2D g) {
        g.setStroke(new BasicStroke(3));
        g.drawLine(AxesX, AxesY, AxesX, AxesY - GRAPHIC_HEIGHT + 2*BORDER);  
        g.drawLine(AxesX, AxesY, AxesX + GRAPHIC_LENGTH - 2*BORDER, AxesY);
    }
    
    private void drawLegend(Graphics2D g) {
       drawAxis(g);
           
       g.setStroke(new BasicStroke(1));
        int nbHourByLine = 24 / nbHourLine;
        for(int i = 0 ; i <= 24 ; i+=nbHourByLine) {
            g.drawLine(AxesX, AxesY - i * hourSize, AxesX + GRAPHIC_LENGTH - 2*BORDER , AxesY - i * hourSize);
            g.setPaint( colorText );
            g.drawString(i + "", AxesX-15, AxesY - i * hourSize + decalHour);
            g.setPaint( colorLine );
        }
        g.setPaint( colorText );
        for(int i = 0 ; i < 7 ; ++i) {
            g.drawString(weekDays[i], AxesX + i * daySize + 15, AxesY + 15);
        }
    }
    
    private void drawTime(Graphics2D g , int startDay , int startH , int startM , int durationD , int durationH , int durationM ) {
        
        int nbMinutesRemaining = durationM + 60 * durationH + 24*60*durationD;
        int currentDay = startDay;
        int minuteMin = startM + 60 * startH;
        
        g.setPaint( nextColor() );
        while(nbMinutesRemaining > 0) {
            int minuteMax = Math.min(minuteMin+nbMinutesRemaining, nbMinutesByDay);
            
            int startBlock = AxesY - (int)(minuteMin*minuteSize);
            int endBlock = AxesY - (int)(minuteMax*minuteSize);
            
            g.setPaint( getCurrentColor() );
            g.fillRect(decalDay + AxesX + currentDay * daySize , endBlock, 
                    widthBlock, startBlock - endBlock);
            g.setPaint( colorText );
            g.drawRect(decalDay + AxesX + currentDay * daySize , endBlock, 
                    widthBlock, startBlock - endBlock);
            
            currentDay = (currentDay+1)%7;
            nbMinutesRemaining -= (minuteMax-minuteMin);
            minuteMin = 0;
            decalDay++;
        }
        
    }

  public void paint(Graphics g) {
    Graphics2D g2 = (Graphics2D) g;
    g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING,
        RenderingHints.VALUE_ANTIALIAS_ON);

    g2.setPaint( colorLine );
    
    drawLegend(g2);
    drawEvents(g2);
  }

  public static void main(String s[]) {
    JFrame f = new JFrame("Planning View");
    f.addWindowListener(new WindowAdapter() {
      public void windowClosing(WindowEvent e) {
        System.exit(0);
      }
    });
    JApplet applet = new ShowApplet();
    f.getContentPane().add("Center", applet);
    f.pack();
    f.setSize(new Dimension(350, 350));
    f.show();
  }
}
