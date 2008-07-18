using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ConfigParser;

namespace AgentForAgent
{
    public partial class ConfigEditor : Form
    {
        private Configuration conf;
        private string location;

        public ConfigEditor(Configuration conf, string confLocation, string agentDir)
        {
            InitializeComponent();

            this.conf = conf;
            this.location = confLocation;

            proactiveLocation.Text = conf.agentConfig.proactiveLocation;
            
            if (conf.agentConfig.javaHome.Equals(""))
            {
                checkBox1.Checked = true;
                jvmDirectory.Enabled = false;
                jvmLocationButton.Enabled = false;
            }
            else
            {
                jvmDirectory.Text = conf.agentConfig.javaHome;
            }

            jvmParams.Text = conf.agentConfig.jvmParams;

            foreach (Event ev in conf.events.events)
            {
                CalendarEvent cEv = (CalendarEvent) ev;
                eventsList.Items.Add(makeEventName(cEv));  
            }

            Action action = conf.action;

            if (action.priority.Equals(""))
            {
                priorityBox.SelectedIndex = 0;
  //              priorityBox.SelectedItem = priorityBox.Items[priorityBox.SelectedIndex];
            }
            else
            {
                priorityBox.SelectedIndex = priorityBox.FindString(action.priority);
//                priorityBox.SelectedItem = priorityBox.Items[priorityBox.SelectedIndex];
            }


            if (action.GetType() == typeof(P2PAction))
            {
                p2pRadioButton.Select();
                P2PAction p2pAction = (P2PAction)action;
                foreach (string host in p2pAction.contacts)
                {
                    hostList.Items.Add(host);
                }

                if (hostList.Items.Count == 0)
                {
                    peerUrl.Enabled = false;
                    saveHost.Enabled = false;
                    deleteHost.Enabled = false;
                }
                else
                    hostList.SelectedIndex = 0;

                p2pProtocol.Text = p2pAction.protocol;
            }
            else if (action.GetType() == typeof(RMAction))
            {
                rmRadioButton.Select();
                RMAction rmAction = (RMAction)action;
                rmUrl.Text = rmAction.url;
            }
            else if (action.GetType() == typeof(AdvertAction))
            {
                rmiRadioButton.Select();
                AdvertAction advAction = (AdvertAction)action;
                if (advAction.nodeName.Equals(""))
                    rmiNodeEnabled.Checked = false;
                else
                    rmiNodeEnabled.Checked = true;
                rmiNodeName.Text = advAction.nodeName;
            }
        }

        private string makeEventName(CalendarEvent cEv)
        {
            return cEv.startDay.Substring(0, 3) + "/" + cEv.startHour + "/" + cEv.startMinute + "/" + cEv.startSecond;
        }

        private void p2pRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (p2pRadioButton.Checked)
            {
                p2pactionGroup.Enabled = true;
                rmiActionGroup.Enabled = false;
                rmActionGroup.Enabled = false;

                P2PAction newAction = new P2PAction();
                string[] hosts = new string[hostList.Items.Count];
                hostList.Items.CopyTo(hosts, 0);
                newAction.contacts = hosts;
                newAction.priority = (string)priorityBox.SelectedItem;
                newAction.protocol = p2pProtocol.Text;

                conf.action = newAction;
            }
        }

