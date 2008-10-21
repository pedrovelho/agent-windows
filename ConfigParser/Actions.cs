using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * Collection of actions on which ProActive agent will react
 */

namespace ConfigParser
{
    public class Actions
    {
        private List<Action> myActions = new List<Action>();

        [XmlElement("action", IsNullable = false)]
        public Action[] actions
        {
            get
            {
                Action[] actions = new Action[myActions.Count];
                myActions.CopyTo(actions);
                return actions;
                //return null;
            }
            set
            {
                if (value == null) return;
                Action[] actions = (Action[])value;
                myActions.Clear();
                foreach (Action aAction in actions)
                    myActions.Add(aAction);
            }
        }

        public void removeAction(int index)
        {
            myActions.RemoveAt(index);
        }

        public void addAction(Action actiont)
        {
            myActions.Add(actiont);
        }

        public void modifyAction(int index, Action actiont)
        {
            myActions[index] = actiont;
        }

    }
}
