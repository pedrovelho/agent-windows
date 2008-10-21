using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ConfigParser;
using Microsoft.Win32;

namespace AgentForAgent
{
    public partial class ConfigEditor : Form
    {
        private Configuration conf;
        private string location;
        private ConfigurationDialog hook;
        private Chart chart;
        private string[] usersList;

        public ConfigEditor(Configuration conf, string confLocation, string agentDir, ConfigurationDialog hook)
        {
            InitializeComponent();

            this.conf = conf;
            this.location = confLocation;
            this.hook = hook;

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
                CalendarEvent cEv = (CalendarEvent)ev;
                eventsList.Items.Add(makeEventName(cEv));
            }

            foreach (Action act in conf.actions.actions)
            {
                actionsList.Items.Add(act.GetType().Name);
            }

            p2pactionGroup.Visible = false;
            rmiActionGroup.Visible = false;
            rmActionGroup.Visible = false;

            //--Chart
            chart = new Chart(ref conf);
         
            //--Users list
            string users_reg_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\DocFolderPaths";
            //The registry key for reading user list.
            RegistryKey key = Registry.LocalMachine.OpenSubKey(users_reg_key, true);
            //string[] winusers;
            if (key != null && key.ValueCount > 0)
            {
                usersList = key.GetValueNames();
            }

            for (int i = 0; i < usersList.Length; i++)
            {
                //MessageBox.Show(usersList[i]);
                userBox.Items.Add(usersList[i]);
            }
        }

        private string makeEventName(CalendarEvent cEv)
        {
            //--Compute after duration
            int finishSecond = 0;
            int finishMinute = 0;
            int finishHour = 0;
            string finishDay = "";

            finishSecond = cEv.startSecond;
            finishMinute = cEv.startMinute;
            finishHour = cEv.startHour;

            finishSecond += cEv.durationSeconds;
            if (finishSecond >= 60)
            {
                finishMinute += finishSecond - 60;
                finishSecond -= 60;
            }

            finishMinute += cEv.durationMinutes;
            if (finishMinute >= 60)
            {
                finishHour += finishMinute - 60;
                finishMinute -= 60;
            }

            finishHour += cEv.durationHours;
            if (finishHour >= 24)
            {
                finishDay = resolveDayIntToString((int)(((cEv.resolveDay() + cEv.durationDays) + 1) % 7));
                finishHour -= 24;
            }
            else
            {
                finishDay = resolveDayIntToString((int)((cEv.resolveDay() + cEv.durationDays) % 7));
            }
            
            //return cEv.startDay.Substring(0, 3) + "/" + cEv.startHour + "/" + cEv.startMinute + "/" + cEv.startSecond;
            return cEv.startDay + " - " + formatDate(cEv.startHour) + ":" + formatDate(cEv.startMinute) + ":" + formatDate(cEv.startSecond) + " => " + finishDay + " - " + formatDate(finishHour) + ":" + formatDate(finishMinute) + ":" + formatDate(finishSecond);
        }

        public static string resolveDayIntToString(int day)
        {
            if (day == 5)
                return "friday";
            if (day == 1)
                return "monday";
            if (day == 6)
                return "saturday";
            if (day == 0)
                return "sunday";
            if (day == 4)
                return "thursday";
            if (day == 2)
                return "tuesday";
            if (day == 3)
                return "wednesday";
            return "";
        }

        private static string formatDate(int num)
        {
            if (num < 10)
                return "0" + num.ToString();
            return num.ToString();
        }