        private void rmiRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rmiRadioButton.Checked)
            {
                p2pactionGroup.Enabled = false;
                rmiActionGroup.Enabled = true;
                rmActionGroup.Enabled = false;

                AdvertAction newAction = new AdvertAction();
                newAction.nodeName = rmiNodeEnabled.Checked ? rmiNodeName.Text : "";
                newAction.priority = (string)priorityBox.SelectedItem;

                conf.action = newAction;
            }
        }

        private void rmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rmRadioButton.Checked)
            {
                p2pactionGroup.Enabled = false;
                rmiActionGroup.Enabled = false;
                rmActionGroup.Enabled = true;

                RMAction newAction = new RMAction();
                newAction.url = rmUrl.Text;
                newAction.priority = (string)priorityBox.SelectedItem;

                conf.action = newAction;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (rmiNodeEnabled.Checked)
                rmiNodeName.Enabled = true;
            else
                rmiNodeName.Enabled = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                jvmDirectory.Enabled = false;
                jvmLocationButton.Enabled = false;
                conf.agentConfig.javaHome = "";
            }
            else
            {
                jvmDirectory.Enabled = true;
                jvmLocationButton.Enabled = true;
            }
        }

        private void proactiveLocationButton_Click(object sender, EventArgs e)
        {
            proActiveLocationBrowser.SelectedPath = proactiveLocation.Text;
            proActiveLocationBrowser.ShowDialog();
            proactiveLocation.Text = proActiveLocationBrowser.SelectedPath;
        }

        private void jvmLocationButton_Click(object sender, EventArgs e)
        {
            jvmLocationBrowser.ShowDialog();
            jvmDirectory.Text = jvmLocationBrowser.SelectedPath;
        }

        private void eventsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
            {
                eventEditorGroup.Enabled = false;
                return;
            }
            eventEditorGroup.Enabled = true;
            CalendarEvent cEv = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];
            weekdayStart.SelectedIndex = weekdayStart.FindString(cEv.startDay);
            hourStart.Value = cEv.startHour;
            minuteStart.Value = cEv.startMinute;
            secondStart.Value = cEv.startSecond;
            dayDuration.Value = cEv.durationDays;
            hoursDuration.Value = cEv.durationHours;
            minutesDuration.Value = cEv.durationMinutes;
            secondsDuration.Value = cEv.durationSeconds;
        }

        private void deleteEventButton_Click(object sender, EventArgs e)
        {
            int selectedIdx = eventsList.SelectedIndex;
            if (selectedIdx == -1)
                return;
            eventsList.Items.RemoveAt(selectedIdx);
            conf.events.removeEvent(selectedIdx);
        }

        private void newEventButton_Click(object sender, EventArgs e)
        {
            CalendarEvent calEvent = new CalendarEvent();
            conf.events.addEvent(calEvent);
            eventsList.Items.Add("new Event");
        }

        private void updateEvent()
        {
            if (eventsList.SelectedIndex == -1)
                return;

            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];
            calEvent.durationSeconds = (int)secondsDuration.Value;
            calEvent.durationMinutes = (int)minutesDuration.Value;
            calEvent.durationHours = (int)hoursDuration.Value;
            calEvent.durationDays = (int)dayDuration.Value;
            calEvent.startDay = (string) weekdayStart.SelectedItem;
            calEvent.startHour = (int)hourStart.Value;
            calEvent.startMinute = (int)minuteStart.Value;
            calEvent.startSecond = (int)secondStart.Value;
            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);

            // change name in the event list control

            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);
        }

        private void saveEventButton_Click(object sender, EventArgs e)
        {
            updateEvent();
        }

        private void closeConfig_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigurationParser.saveXml(location, conf);
            }
            catch (Exception)
            {
                MessageBox.Show("");
            }
            Close();
        }

        private void proactiveLocation_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.proactiveLocation = proactiveLocation.Text;
        }

        private void jvmDirectory_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.javaHome = jvmDirectory.Text;
        }

        private void jvmParams_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.jvmParams = jvmParams.Text;
        }

        private void rmiNodeName_TextChanged(object sender, EventArgs e)
        {
            AdvertAction ourAction = (AdvertAction) conf.action;
            ourAction.nodeName = rmiNodeName.Text;
        }

        private void rmUrl_TextChanged(object sender, EventArgs e)
        {
            RMAction ourAction = (RMAction)conf.action;
            ourAction.url = rmUrl.Text;
        }

        private void hostList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hostList.SelectedIndex == -1)
            {
                peerUrl.Enabled = false;
                saveHost.Enabled = false;
                deleteHost.Enabled = false;
                return;
            }
            peerUrl.Enabled = true;
            saveHost.Enabled = true;
            deleteHost.Enabled = true;

            peerUrl.Text = (string) hostList.Items[hostList.SelectedIndex];
        }

        private void saveHost_Click(object sender, EventArgs e)
        {
            P2PAction ourAction = (P2PAction)conf.action;
            hostList.Items[hostList.SelectedIndex] = peerUrl.Text;
            ourAction.modifyContact(hostList.SelectedIndex, peerUrl.Text);
        }

        private void addHost_Click(object sender, EventArgs e)
        {
            P2PAction ourAction = (P2PAction)conf.action;
            hostList.Items.Add("newPeer");
            ourAction.addContact("newPeer");
        }

        private void deleteHost_Click(object sender, EventArgs e)
        {
            P2PAction ourAction = (P2PAction)conf.action;
            int index = hostList.SelectedIndex;
            hostList.Items.RemoveAt(index);
            ourAction.deleteContact(index);
        }

        private void priorityBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (priorityBox.SelectedIndex == -1)
                conf.action.priority = "";
            else
                conf.action.priority = (string) priorityBox.Items[priorityBox.SelectedIndex];
        }

        private void p2pProtocol_TextChanged(object sender, EventArgs e)
        {
            P2PAction ourAction = (P2PAction)conf.action;
            ourAction.protocol = p2pProtocol.Text;
        }




    }
}
