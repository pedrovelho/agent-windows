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

        public override string[] getArgs(int processRank)
        {
            return this.myArgs;
        }
    }
}
