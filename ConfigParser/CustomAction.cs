/*
* ################################################################
*
* ProActive: The Java(TM) library for Parallel, Distributed,
*            Concurrent computing with Security and Mobility
*
* Copyright (C) 1997-2009 INRIA/University of Nice-Sophia Antipolis
* Contact: proactive@ow2.org
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either version
* 2 of the License, or any later version.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this library; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
*
*  Initial developer(s):               The ProActive Team
*                        http://proactive.inria.fr/team_members.htm
*  Contributor(s): ActiveEon Team - http://www.activeeon.com
*
* ################################################################
* $$ACTIVEEON_CONTRIBUTOR$$
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ConfigParser
{
    public class CustomAction : Action
    {
        /** The string description of this action **/
        public const string DESCRIPTION = "Custom";
        /// <summary>
        /// The java class that corresponds to this action.</summary>
        public const string DEFAULT_JAVA_STARTER_CLASS = "user.Starter";
        /// <summary>
        /// The arguments used for this custom conenction.</summary>
        private string[] myArgs;

        public CustomAction()
        {
            base.javaStarterClass = DEFAULT_JAVA_STARTER_CLASS;
            this.args = new string[0];
        }

        [XmlArray("args", IsNullable = false)]
        [XmlArrayItem("arg", IsNullable = true)]
        public String[] args
        {
            get
            {
                return this.myArgs;
            }
            set
            {
                this.myArgs = value;
            }
        }

        public override string[] getArgs()
        {
            return this.myArgs;
        }
    }
}
