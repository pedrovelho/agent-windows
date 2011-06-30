/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Utils;

import java.io.File;

/**
 *
 * @author pgouttef
 */
public class FileLocator {
    
    private String path = "";
    
    /**
     * 
     * @param f : startPoint to search the file
     * @param fileName : file name
     */
    private void searchFile(File f , String fileName)
	{
		File[] list_child = f.listFiles();
                
		if(list_child!=null)
		{
			for(File child:list_child)
			{
                            if(child.getName().equals( fileName )) {
                                path = child.getAbsolutePath();
                            }
                            searchFile(child,fileName);
			}
		}
	}
    
    /**
     * search in the user folder the file.
     * @param fileName : file to find
     * @return path : absolute file's path 
     */
    public String getPathOfFile(String fileName) {
        
        File config = new File( System.getProperty("user.dir") );
        searchFile(config, fileName);
        return path;
    }
    
    /**
     * 
     * @return the result of the last search 
     */
    public String getLastPath() {
        return path;
    }
    
    public static void main(String argv[]) {
        
        FileLocator loc = new FileLocator();
        loc.getPathOfFile("icon.ico");
        System.out.println( loc.getLastPath());
    }
}
