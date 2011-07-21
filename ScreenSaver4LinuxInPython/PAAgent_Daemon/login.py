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

import commands
import string
import ConfigParser

# A class Login to check differents access of the Daemon using ConfigParser standard API of Python
class Login:
    
    def checkAccess(self,username): 
        config = ConfigParser.RawConfigParser()
        
        config.read( '/usr/bin/PAAgent/config.txt' )
        items_users = config.items('USERS-ACCESS')
        items_groups = config.items('GROUPS-ACCESS')

	right = False

	user_groups = commands.getoutput('groups' + username)
	user_groups_list = user_groups.split()
	#Get only the group name
	user_groups_list = user_groups_list[2::]

	# Tests for group name
        for group_l in items_groups: 
	    for group_u in user_groups_list:
		    # user[NAME,RIGHTS]
		    if group_l[1] == 'yes' and group_l[0] == group_u:
		        right = True

	# Tests for user name
	for user_l in items_users: 
	    # user[NAME,RIGHTS]
	    if user_l[1] == 'yes' and user_l[0] == username:
	        right = True
            if user_l[1] == 'no' and user_l[0] == username:
	        right = False
            
        return right
