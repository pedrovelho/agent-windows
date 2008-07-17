using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

/**
 * In P2PAction a machine registers itself in P2P network
 * The configuration contains a list of first-contact
 * nodes that P2P service will be initialized with
 */

namespace ConfigParser
{
    public class P2PAction : Action
    {
        private List<String> myContacts = new List<string>();
        private String myProtocol = "RMI"; // maybe this will be configurable in the future

        [XmlAttribute("protocol")]
        public String protocol
        {
            get
            {
                return myProtocol;
            }
            set
            {
                myProtocol = value;
            }
        }

        [XmlElement("contact")]
        public String[] contacts
        {
            get
            {
                String[] contacts = new String[myContacts.Count];
                myContacts.CopyTo(contacts);
                return contacts;
            }
            set
            {
                if (value == null) return;
                String[] contacts = (String[])value;
                myContacts.Clear();
                foreach (String contact in contacts)
                    myContacts.Add(contact);
            }
        }

        public void modifyContact(int index, string value)
        {
            myContacts[index] = value;
        }

        public void addContact(string value)
        {
            myContacts.Add(value);
        }

        public void deleteContact(int index)
        {
            myContacts.RemoveAt(index);
        }

// THE FOLLOWING CODE IS DEPRECATED 
/*        public void addContact(String contact)
        {
            contacts.Add(contact);
        }

        public List<String> getContacts()
        {
            return contacts;
        } */
    }
}
