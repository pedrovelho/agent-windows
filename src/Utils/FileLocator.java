/*
 * ################################################################
 *
 * ProActive Parallel Suite(TM): The Java(TM) library for
 *    Parallel, Distributed, Multi-Core Computing for
 *    Enterprise Grids & Clouds
 *
 * Copyright (C) 1997-2011 INRIA/University of
 *                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 *
 * If needed, contact us to obtain a release under GPL Version 2 or 3
 * or a different license than the AGPL.
 *
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################ 
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
package Utils;

import java.io.File;

/**
 *
 * @author philippe Gouttefarde
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
