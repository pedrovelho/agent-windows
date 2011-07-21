#!/usr/bin/env python
  
#
# ################################################################
#
# ProActive Parallel Suite(TM): The Java(TM) library for
#    Parallel, Distributed, Multi-Core Computing for
#    Enterprise Grids & Clouds
#
# Copyright (C) 1997-2011 INRIA/University of
#                 Nice-Sophia Antipolis/ActiveEon
# Contact: proactive@ow2.org or contact@activeeon.com
#
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Affero General Public License
# as published by the Free Software Foundation; version 3 of
# the License.
#
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Affero General Public License for more details.
#
# You should have received a copy of the GNU Affero General Public License
# along with this library; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
# USA
#
# If needed, contact us to obtain a release under GPL Version 2 or 3
# or a different license than the AGPL.
#
#  Initial developer(s):               The ActiveEon Team
#                        http://www.activeeon.com/
#  Contributor(s):
#
# ################################################################ 
# $$ACTIVEEON_CONTRIBUTOR$$
#

import login
import commands
  
# The model of the Daemon, contain all functions.
class Model:
    
    config_file = '/usr/bin/PAAgent/log.txt'
    
    def launcher(self,command):
        user = commands.getoutput( 'whoami' )
        log = login.Login()
        if log.checkAccess(user):
            if command == 'ScreenSaver':
                print 'ss'
                self.launchScreenSaver()
                
            if command == 'startJVM':
                print 'startJVM'
         	self.startJVM(user)
                
            if command == 'stopJVM':
                print 'stopJVM'
                self.stopJVM(user)
                
        else:
            print 'acces denied' 
    
    def launchScreenSaver(self):
        command = 'gnome-screensaver-command --activate'
        result = commands.getstatusoutput(command)
        print result
        
    def startJVM(self,user):
        f = open( self.config_file ,'a')
        current_time = commands.getstatusoutput( 'date' )
	line = user + ' started JVM at : ' + str(current_time[1]) + '\n'
        f.write( line )
        f.close() 
    
    def stopJVM(self,user):
        f = open( self.config_file ,'a')
        current_time = commands.getstatusoutput( 'date' )
        line = user + ' stopped JVM at : ' + str(current_time[1]) + '\n'
        f.write( line )
        f.close()   
    
    