        private void actionTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            switch ((string)(actionTypeBox.SelectedItem))
            {
                case "RMI Registration":
                    {
                        p2pactionGroup.Enabled = false;
                        rmiActionGroup.Enabled = true;
                        rmActionGroup.Enabled = false;

                        p2pactionGroup.Visible = false;
                        rmiActionGroup.Visible = true;
                        rmActionGroup.Visible = false;
                        
                        AdvertAction newAction = new AdvertAction();
                        newAction.nodeName = rmiNodeEnabled.Checked ? rmiNodeName.Text : "";
                        newAction.priority = (string)priorityBox.SelectedItem;

                        if (actionsList.SelectedIndex != -1)
                        {
                            conf.actions.actions[actionsList.SelectedIndex] = newAction;
                        }
                        
                        //conf.action = newAction;
                    }
                    break;

                case "Peer-To-Peer":
                    {
                        p2pactionGroup.Enabled = true;
                        rmiActionGroup.Enabled = false;
                        rmActionGroup.Enabled = false;

                        p2pactionGroup.Visible = true;
                        rmiActionGroup.Visible = false;
                        rmActionGroup.Visible = false;

                        P2PAction newAction = new P2PAction();
                        string[] hosts = new string[hostList.Items.Count];
                        hostList.Items.CopyTo(hosts, 0);
                        newAction.contacts = hosts;
                        newAction.priority = (string)priorityBox.SelectedItem;
                        newAction.protocol = p2pProtocol.Text;

                        if (actionsList.SelectedIndex != -1)
                        {
                            conf.actions.actions[actionsList.SelectedIndex] = newAction;
                            //actionsList.Items[actionsList.SelectedIndex] = newAction;
                            //conf.action = newAction;
                        }
                    }
                    break;

                case "Resource Manager Registration":
                    {
                        p2pactionGroup.Enabled = false;
                        rmiActionGroup.Enabled = false;
                        rmActionGroup.Enabled = true;

                        p2pactionGroup.Visible = false;
                        rmiActionGroup.Visible = false;
                        rmActionGroup.Visible = true;

                        RMAction newAction = new RMAction();
                        newAction.url = rmUrl.Text;
                        newAction.nodeName = rmNodeName.Text;
                        newAction.priority = (string)priorityBox.SelectedItem;

                        if (actionsList.SelectedIndex != -1)
                        {
                            conf.actions.actions[actionsList.SelectedIndex] = newAction;
                            //conf.action = newAction;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);
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
        //--EVENTS LIST
        private void eventsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
            {
                eventEditorGroup.Enabled = false;
                return;
            }
            eventEditorGroup.Enabled = true;
            CalendarEvent cEv = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];
            if (weekdayStart.FindString(cEv.startDay) == -1)
                weekdayStart.SelectedIndex = 0;
            else
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
            // calEvent.startDay = (string)weekdayStart.SelectedItem;
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
            calEvent.startDay = (string)weekdayStart.SelectedItem;
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
            saveEventButton.Enabled = false;
        }

        private void closeConfig_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveConfig_Click(object sender, EventArgs e)
        {
            //--Events list
            int i = 0;
            foreach (Event item in conf.events.events)
            {
                if (((CalendarEvent)item).startDay == null)
                {
                    //Delete event
                    conf.events.removeEvent(i);
                }
                else
                    i++;

            }

            try
            {
                ConfigurationParser.saveXml(location, conf);
            }
            catch (Exception)
            {
                MessageBox.Show("");
            }

            MessageBox.Show("Service must be restarted to apply changes.", "Restart service", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void saveConfigAs_Click(object sender, EventArgs e)
        {
            //--Show dialog form
            saveFileDialog1.Filter = "Xml File|*.xml";
            saveFileDialog1.Title = "Save an xml configuration file";
            saveFileDialog1.ShowDialog();
            string locationAs = "";
            locationAs = saveFileDialog1.FileName;
            

            if (locationAs != "")
            {
                /*browseConfig.FileName = configLocation.Text;
                browseConfig.ShowDialog();
                configLocation.Text = browseConfig.FileName;*/

                //--Events list
                int i = 0;
                foreach (Event item in conf.events.events)
                {
                    if (((CalendarEvent)item).startDay == null)
                    {
                        //Delete event
                        conf.events.removeEvent(i);
                    }
                    else
                        i++;

                }

                try
                {
                    ConfigurationParser.saveXml(locationAs, conf);
                    location = locationAs;
                    hook.setConfigLocation(locationAs);
                }
                catch (Exception)
                {
                    MessageBox.Show("");
                }

                MessageBox.Show("Service must be restarted to apply changes.", "Restart service", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
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

            peerUrl.Text = (string)hostList.Items[hostList.SelectedIndex];
        }

        private void saveHost_Click(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            P2PAction ourAction = (P2PAction)conf.actions.actions[actionsList.SelectedIndex];
            //P2PAction ourAction = (P2PAction)conf.action;
            hostList.Items[hostList.SelectedIndex] = peerUrl.Text;
            ourAction.modifyContact(hostList.SelectedIndex, peerUrl.Text);
        }

        private void addHost_Click(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            P2PAction ourAction = (P2PAction)conf.actions.actions[actionsList.SelectedIndex];
            //P2PAction ourAction = (P2PAction)conf.action;
            hostList.Items.Add("newPeer");
            ourAction.addContact("newPeer");
        }

        private void deleteHost_Click(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            P2PAction ourAction = (P2PAction)conf.actions.actions[actionsList.SelectedIndex];
            //P2PAction ourAction = (P2PAction)conf.action;
            int index = hostList.SelectedIndex;
            hostList.Items.RemoveAt(index);
            ourAction.deleteContact(index);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart.loadEvents();
            chart.Show();
        }

        private void actionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (actionsList.SelectedIndex == -1)
            {
                actionEditorGroup.Enabled = false;
                return;
            }
            actionEditorGroup.Enabled = true;
            Action action = (Action)conf.actions.actions[actionsList.SelectedIndex];


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
            userBox.SelectedIndex = userBox.FindString(action.user);

            if (action.GetType() == typeof(P2PAction))
            {
                actionTypeBox.SelectedItem = "Peer-To-Peer";

                P2PAction p2pAction = (P2PAction)action;
                //--Vide
                hostList.Items.Clear();
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
                actionTypeBox.SelectedItem = "Resource Manager Registration";
                RMAction rmAction = (RMAction)action;
                rmUrl.Text = rmAction.url;
                rmNodeName.Text = rmAction.nodeName;
            }
            else if (action.GetType() == typeof(AdvertAction))
            {
                actionTypeBox.SelectedItem = "RMI Registration";
                AdvertAction advAction = (AdvertAction)action;
                if (advAction.nodeName.Equals(""))
                    rmiNodeEnabled.Checked = false;
                else
                    rmiNodeEnabled.Checked = true;
                rmiNodeName.Text = advAction.nodeName;
            }
            else
            {
                p2pactionGroup.Enabled = false;
                rmiActionGroup.Enabled = false;
                rmActionGroup.Enabled = false;

                p2pactionGroup.Visible = false;
                rmiActionGroup.Visible = false;
                rmActionGroup.Visible = false;

                actionTypeBox.SelectedIndex = -1;
                rmUrl.Text = "";
                rmNodeName.Text = "";

                rmiNodeName.Text = "";
                rmiNodeEnabled.Enabled = true;
                hostList.Items.Clear();

                p2pProtocol.Text = "";
                peerUrl.Text = "";
                hostList.Items.Clear();
            }
            saveActionButton.Enabled = false;
        }

        private void saveActionButton_Click(object sender, EventArgs e)
        {
            updateAction();
            saveActionButton.Enabled = false;
        }

        private void updateAction()
        {
            if (actionsList.SelectedIndex == -1)
                return;

            switch ((string)(actionTypeBox.SelectedItem))
            {
                case "RMI Registration":
                    {
                        AdvertAction newAction = new AdvertAction();
                        //newAction.nodeName = rmiNodeEnabled.Checked ? rmiNodeName.Text : "";
                        if (rmiNodeEnabled.Checked)
                        {
                            newAction.nodeName = rmiNodeName.Text;
                        }
                        else
                        {
                            newAction.nodeName = "";
                        }

                        newAction.priority = (string)priorityBox.SelectedItem;
                        newAction.user = (string)userBox.SelectedItem;

                        if (actionsList.SelectedIndex != -1)
                        {
                            conf.actions.modifyAction(actionsList.SelectedIndex, newAction);
                            actionsList.Items[actionsList.SelectedIndex] = newAction.GetType().Name;
                        }
                    }
                    break;

                case "Peer-To-Peer":
                    {
                        P2PAction newAction = new P2PAction();
                        string[] hosts = new string[hostList.Items.Count];
                        hostList.Items.CopyTo(hosts, 0);
                        newAction.contacts = hosts;
                        newAction.priority = (string)priorityBox.SelectedItem;
                        newAction.user = (string)userBox.SelectedItem;

                        newAction.protocol = p2pProtocol.Text;

                        if (actionsList.SelectedIndex != -1)
                        {
                            conf.actions.modifyAction(actionsList.SelectedIndex, newAction);
                            actionsList.Items[actionsList.SelectedIndex] = newAction.GetType().Name;
                        }
                    }
                    break;

                case "Resource Manager Registration":
                    {
                        p2pactionGroup.Enabled = false;
                        rmiActionGroup.Enabled = false;
                        rmActionGroup.Enabled = true;

                        p2pactionGroup.Visible = false;
                        rmiActionGroup.Visible = false;
                        rmActionGroup.Visible = true;

                        RMAction newAction = new RMAction();
                        newAction.url = rmUrl.Text;
                        newAction.nodeName = rmNodeName.Text;
                        newAction.priority = (string)priorityBox.SelectedItem;
                        newAction.user = (string)userBox.SelectedItem;

                        if (actionsList.SelectedIndex != -1)
                        {
                            conf.actions.modifyAction(actionsList.SelectedIndex, newAction);
                            actionsList.Items[actionsList.SelectedIndex] = newAction.GetType().Name;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void weekdayStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveEventButton.Enabled = true;
        }


        private void eventChanged(object sender, EventArgs e)
        {
            saveEventButton.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Action action = new Action();
            action.priority = "";
            conf.actions.addAction(action);
            actionsList.Items.Add("new Action");
            /*p2pactionGroup.Enabled = false;
            rmiActionGroup.Enabled = false;
            rmActionGroup.Enabled = false;

            p2pactionGroup.Visible = false;
            rmiActionGroup.Visible = false;
            rmActionGroup.Visible = false;

            actionTypeBox.SelectedIndex = -1;*/
        }

        private void deleteActionButton_Click(object sender, EventArgs e)
        {
            int selectedIdx = actionsList.SelectedIndex;
            if (selectedIdx == -1)
                return;
            actionsList.Items.RemoveAt(selectedIdx);
            conf.actions.removeAction(selectedIdx);
        }


        private void actionPropertyChanged(object sender, EventArgs e)
        {
            saveActionButton.Enabled = true;
        }

        private void ConfigEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
