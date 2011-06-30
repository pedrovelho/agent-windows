/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package GUI;

import snoozesoft.systray4j.SysTrayMenu;
import snoozesoft.systray4j.SysTrayMenuIcon;
import snoozesoft.systray4j.SysTrayMenuItem;

/**
 *
 * @author pgouttef
 */
public class SystemTray {
  
    private SysTrayMenu menu;
    private String toolTlip = "ProActive Agent Control";
    
    private SysTrayMenuIcon iconStopped = new SysTrayMenuIcon(getClass().getResource("/GUI/icon.ico"));
    private SysTrayMenuIcon iconRunning = new SysTrayMenuIcon(getClass().getResource("/GUI/icon_blue.ico"));
    
    public static enum STATUS { RUNNING , STOPPED , ERROR };
    private STATUS currentStatus = STATUS.STOPPED; 
    
    private SysTrayMenuItem itemStart = new SysTrayMenuItem("Start service");
    private SysTrayMenuItem itemStop = new SysTrayMenuItem("Stop service");
    private SysTrayMenuItem itemAuto = new SysTrayMenuItem("Automatic launch");
    private SysTrayMenuItem itemClose = new SysTrayMenuItem("Close Administration panel");
    
    /**
     * The default construstor check System platform and do nothing on Linux platform
     */
    public SystemTray() {
        
        String os = System.getProperty("os.name").toLowerCase();
        
        if(os.indexOf( "win" ) >= 0) {
            System.out.println("You are running SystemTray Icons on Windows.");
            menu = new SysTrayMenu(iconStopped, toolTlip);

            menu.addItem(itemClose,0);
            menu.addSeparator(1);
            menu.addItem(itemAuto,2);
            menu.addItem(itemStop,3);
            menu.addItem(itemStart,4);

            menu.showIcon();
        } else {
            System.out.println("SystemTray Icons not enabled yet on your System.");
        }
    }

    public SysTrayMenu getMenu() {
        return menu;
    }
    
    /**
     * update icone in system tray
     * @param status 
     */
    public void setIcon( STATUS status ) {
        
        currentStatus = status;
        
        String os = System.getProperty("os.name").toLowerCase();
        if(os.indexOf( "win" ) >= 0) {
        
            switch (currentStatus) {

                case STOPPED:   menu.setIcon(iconStopped);
                                break;
                case RUNNING:   menu.setIcon(iconRunning);
                                break;
                case ERROR:
                                break;

            }
        }
    }
    
    public static void main(String argv[]) throws InterruptedException { 
    
        SystemTray sysTray = new SystemTray();
        Thread.sleep(15000);
        sysTray.getMenu().hideIcon();
    }
    
}
