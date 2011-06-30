/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Main;

import GUI.GUIEditorWindows;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;

/**
 *
 * @author fifi
 */
public class main {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] argv) {
        
    try {
	// Set System L&F
        UIManager.setLookAndFeel(
            UIManager.getSystemLookAndFeelClassName());
        } 
        catch (UnsupportedLookAndFeelException e) {
           // handle exception
        }
        catch (ClassNotFoundException e) {
           // handle exception
        }
        catch (InstantiationException e) {
           // handle exception
        }
        catch (IllegalAccessException e) {
           // handle exception
        }
        
        GUIEditorWindows cEUI = null;
        if(argv.length == 1) {
            cEUI = new GUIEditorWindows(argv[0]);
        } else {
            cEUI = new GUIEditorWindows();
        }
    
        
        cEUI.setVisible(true);
    }
}
