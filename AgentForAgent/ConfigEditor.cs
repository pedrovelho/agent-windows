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

        private string _typeOfNewAction;
        public string typeofNewAction
        {
            get { return _typeOfNewAction; }
            set { _typeOfNewAction = value; }
        }

        private IniFile configuration;
        //--Constructor
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
                if (cEv.durationDays == 6 && cEv.durationHours == 23 && cEv.durationMinutes == 59 && cEv.durationSeconds == 59)
                {
                    checkBox2.Checked = true;
                    eventEditorGroup.Enabled = false;
                    eventsList.Enabled = false;
                    newEventButton.Enabled = false;
                    deleteEventButton.Enabled = false;
                }
                eventsList.Items.Add(makeEventName(cEv));
            }

            // Iterate through all actions in configuration then 
            // add its description in the actions list
            foreach (ConfigParser.Action act in conf.actions.actions)
            {
                if (act.GetType() == typeof(AdvertAction))
                {
                    actionsList.Items.Add(AdvertAction.DESCRIPTION);
                }
                else if (act.GetType() == typeof(RMAction))
                {
                    actionsList.Items.Add(RMAction.DESCRIPTION);
                }
                else if (act.GetType() == typeof(P2PAction))
                {
                    actionsList.Items.Add(P2PAction.DESCRIPTION);
                }
                else
                {
                    // Unknown action
                };
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

            configuration = new IniFile("configuration.ini");
        }

        /**************************************************************
        * WINDOWS                                                     *
        * ************************************************************/
        //--Click to "Cancel" button
        private void closeConfig_Click(object sender, EventArgs e)
        {
            Close();
        }

        //--Form close
        private void ConfigEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        //--Save current config
        private void saveConfig_Click(object sender, EventArgs e)
        {
            saveConfig.Enabled = false;
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



            if (configuration.GetValue("params", "saveWarning") != "0")
            {
                DialogResult res = MessageBox.Show("Service must be restarted to apply changes.\nDisplay again this message ?", "Restart service", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.No)
                {
                    configuration.SetValue("params", "saveWarning", "0");
                    configuration.Save();
                }
            }
            //Close();
        }

        //--Save as new file
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
                    MessageBox.Show("Cannot save your file");
                }

                if (configuration.GetValue("params", "saveWarning") != "0")
                {
                    DialogResult res = MessageBox.Show("Service must be restarted to apply changes.\nDisplay again this message ?", "Restart service", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.No)
                    {
                        configuration.SetValue("params", "saveWarning", "0");
                        configuration.Save();
                    }
                }
                Close();
            }
        }

        /**************************************************************
        * PROACTIVE CONFIGURATION                                     *
        * ************************************************************/
        //--Checkbox (Use system-wide JVM location)
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
            saveConfig.Enabled = true;
        }

        //--Show dialog box to select proactive path
        private void proactiveLocationButton_Click(object sender, EventArgs e)
        {
            proActiveLocationBrowser.SelectedPath = proactiveLocation.Text;
            proActiveLocationBrowser.ShowDialog();
            proactiveLocation.Text = proActiveLocationBrowser.SelectedPath;
        }

        //--Show dialog box to select jvm path
        private void jvmLocationButton_Click(object sender, EventArgs e)
        {
            jvmLocationBrowser.ShowDialog();
            jvmDirectory.Text = jvmLocationBrowser.SelectedPath;
        }

        //--Update config if it change
        private void proactiveLocation_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.proactiveLocation = proactiveLocation.Text;

            saveConfig.Enabled = true;
        }

        //--Update config if it change
        private void jvmDirectory_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.javaHome = jvmDirectory.Text;

            saveConfig.Enabled = true;
        }

        //--Update config if it change
        private void jvmParams_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.jvmParams = jvmParams.Text;

            saveConfig.Enabled = true;
        }

        /**************************************************************
        * EVENTS                                                      *
        * ************************************************************/
        //--Make the name of the event for the listBox
        private string makeEventName(CalendarEvent cEv)
        {
            if (cEv.durationDays == 6 && cEv.durationHours == 23 && cEv.durationMinutes == 59 && cEv.durationSeconds == 59)
            {
                return "Always available";
            }
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

        //--Fill the fields with the values of the selected event
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

        //--Delete event
        private void deleteEventButton_Click(object sender, EventArgs e)
        {
            int selectedIdx = eventsList.SelectedIndex;
            if (selectedIdx == -1)
                return;
            eventsList.Items.RemoveAt(selectedIdx);
            conf.events.removeEvent(selectedIdx);

            saveConfig.Enabled = true;
        }

        //--Add event
        private void newEventButton_Click(object sender, EventArgs e)
        {
            CalendarEvent calEvent = new CalendarEvent();
            // calEvent.startDay = (string)weekdayStart.SelectedItem;
            conf.events.addEvent(calEvent);
            eventsList.Items.Add("new Event");
            //updateEvent();
        }

        //--Show chart
        private void button2_Click(object sender, EventArgs e)
        {
            chart.loadEvents();
            chart.Show();
        }

        /*private void updateEvent()
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
        }*/

        //--Change start day
        private void weekdayStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];
            calEvent.startDay = (string)weekdayStart.SelectedItem;
            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);
        }

        //-- Check state of the "Always" checkbox and update it after each change on one property of th event
        private void checkAlwaysAvailable(CalendarEvent cEv)
        {
            if (cEv.durationDays == 6 && cEv.durationHours == 23 && cEv.durationMinutes == 59 && cEv.durationSeconds == 59)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
        }

        //--LISTENERS
        private void startHourChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];
            calEvent.startHour = (int)hourStart.Value;
            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);

            saveConfig.Enabled = true;
        }

        private void startMinuteChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];
            calEvent.startMinute = (int)minuteStart.Value;
            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);

            saveConfig.Enabled = true;
        }

        private void startSecondChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];
            calEvent.startSecond = (int)secondStart.Value;
            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);

            saveConfig.Enabled = true;
        }

        private void durationDayChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];

            calEvent.durationDays = (int)dayDuration.Value;

            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);
            checkAlwaysAvailable(calEvent);

            saveConfig.Enabled = true;
        }

        private void durationHourChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];

            calEvent.durationHours = (int)hoursDuration.Value;

            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);
            checkAlwaysAvailable(calEvent);

            saveConfig.Enabled = true;
        }

        private void durationMinuteChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];

            calEvent.durationMinutes = (int)minutesDuration.Value;

            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);
            checkAlwaysAvailable(calEvent);

            saveConfig.Enabled = true;
        }

        private void durationSecondChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)conf.events.events[eventsList.SelectedIndex];

            calEvent.durationSeconds = (int)secondsDuration.Value;

            conf.events.modifyEvent(eventsList.SelectedIndex, calEvent);
            // change name in the event list control
            eventsList.Items[eventsList.SelectedIndex] = makeEventName(calEvent);
            checkAlwaysAvailable(calEvent);
            saveConfig.Enabled = true;
        }
        //-- END LISTENERS

        //--Behaviour of the "Always" Checkbox
        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                //--Check if the event always exist
                bool isExist = false;
                foreach (Event ev in conf.events.events)
                {
                    CalendarEvent cEv = (CalendarEvent)ev;
                    if (cEv.durationDays == 6 && cEv.durationHours == 23 && cEv.durationMinutes == 59 && cEv.durationSeconds == 59)
                    {
                        isExist = true;
                    }
                }

                if (!isExist)
                {
                    //--We create the event if it doesnt exist
                    CalendarEvent cEv = new CalendarEvent();
                    // calEvent.startDay = (string)weekdayStart.SelectedItem;
                    cEv.startDay = "monday";
                    cEv.startHour = 0;
                    cEv.startMinute = 0;
                    cEv.startSecond = 0;

                    cEv.durationDays = 6;
                    cEv.durationHours = 23;
                    cEv.durationMinutes = 59;
                    cEv.durationSeconds = 59;

                    conf.events.addEvent(cEv);
                    eventsList.Items.Add("Always available");
                }

                //--Disabled buttons
                eventEditorGroup.Enabled = false;
                eventsList.Enabled = false;
                newEventButton.Enabled = false;
                deleteEventButton.Enabled = false;
            }
            else
            {
                //--Enabled buttons
                eventEditorGroup.Enabled = true;
                eventsList.Enabled = true;
                newEventButton.Enabled = true;
                deleteEventButton.Enabled = true;

                //--Delete always available event if it exist
                int idx = -1;
                for (int i = 0; i < conf.events.events.Length; i++)
                {
                    CalendarEvent cEv = (CalendarEvent)conf.events.events[i];
                    if (cEv.durationDays == 6 && cEv.durationHours == 23 && cEv.durationMinutes == 59 && cEv.durationSeconds == 59)
                    {
                        idx = i;
                    }
                }

                if (idx == -1)
                    return;
                eventsList.Items.RemoveAt(idx);
                conf.events.removeEvent(idx);

            }
            saveConfig.Enabled = true;
        }



        /**************************************************************
        * ACTIONS                                                    *
        * ***********************************************************/
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

        //--P2P - save host
        private void saveHost_Click(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            P2PAction ourAction = (P2PAction)conf.actions.actions[actionsList.SelectedIndex];
            //P2PAction ourAction = (P2PAction)conf.action;
            hostList.Items[hostList.SelectedIndex] = peerUrl.Text;
            ourAction.modifyContact(hostList.SelectedIndex, peerUrl.Text);
        }

        //--P2P - add host
        private void addHost_Click(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            P2PAction ourAction = (P2PAction)conf.actions.actions[actionsList.SelectedIndex];
            //P2PAction ourAction = (P2PAction)conf.action;
            hostList.Items.Add("newPeer");
            ourAction.addContact("newPeer");
        }

        //--P2P - delete host
        private void deleteHost_Click(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            P2PAction ourAction = (P2PAction)conf.actions.actions[actionsList.SelectedIndex];
            //P2PAction ourAction = (P2PAction)conf.action;
            int index = hostList.SelectedIndex;
            hostList.Items.RemoveAt(index);
            ourAction.deleteContact(index);
        }

        private void actionTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);

            switch ((string)(actionTypeBox.SelectedItem))
            {
                case AdvertAction.DESCRIPTION:
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

                case P2PAction.DESCRIPTION:
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

                case RMAction.DESCRIPTION:
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

        private void actionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (actionsList.SelectedIndex == -1)
            {
                actionEditorGroup.Enabled = false;
                return;
            }
            actionEditorGroup.Enabled = true;
            ConfigParser.Action action = (ConfigParser.Action)conf.actions.actions[actionsList.SelectedIndex];

            if (action.GetType() == typeof(P2PAction))
            {

                p2pactionGroup.Enabled = true;
                rmiActionGroup.Enabled = false;
                rmActionGroup.Enabled = false;

                p2pactionGroup.Visible = true;
                rmiActionGroup.Visible = false;
                rmActionGroup.Visible = false;

                actionTypeBox.SelectedItem = P2PAction.DESCRIPTION;

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
                p2pactionGroup.Enabled = false;
                rmiActionGroup.Enabled = false;
                rmActionGroup.Enabled = true;

                p2pactionGroup.Visible = false;
                rmiActionGroup.Visible = false;
                rmActionGroup.Visible = true;

                actionTypeBox.SelectedItem = RMAction.DESCRIPTION;
                RMAction rmAction = (RMAction)action;
                rmUrl.Text = rmAction.url;
                rmNodeName.Text = rmAction.nodeName;
            }
            else if (action.GetType() == typeof(AdvertAction))
            {
                p2pactionGroup.Enabled = false;
                rmiActionGroup.Enabled = true;
                rmActionGroup.Enabled = false;

                p2pactionGroup.Visible = false;
                rmiActionGroup.Visible = true;
                rmActionGroup.Visible = false;

                actionTypeBox.SelectedItem = AdvertAction.DESCRIPTION;
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


            if (action.priority.Equals(""))
            {
                priorityBox.SelectedIndex = 0;
            }
            else
            {
                priorityBox.SelectedIndex = priorityBox.FindString(action.priority);
            }
            userBox.SelectedIndex = userBox.FindString(action.user);
        }

        private void saveActionButton_Click(object sender, EventArgs e)
        {
            updateAction();
        }

        private void updateAction()
        {
            if (actionsList.SelectedIndex == -1)
                return;

            switch ((string)(actionTypeBox.SelectedItem))
            {
                case AdvertAction.DESCRIPTION:
                    {
                        AdvertAction newAction = new AdvertAction();
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
                            actionsList.Items[actionsList.SelectedIndex] = AdvertAction.DESCRIPTION;
                        }
                    }
                    break;

                case P2PAction.DESCRIPTION:
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
                            actionsList.Items[actionsList.SelectedIndex] = P2PAction.DESCRIPTION;
                        }
                    }
                    break;

                case RMAction.DESCRIPTION:
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
                            actionsList.Items[actionsList.SelectedIndex] = RMAction.DESCRIPTION;
                        }
                    }
                    break;

                default:
                    break;
            }
        }



        //--Checkbox (Define a nodeName for RMI registration)
        private void checkDefineNodeName_CheckedChanged(object sender, EventArgs e)
        {
            actionPropertyChanged(sender, e);
            if (rmiNodeEnabled.Checked)
                rmiNodeName.Enabled = true;
            else
            {
                rmiNodeName.Enabled = false;
                rmiNodeName.Text = "";
            }

        }

        private void newActionButton_Click(object sender, EventArgs e)
        {
            CreateAction guiCreate = new CreateAction(this);
            guiCreate.ShowDialog();

            switch (_typeOfNewAction)
            {
                case AdvertAction.DESCRIPTION:
                    {
                        AdvertAction action = new AdvertAction();
                        action.priority = "Normal";
                        action.nodeName = "";
                        conf.actions.addAction(action);
                        actionsList.Items.Add(AdvertAction.DESCRIPTION);
                    }
                    break;

                case RMAction.DESCRIPTION:
                    {
                        RMAction action = new RMAction();
                        action.priority = "Normal";
                        action.nodeName = "";
                        action.url = "";
                        action.user = "";

                        conf.actions.addAction(action);
                        actionsList.Items.Add(RMAction.DESCRIPTION);
                    }
                    break;

                case P2PAction.DESCRIPTION:
                    {
                        P2PAction action = new P2PAction();
                        action.priority = "Normal";
                        conf.actions.addAction(action);
                        actionsList.Items.Add(P2PAction.DESCRIPTION);
                    }
                    break;
            }
        }

        private void deleteActionButton_Click(object sender, EventArgs e)
        {
            int selectedIdx = actionsList.SelectedIndex;
            if (selectedIdx == -1)
                return;
            actionsList.Items.RemoveAt(selectedIdx);
            conf.actions.removeAction(selectedIdx);

            saveConfig.Enabled = true;
        }

        private void actionPropertyChanged(object sender, EventArgs e)
        {
            saveConfig.Enabled = true;
        }

        //--Save Priority of the action after change
        private void actionPriorityChanged(object sender, EventArgs e)
        {
            if (actionsList.SelectedIndex == -1)
                return;

            switch ((string)(actionTypeBox.SelectedItem))
            {
                case AdvertAction.DESCRIPTION:
                    {
                        AdvertAction newAction = new AdvertAction();
                        newAction = (AdvertAction)(conf.actions.actions[actionsList.SelectedIndex]);
                        newAction.priority = (string)priorityBox.SelectedItem;
                        conf.actions.modifyAction(actionsList.SelectedIndex, newAction);
                    }
                    break;

                case P2PAction.DESCRIPTION:
                    {
                        P2PAction newAction = new P2PAction();
                        newAction = (P2PAction)(conf.actions.actions[actionsList.SelectedIndex]);
                        newAction.priority = (string)priorityBox.SelectedItem;
                        conf.actions.modifyAction(actionsList.SelectedIndex, newAction);
                    }
                    break;

                case RMAction.DESCRIPTION:
                    {
                        RMAction newAction = new RMAction();
                        newAction = (RMAction)(conf.actions.actions[actionsList.SelectedIndex]);
                        newAction.priority = (string)priorityBox.SelectedItem;
                        conf.actions.modifyAction(actionsList.SelectedIndex, newAction);
                    }
                    break;

                default:
                    break;
            }
            saveConfig.Enabled = true;
        }

        private void actionAdvertNodeNameChanged(object sender, EventArgs e)
        {
            if (actionsList.SelectedIndex == -1)
                return;

            AdvertAction newAction = new AdvertAction();
            newAction = (AdvertAction)(conf.actions.actions[actionsList.SelectedIndex]);
            newAction.nodeName = rmiNodeName.Text;
            conf.actions.modifyAction(actionsList.SelectedIndex, newAction);

            saveConfig.Enabled = true;
        }

        private void actionRMUrlChanged(object sender, EventArgs e)
        {
            RMAction newAction = new RMAction();
            newAction = (RMAction)(conf.actions.actions[actionsList.SelectedIndex]);
            newAction.url = rmUrl.Text;
            conf.actions.modifyAction(actionsList.SelectedIndex, newAction);

            saveConfig.Enabled = true;
        }

        private void actionRMNodNameChanged(object sender, EventArgs e)
        {
            RMAction newAction = new RMAction();
            newAction = (RMAction)(conf.actions.actions[actionsList.SelectedIndex]);
            newAction.nodeName = rmNodeName.Text;
            conf.actions.modifyAction(actionsList.SelectedIndex, newAction);

            saveConfig.Enabled = true;
        }

        private void actionP2PProtocolChanged(object sender, EventArgs e)
        {
            P2PAction newAction = new P2PAction();
            newAction = (P2PAction)(conf.actions.actions[actionsList.SelectedIndex]);
            newAction.protocol = p2pProtocol.Text;
            conf.actions.modifyAction(actionsList.SelectedIndex, newAction);

            saveConfig.Enabled = true;
        }

        private void actionP2PUrlChanged(object sender, EventArgs e)
        {
            if (hostList.SelectedIndex != -1)
            {
                P2PAction ourAction = new P2PAction();
                ourAction = (P2PAction)conf.actions.actions[actionsList.SelectedIndex];
                hostList.Items[hostList.SelectedIndex] = peerUrl.Text;
                ourAction.modifyContact(hostList.SelectedIndex, peerUrl.Text);
            }
            saveConfig.Enabled = true;
        }
    }
}