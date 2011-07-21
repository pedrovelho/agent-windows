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

#

from gobject import MainLoop
from dbus import SessionBus
from dbus.mainloop.glib import DBusGMainLoop
from subprocess import Popen
import commands

from daemon import Daemon
 
import sys, time
 
class MyDaemon(Daemon):
        def run(self):
                while True:
                        time.sleep(1)

# A listenner of gnome screensaver status
class SSTrigger:
    def __init__(self):
        DBusGMainLoop(set_as_default=True)
        self.mem='ActiveChanged'
        self.dest='org.gnome.ScreenSaver'
        self.bus=SessionBus()
        self.loop=MainLoop()
        self.bus.add_signal_receiver(self.catch,self.mem,self.dest)
    def catch(self,ssOn):
        daemon = MyDaemon('/tmp/daemon-example.pid')

        if ssOn == 1: #Screensaver turned on
	    print 'Turn On'
            daemon.startJVM()
        else: #Screensaver turned off
	    print 'Turn Off'
            daemon.stopJVM()

SSTrigger().loop.run()
