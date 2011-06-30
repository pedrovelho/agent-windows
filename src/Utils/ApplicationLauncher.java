/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Utils;

import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author pgouttef
 */
public class ApplicationLauncher {
    
    private String ScreenSaver4Windows = "ProActiveSSaver.scr"; 
    private String ScreenSaver4Linux = "gnome-screensaver-command";
    
    /**
     * Default constructor
     * 
     */
    public ApplicationLauncher() {
        
        ScreenSaver4Windows = new FileLocator().getPathOfFile(ScreenSaver4Windows);
    }
    
    /**
     * Launch an application with specifics parameters
     * @param params : parameters of executable 
     */
    public void launch(String[] params) {
        try {
            if(params.length == 1) {
                System.out.println(params[0] + " starting...");
            } else if(params.length == 2) {
                System.out.println(params[0] + " " + params[1] + " starting...");
            }
            Process proc = Runtime.getRuntime().exec( params );
            System.out.println(params[0] + " is ready to use");
        } catch (IOException ex) {
            Logger.getLogger(ApplicationLauncher.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    public void openDirectory(String dir) {
        try {
            Process p = Runtime.getRuntime().exec("open " + dir);
        } catch (IOException ex) {
            Logger.getLogger(ApplicationLauncher.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    /**
     * Launch screen saver on any platforms
     */
    public void launchScreenSaver() {
        
        String os = System.getProperty("os.name").toLowerCase();
        if(os.indexOf( "win" ) >= 0) {
        
            launch(ScreenSaver4Windows);
        } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {
            
            String[] params = new String[2];
        
            params[0] = ScreenSaver4Linux;
            params[1] = "--activate"; // To active screenSaver on Linux
            launch(params);
        }
        
    }
    
    /**
     * Launch a simple application without parameter
     * @param param : application's name
     */
    public void launch(String param) {
        try {
            System.out.println(param + " starting...");
            Process proc = Runtime.getRuntime().exec( param );
            System.out.println(param + " is ready to use");
        } catch (IOException ex) {
            Logger.getLogger(ApplicationLauncher.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    /**
     * Execute a text editor on any platform
     * @param paramFile : text file to open
     */
    public void executeEditor(String paramFile) {
        
        String os = System.getProperty("os.name").toLowerCase();
        String[] params = new String[2];
        if(os.indexOf( "win" ) >= 0) {
        
            params[0] = "notepad.exe";
            params[1] = paramFile;
        } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {
            params[0] = "gedit";
            params[1] = paramFile;
        }
        
        launch(params);
    }
    
    /**
     * Execute a browser on any platform
     * @param paramFile : text file to open
     */
    public void executeBrowser(String paramFile) {
        
        String os = System.getProperty("os.name").toLowerCase();
        String[] params = new String[2];
        if(os.indexOf( "win" ) >= 0) {
        
            params[0] = "explorer.exe";
            params[1] = paramFile;
        } else if (os.indexOf( "nix"  ) >=0 || os.indexOf( "nux" ) >=0) {
            params[0] = "firefox";
            params[1] = paramFile;
        }
        
        launch(params);
    }
    
    public static void main(String argv[]) throws IOException {
        
        ApplicationLauncher app = new ApplicationLauncher();
        app.launchScreenSaver();
    }
    
}
