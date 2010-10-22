/*
 * ################################################################
 *
 * ProActive: The Java(TM) library for Parallel, Distributed,
 *            Concurrent computing with Security and Mobility
 *
 * Copyright (C) 1997-2010 INRIA/University of 
 *                                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
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
 * If needed, contact us to obtain a release under GPL Version 2 
 * or a different license than the GPL.
 *
 *  Initial developer(s):               The ProActive Team
 *                        http://proactive.inria.fr/team_members.htm
 *  Contributor(s): ActiveEon Team - http://www.activeeon.com
 *
 * ################################################################
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ConfigParser;
using ProActiveAgent;
using System.Collections.Generic;

namespace AgentForAgent
{
    /// <summary>
    /// Enumeration type of ProActive Agent running type.
    /// </summary>
    public enum ProActiveCommunicationProtocol
    {
        undefined = 0,
        rmi = 1,
        http = 2,
        pamr = 3,
        pnp = 4
    }

    public partial class ConfigurationEditor : Form
    {
        private readonly AgentType configuration;
        private string configurationLocation;
        private readonly string agentLocation;
        private readonly AgentWindow hook;

        private Chart chart;

        // Internal data used for jvm options list customization
        private const int DELTA = 5;
        private System.Windows.Forms.TextBox editBox;
        int itemSelected = -1;

        private IniFile iniConfiguration;
        //--Constructor
        public ConfigurationEditor(AgentType conf, string confLocation, string agentLocation, AgentWindow hook)
        {
            // First initialize all widgets (gui-generated)
            InitializeComponent();

            this.configuration = conf;
            this.configurationLocation = confLocation;
            this.agentLocation = agentLocation;
            this.hook = hook;

            // Load the proactive location from the configuration into the gui
            this.proactiveDirectory.Text = conf.config.proactiveHome;

            if (conf.config.javaHome.Equals(""))
            {
                checkBox1.Checked = true;
                jvmDirectory.Enabled = false;
                jvmLocationButton.Enabled = false;
            }
            else
            {
                jvmDirectory.Text = conf.config.javaHome;
            }

            // Load the On Runtime Exit script absolute path
            this.scriptLocationTextBox.Text = conf.config.onRuntimeExitScript;

            ///////////////////////////////////////////////////
            // Load memory management from the configuration //
            ///////////////////////////////////////////////////            
            // Get total physical memory
            System.Decimal val = Utils.getAvailablePhysicalMemory();
            this.availablePhysicalMemoryValue.Text = val.ToString();
            // Set maximums
            this.memoryLimitNumericUpDown.Maximum = val;     
            // Load memory limit values from the configuration
            this.memoryLimitNumericUpDown.Value = conf.config.memoryLimit;            

            ///////////////////////////////////////
            // Load Multi-Runtime related config //
            ///////////////////////////////////////
            this.availableCPUsValue.Text = "" + Environment.ProcessorCount;

            if (conf.config.nbRuntimes == 0)
            {
                this.useAllAvailableCPUsCheckBox.Checked = true;
                this.nbRuntimesNumericUpDown.Value = Environment.ProcessorCount;
            }
            else
            {
                this.nbRuntimesNumericUpDown.Value = conf.config.nbRuntimes;
                this.nbRuntimesNumericUpDown.Enabled = true;
            }                        
            
            ////////////////////////////////////////
            // Load events from the configuration //
            ////////////////////////////////////////            

            // Init default values for list boxes
            this.weekdayStart.SelectedIndex = 0;
            this.processPriorityComboBox.SelectedIndex = 0;
            this.maxCpuUsageNumericUpDown.Value = this.maxCpuUsageNumericUpDown.Maximum;

            // Load config events in the GUI
            foreach (CalendarEventType cEv in this.configuration.events)
            {                
                int index = this.eventsList.Items.Add(cEv);

                // Check for always available 
                if (cEv.isAlwaysAvailable())
                {
                    this.alwaysAvailableCheckBox.Checked = true;
                    this.processPriorityComboBox.SelectedItem = Enum.GetName(typeof(ProcessPriorityClass), configuration.config.processPriority);
                    this.maxCpuUsageNumericUpDown.Value = configuration.config.maxCpuUsage;
                    this.eventsList.SelectedIndex = index;

                    // Disable all other controls
                    this.eventEditorGroup.Enabled = false;
                    this.eventsList.Enabled = false;
                    this.createEventButton.Enabled = false;
                    this.deleteEventButton.Enabled = false;
                }
            }

            // Init default values for ProActive Communication Protocol and Port
            this.protocolComboBox.SelectedItem = Enum.GetName(typeof(ProActiveCommunicationProtocol), Enum.Parse(typeof(ProActiveCommunicationProtocol),conf.config.protocol));
            this.portInitialValueNumericUpDown.Value = conf.config.portRange.first;

            /////////////////////////////////////////////
            // Load the actions from the configuration //
            /////////////////////////////////////////////            

            // Iterate through all actions in the configuration then 
            // load them into the gui            
            foreach (ConnectionType con in this.configuration.connections)
            {
                if (con.GetType() == typeof(LocalBindConnectionType))
                {
                    if (con.enabled)
                    {
                        this.localRegistrationRadioButton.Select();
                        this.connectionTypeTabControl.SelectedTab = this.localRegistrationTabPage;
                    }
                    if (con.javaStarterClass == null || con.javaStarterClass.Equals(""))
                    {
                        this.rmiRegistrationJavaActionClassTextBox.Text = LocalBindConnectionType.DEFAULT_JAVA_STARTER_CLASS;
                    }
                    else
                    {
                        this.rmiRegistrationJavaActionClassTextBox.Text = con.javaStarterClass;
                    }
                    LocalBindConnectionType localbind = (LocalBindConnectionType)con;                    
                    this.localRegistrationNodeName.Text = localbind.nodename;
                }
                else if (con.GetType() == typeof(ResoureManagerConnectionType))
                {
                    if (con.enabled)
                    {
                        this.resourceManagerRegistrationRadioButton.Select();
                        this.connectionTypeTabControl.SelectedTab = this.resourceManagerRegistrationTabPage;
                    }
                    if (con.javaStarterClass == null || con.javaStarterClass.Equals(""))
                    {
                        this.resourceManagerRegistrationJavaActionClassTextBox.Text = ResoureManagerConnectionType.DEFAULT_JAVA_STARTER_CLASS;
                    }
                    else
                    {
                        this.resourceManagerRegistrationJavaActionClassTextBox.Text = con.javaStarterClass;
                    }
                    ResoureManagerConnectionType rmAction = (ResoureManagerConnectionType)con;
                    this.rmUrl.Text = rmAction.url;
                    this.nodeNameTextBox.Text = rmAction.nodename;
                    this.nodeSourceNameTextBox.Text = rmAction.nodeSourceName;
                    this.credentialLocationTextBox.Text = rmAction.credential;
                }
                else if (con.GetType() == typeof(CustomConnectionType))
                {
                    if (con.enabled)
                    {
                        this.customRadioButton.Select();
                        this.connectionTypeTabControl.SelectedTab = this.customTabPage;
                    }
                    if (con.javaStarterClass == null || con.javaStarterClass.Equals(""))
                    {
                        this.customJavaActionClassTextBox.Text = CustomConnectionType.DEFAULT_JAVA_STARTER_CLASS;
                    }
                    else
                    {
                        this.customJavaActionClassTextBox.Text = con.javaStarterClass;
                    }
                    CustomConnectionType customConnection = (CustomConnectionType)con;
                    if (customConnection.args != null)
                    {
                        this.customArgumentsListBox.Items.AddRange(customConnection.args);
                    }
                }
                else
                {
                    // Unknown action
                }
            }

            //--Chart
            chart = new Chart();
            iniConfiguration = new IniFile(this.agentLocation + "\\configuration.ini");

            this.saveConfig.Enabled = false;
        }

        private void CreateEditBox(object sender)
        {
            this.jvmOptionsListBox = (ListBox)sender;
            this.itemSelected = this.jvmOptionsListBox.SelectedIndex;
            Rectangle r = this.jvmOptionsListBox.GetItemRectangle(this.itemSelected);
            string itemText = (string)this.jvmOptionsListBox.Items[this.itemSelected];

            editBox.Location = new System.Drawing.Point(r.X /*+ DELTA*/, r.Y/* + DELTA*/);
            editBox.Size = new System.Drawing.Size(r.Width /*- 10*/, r.Height/* - DELTA*/);
            editBox.Show();
            this.jvmOptionsListBox.Controls.AddRange(new System.Windows.Forms.Control[] { this.editBox });
            editBox.Text = itemText;
            editBox.Focus();
            editBox.SelectAll();
            editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            editBox.LostFocus += new System.EventHandler(this.FocusOver);

            this.saveConfig.Enabled = true;
        }

        private void FocusOver(object sender, System.EventArgs e)
        {
            this.jvmOptionsListBox.Items[this.itemSelected] = editBox.Text;
            editBox.Hide();
        }

        private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //Keys.Enter
            {
                this.jvmOptionsListBox.Items[this.itemSelected] = editBox.Text;
                this.editBox.Hide();
                this.jvmOptionsListBox.Focus();
                return;
            }

            if (e.KeyChar == 27) //Keys.Escape
                editBox.Hide();
        }

        /**************************************************************
        * WINDOWS                                                     *
        * ************************************************************/
        //--Click to "Cancel" button
        private void closeConfig_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfigEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the save button is enabled
            if (this.saveConfig.Enabled)
            {
                // Ask the user to save before exit
                DialogResult res = MessageBox.Show("Save before exit ?", "Exit Configuration Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    // Save the configuration
                    this.internalSave(this.configurationLocation);
                }
            }
        }

        //--Save current config
        private void saveConfig_Click(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = false;
            this.internalSave(this.configurationLocation);
        }

        //--Save as new file
        private void saveConfigAs_Click(object sender, EventArgs e)
        {
            saveConfig.Enabled = false;
            //--Show dialog form
            saveFileDialog1.Filter = "Xml File|*.xml";
            saveFileDialog1.Title = "Save an xml configuration file";
            saveFileDialog1.ShowDialog();
            string locationAs = saveFileDialog1.FileName;
            if (locationAs == null || locationAs.Equals(""))
            {
                return;
            }
            this.configurationLocation = locationAs;
            hook.setConfigLocation(locationAs);
            this.internalSave(this.configurationLocation);
        }

        private void internalSave(string internalLocation)
        {
            // Copy all jvm options from listbox into the cofiguration
            string[] values = new string[this.jvmOptionsListBox.Items.Count];
            if (this.jvmOptionsListBox.Items.Count > 0)
            {
                this.jvmOptionsListBox.Items.CopyTo(values, 0);                
            }
            this.configuration.config.jvmParameters = values;

            // Set the on runtime exit script
            this.configuration.config.onRuntimeExitScript = this.scriptLocationTextBox.Text;

            // Save memory management configuration
            ushort memoryLimit = Convert.ToUInt16(memoryLimitNumericUpDown.Value);
            if (memoryLimit > 0) {
                this.configuration.config.memoryLimit = memoryLimit;
            }                        
            // Save multi process related config
            if (this.useAllAvailableCPUsCheckBox.Checked) {
                this.configuration.config.nbRuntimes = 0;
            } else {
                this.configuration.config.nbRuntimes = Convert.ToUInt16(this.nbRuntimesNumericUpDown.Value);
            }            
            //--Events list                        
            this.internalCopyEventsList();
            // Save ProActive Communication Protocol and Port initial value
            this.configuration.config.protocol = (string)this.protocolComboBox.SelectedItem;
            this.configuration.config.portRange.first = Convert.ToUInt16(this.portInitialValueNumericUpDown.Value);
            // Save all defined actions                        
            if (this.configuration.connections == null || this.configuration.connections.Length < 3)
            {
                this.configuration.connections = new ConfigParser.ConnectionType[3];
            }
            // Save rmi registration action definition
            LocalBindConnectionType localbind = new LocalBindConnectionType();
            if (!"".Equals(localRegistrationNodeName.Text)){
                localbind.nodename = localRegistrationNodeName.Text;
            }
            localbind.javaStarterClass = this.rmiRegistrationJavaActionClassTextBox.Text;
            localbind.enabled = this.localRegistrationRadioButton.Checked;            
            this.configuration.connections[0] = localbind;

            // Save resource manager registration action definition
            ResoureManagerConnectionType rmConnection = new ResoureManagerConnectionType();
            if ("".Equals(rmUrl.Text)) {
                rmConnection.url = Constants.DEFAULT_RM_URL;
            } else {
                rmConnection.url = rmUrl.Text;
            }
            if (!"".Equals(nodeNameTextBox.Text))
            {
                rmConnection.nodename = nodeNameTextBox.Text;
            }
            if (!"".Equals(nodeSourceNameTextBox.Text))
            {
                rmConnection.nodeSourceName = nodeSourceNameTextBox.Text;
            }
            if (!"".Equals(credentialLocationTextBox.Text)){ 
                rmConnection.credential = credentialLocationTextBox.Text;
            }
            if (!"".Equals(this.resourceManagerRegistrationJavaActionClassTextBox.Text))
            {
                rmConnection.javaStarterClass = this.resourceManagerRegistrationJavaActionClassTextBox.Text;
            }            
            rmConnection.enabled = this.resourceManagerRegistrationRadioButton.Checked;
            this.configuration.connections[1] = rmConnection;

            // Save custom action definition
            CustomConnectionType customAction = new CustomConnectionType();
            if (this.customArgumentsListBox.Items.Count > 0)
            {
                string[] arguments = new string[this.customArgumentsListBox.Items.Count];
                customArgumentsListBox.Items.CopyTo(arguments, 0);
                customAction.args = arguments;
            }
            customAction.javaStarterClass = this.customJavaActionClassTextBox.Text;
            customAction.enabled = this.customRadioButton.Checked;
            this.configuration.connections[2] = customAction;
            // Save the configuration into a file
            try
            {
                ConfigurationParser.saveXml(internalLocation, this.configuration);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Could not save the configuration file: " + exception.ToString());
            }

            this.hook.askAndRestart();
        }

        private void internalCopyEventsList()
        {            
            List<CalendarEventType> list = new List<CalendarEventType>();
            foreach (object item in this.eventsList.Items)
            {
                CalendarEventType evt = (CalendarEventType)item;
                if (evt.isAlwaysAvailable())
                {
                    evt.config.portRange = null;
                }
                list.Add((CalendarEventType)item);
            }
            this.configuration.events = list.ToArray();
        }

        /**************************************************************
        * GENERAL CONFIGURATION                                     *
        * ************************************************************/
        //--Checkbox (Use system-wide JVM location)
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                jvmDirectory.Enabled = false;
                jvmLocationButton.Enabled = false;
                configuration.config.javaHome = "";
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
            proActiveLocationBrowser.SelectedPath = proactiveDirectory.Text;
            DialogResult result = proActiveLocationBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                proactiveDirectory.Text = proActiveLocationBrowser.SelectedPath;
                // Once the proactive location is specified check if classpath can be read
                try
                {
                    Utils.readClasspath(this.configuration.config);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid ProActive location : " + ex.ToString(), "Invalid ProActive location", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        //--Show dialog box to select jvm path
        private void jvmLocationButton_Click(object sender, EventArgs e)
        {
            jvmLocationBrowser.SelectedPath = jvmDirectory.Text;
            DialogResult result = jvmLocationBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                jvmDirectory.Text = jvmLocationBrowser.SelectedPath;
            }
        }

        //--Update config if it change
        private void proactiveLocation_TextChanged(object sender, EventArgs e)
        {
            configuration.config.proactiveHome = proactiveDirectory.Text;
            saveConfig.Enabled = true;
        }

        //--Update config if it change
        private void jvmDirectory_TextChanged(object sender, EventArgs e)
        {
            configuration.config.javaHome = jvmDirectory.Text;
            saveConfig.Enabled = true;
        }

        //--List Available network interfaces to get java style names
        private void listNetworkInterfacesButton_Click(object sender, EventArgs e)
        {
            if (this.jvmDirectory.Text == null || this.jvmDirectory.Text.Equals(""))
            {
                return;
            }
            if (!System.IO.File.Exists(this.jvmDirectory.Text+Constants.BIN_JAVA))
            {
                MessageBox.Show("The Java Home location is incorrect.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                jvmLocationButton_Click(null,null);
                return;
            }
            try
            {
                string[] values = Utils.listJavaNetworkInterfaces(this.jvmDirectory.Text, this.agentLocation);
                this.networkInterfacesListBox.Items.Clear();
                this.networkInterfacesListBox.Items.AddRange(values);
                this.useNetworkInterfaceButton.Enabled = values.Length > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void useNetworkInterfaceButton_Click(object sender, EventArgs e)
        {
            // Get the selected element in the network interfaces list box then 
            // extract the java interface name add add it as jvm option

            int selectedIndex = this.networkInterfacesListBox.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            else
            {
                // Extract the interface name from the selected string
                string str = (string)this.networkInterfacesListBox.Items[selectedIndex];
                // The java interface name should be the word before the first white space
                string[] splittedStr = str.Split(new char[] { ' ' });
                string paramNetworkInterface = "-Dproactive.net.interface=" + splittedStr[0];

                // Search in the JVM options the -Dproactive.net.interface string
                int indexOfJvmOption = -1;
                for (int i = 0; i < this.jvmOptionsListBox.Items.Count; i++)
                {
                    string s = (string)this.jvmOptionsListBox.Items[i];
                    if (s.StartsWith("-Dproactive.net.interface="))
                    {
                        indexOfJvmOption = i;
                        break;
                    }
                }
                if (indexOfJvmOption == -1)
                {
                    // No option was found so add a new one
                    this.jvmOptionsListBox.Items.Add(paramNetworkInterface);
                }
                else
                {
                    this.jvmOptionsListBox.Items[indexOfJvmOption] = paramNetworkInterface;
                }

                saveConfig.Enabled = true;
            }

        }

        /**************************************************************
        * EVENTS                                                      *
        * ************************************************************/

        //--Fill the fields with the values of the selected event
        private void eventsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalendarEventType cEv = (CalendarEventType)this.eventsList.SelectedItem;
            if (cEv == null) { return; }
            this.weekdayStart.SelectedIndex = cEv.resolveDay();
            this.hourStart.Value = cEv.start.hour;
            this.minuteStart.Value = cEv.start.minute;
            this.secondStart.Value = cEv.start.second;
            this.dayDuration.Value = cEv.duration.days;
            this.hoursDuration.Value = cEv.duration.hours;
            this.minutesDuration.Value = cEv.duration.minutes;
            this.secondsDuration.Value = cEv.duration.seconds;
            this.processPriorityComboBox.SelectedItem = Enum.GetName(typeof(ProcessPriorityClass), cEv.config.processPriority);
            this.maxCpuUsageNumericUpDown.Value = cEv.config.maxCpuUsage;
            this.eventEditorGroup.Enabled = true;
        }


        //--Add event
        private void createEventButton_Click(object sender, EventArgs e)
        {
            CalendarEventType calEvent = new CalendarEventType();
            calEvent.config = new AgentConfigType();
            int indexToSelect = this.eventsList.Items.Add(calEvent);
            // Select the created item
            this.eventsList.SelectedIndex = indexToSelect;
            this.saveConfig.Enabled = true;
        }

        //--Delete event
        private void deleteEventButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.eventsList.SelectedIndex;
            if (selectedIndex != -1)
            {
                this.eventsList.Items.RemoveAt(selectedIndex--);

                // After deletion automatically select precedent if there is one
                if (selectedIndex > -1)
                {
                    this.eventsList.SelectedIndex = selectedIndex;
                }
                else
                {
                    // Try to select the last if there is one
                    if (this.eventsList.Items.Count > 0)
                    {
                        this.eventsList.SelectedIndex = this.eventsList.Items.Count - 1;
                    }
                    else
                    {
                        // Disable other widgets if there is no more items
                        this.eventEditorGroup.Enabled = false;
                    }
                }
                this.saveConfig.Enabled = true;
            }

        }

        //--Show chart
        private void showButton_Click(object sender, EventArgs e)
        {
            // Save the configuration
            List<CalendarEventType> copyList = new List<CalendarEventType>(this.eventsList.Items.Count);
            foreach (CalendarEventType ev in this.eventsList.Items)
            {
                copyList.Add(ev);
            }
            chart.loadEvents(copyList);
            chart.Show();
        }

        //--Change start day
        private void weekdayStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.start.day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)weekdayStart.SelectedItem);
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            this.saveConfig.Enabled = true;
        }

        //--LISTENERS
        private void startHourChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.start.hour = (ushort)hourStart.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            saveConfig.Enabled = true;
        }

        private void startMinuteChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.start.minute = (ushort)minuteStart.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            saveConfig.Enabled = true;
        }

        private void startSecondChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.start.second = (ushort)secondStart.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            saveConfig.Enabled = true;
        }

        private void durationDayChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.duration.days = (ushort)dayDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void durationHourChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.duration.hours = (ushort)hoursDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void durationMinuteChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.duration.minutes = (ushort)minutesDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void durationSecondChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.duration.seconds = (ushort)secondsDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void processPriorityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.config.processPriority = (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), (string)this.processPriorityComboBox.SelectedItem);
            this.saveConfig.Enabled = true;
        }

        private void maxCpuUsageNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEventType calEvent = (CalendarEventType)this.eventsList.SelectedItem;
            calEvent.config.maxCpuUsage = Convert.ToUInt16(this.maxCpuUsageNumericUpDown.Value);
            this.saveConfig.Enabled = true;
        }

        //-- END LISTENERS

        //--Behaviour of the "Always" Checkbox
        private void alwaysAvailableCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (alwaysAvailableCheckBox.Checked)
            {
                //--Check if the event always exist
                bool isExist = false;
                foreach (CalendarEventType ev in this.eventsList.Items)
                {
                    if (ev.isAlwaysAvailable())
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist)
                {
                    // The always available event is a full week
                    // with by default a normal process priority and 100% max CPU usage
                    CalendarEventType cEv = new CalendarEventType();
                    cEv.config.portRange = null; // not needed
                    cEv.start.day = DayOfWeek.Sunday;
                    cEv.start.hour = 0;
                    cEv.start.minute = 0;
                    cEv.start.second = 0;

                    cEv.duration.days = 6;
                    cEv.duration.hours = 23;
                    cEv.duration.minutes = 59;
                    cEv.duration.seconds = 59;

                    this.processPriorityComboBox.SelectedIndex = 0;
                    this.maxCpuUsageNumericUpDown.Value = this.maxCpuUsageNumericUpDown.Maximum; 

                    int index = this.eventsList.Items.Add(cEv);
                    this.eventsList.SelectedIndex = index;
                }

                // Disable buttons and group boxes
                this.eventEditorGroup.Enabled = false;
                this.eventsList.Enabled = false;
                this.createEventButton.Enabled = false;
                this.deleteEventButton.Enabled = false;
            }
            else
            {
                //--Enabled buttons
                if (this.eventsList.SelectedIndex != -1)
                {
                    this.eventEditorGroup.Enabled = true;
                }
                eventsList.Enabled = true;
                createEventButton.Enabled = true;
                deleteEventButton.Enabled = true;

                //--Delete always available event if it exist
                int idx = -1;
                for (int i = 0; i < this.eventsList.Items.Count; i++)
                {
                    CalendarEventType cEv = (CalendarEventType)this.eventsList.Items[i];
                    if (cEv.isAlwaysAvailable())
                    {
                        idx = i;
                        break;
                    }
                }

                if (idx == -1)
                    return;
                eventsList.Items.RemoveAt(idx);

            }
            saveConfig.Enabled = true;
        }

        /******************************************/
        /** CONNECTION TYPE GUI HANDLING METHODS **/
        /******************************************/

        private void protocolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        private void initialValueNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        /****************************************************************/
        /** ADVERT ACTION TYPE - rmi registration gui handling methods **/
        /****************************************************************/

        private void rmiRegistrationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.connectionTypeTabControl.SelectedTab = this.localRegistrationTabPage;
            this.saveConfig.Enabled = true;
        }

        private void rmiNodeName_TextChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        /*************************************************************************/
        /** RM ACTION TYPE - resource manager registration gui handling methods **/
        /*************************************************************************/

        private void resourceManagerRegistrationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.connectionTypeTabControl.SelectedTab = this.resourceManagerRegistrationTabPage;
            this.saveConfig.Enabled = true;
        }

        private void rmUrl_TextChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        private void rmNodeName_TextChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        private void nodeSourceNameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        private void credentialLocationTextBox_TextChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        /***********************************************************************/
        /** CUSTOM ACTION TYPE - custom conenction gui handling methods **/
        /***********************************************************************/

        private void customRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.connectionTypeTabControl.SelectedTab = this.customTabPage;
            this.saveConfig.Enabled = true;
        }

        private void customAddHostButton_Click(object sender, EventArgs e)
        {
            string contactUrl = this.customArgumentTextBox.Text;
            int addedIndex;
            if (contactUrl == null || contactUrl.Equals(""))
            {
                addedIndex = this.customArgumentsListBox.Items.Add("new Arg");
            }
            else
            {
                addedIndex = this.customArgumentsListBox.Items.Add(contactUrl);
            }
            this.customArgumentsListBox.SelectedIndex = addedIndex;
            this.saveConfig.Enabled = true;
        }

        private void customArgumentsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.customArgumentTextBox.Text = (string)this.customArgumentsListBox.SelectedItem;
            this.customSaveArgumentButton.Enabled = true;
        }

        private void customDeleteArgumentButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.customArgumentsListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                this.customArgumentsListBox.Items.RemoveAt(selectedIndex--);
                this.customArgumentTextBox.Text = "";
                this.customSaveArgumentButton.Enabled = false;

                // After deletion automatically select precedent if there is one
                if (selectedIndex > -1)
                {
                    this.customArgumentsListBox.SelectedIndex = selectedIndex;
                }
                else
                {
                    // Try to select the last if there is one
                    if (this.customArgumentsListBox.Items.Count > 0)
                    {
                        this.customArgumentsListBox.SelectedIndex = this.customArgumentsListBox.Items.Count - 1;
                    }
                }
                this.saveConfig.Enabled = true;
            }
        }

        private void customSaveArgumentButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.customArgumentsListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                this.customArgumentsListBox.Items[selectedIndex] = this.customArgumentTextBox.Text;
                this.saveConfig.Enabled = true;
            }
        }

        /****************************************************/
        /** GENERAL configuration tab GUI HANDLING METHODS **/
        /****************************************************/

        private void addJvmOptionButton_Click(object sender, EventArgs e)
        {
            // Add a new entry in the jvm option list box
            int i = this.jvmOptionsListBox.Items.Add("New option");
            this.jvmOptionsListBox.SetSelected(i, true);
            saveConfig.Enabled = true;
        }

        private void removeJvmOptionButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.jvmOptionsListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                this.jvmOptionsListBox.Items.RemoveAt(selectedIndex--);
                // After deletion automatically select precedent if there is one
                if (selectedIndex > -1)
                {
                    this.jvmOptionsListBox.SelectedIndex = selectedIndex;
                }
                else
                {
                    // Try to select the last if there is one
                    if (this.jvmOptionsListBox.Items.Count > 0)
                    {
                        this.jvmOptionsListBox.SelectedIndex = this.jvmOptionsListBox.Items.Count - 1;
                    }
                }
                saveConfig.Enabled = true;
            }
        }

        private void ConfigEditor_Load(object sender, EventArgs e)
        {
            this.editBox = new System.Windows.Forms.TextBox();
            this.editBox.Location = new System.Drawing.Point(0, 0);
            this.editBox.Size = new System.Drawing.Size(0, 0);
            this.editBox.Hide();
            this.jvmOptionsListBox.Controls.AddRange(new System.Windows.Forms.Control[] { this.editBox });
            this.editBox.Text = "";
            this.editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            this.editBox.LostFocus += new System.EventHandler(this.FocusOver);
            //this.editBox.BackColor = Color.Beige;
            //this.editBox.Font = new Font("Varanda", 15, FontStyle.Regular | FontStyle.Underline, GraphicsUnit.Pixel);
            //this.editBox.ForeColor = Color.Blue;
            this.editBox.BorderStyle = BorderStyle.FixedSingle;

            if (this.configuration.config.jvmParameters != null)
            {
                this.jvmOptionsListBox.Items.AddRange(this.configuration.config.jvmParameters);
            }
        }

        private void jvmOptionsListBox_DoubleClick(object sender, EventArgs e)
        {
            // If no selected items and list box is empty add one item and select it
            // otherwise select current item
            if (this.jvmOptionsListBox.SelectedIndex == -1)
            {
                if (this.jvmOptionsListBox.Items.Count == 0)
                {
                    this.jvmOptionsListBox.SelectedIndex = this.jvmOptionsListBox.Items.Add("New option");
                }
                else
                {
                    this.jvmOptionsListBox.SelectedIndex = this.jvmOptionsListBox.Items.Count - 1;
                }
            }
            this.CreateEditBox(sender);
        }

        private void jvmOptionsListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) // Enter key
            {
                if (this.jvmOptionsListBox.SelectedIndex > -1)
                {
                    this.CreateEditBox(sender);
                }
            }
        }

        /********************************************/
        /** MEMORY MANAGEMENT GUI HANDLING METHODS **/
        /********************************************/

        private void actionTypeTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            // Get the item from the collection.
            TabPage _TabPage = this.connectionTypeTabControl.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _TabBounds = this.connectionTypeTabControl.GetTabRect(e.Index);

            Brush _TextBrush = new SolidBrush(Color.Black);

            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.                
                g.FillRectangle(Brushes.Gray, e.Bounds);
            }

            // Draw string. Center the text.
            StringFormat _StringFlags = new StringFormat();
            _StringFlags.Alignment = StringAlignment.Center;
            _StringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_TabPage.Text, _TabPage.Font, _TextBrush,
                         _TabBounds, _StringFlags);
        }

        /*******************************************************/
        /** MULTI-PROCESS RELATED CONFIG GUI HANDLING METHODS **/
        /*******************************************************/

        private void nbRuntimesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        private void useAllAvailableCPUsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.nbRuntimesNumericUpDown.Value = 0;
            this.nbRuntimesNumericUpDown.Enabled = !this.useAllAvailableCPUsCheckBox.Checked;
            this.saveConfig.Enabled = true;
        }

        /*************************************************/
        /** ON RUNTIME EXIT SCRIPT GUI HANDLING METHODS **/
        /*************************************************/

        private void scriptLocationButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.scriptLocationOpenDialog.ShowDialog();
            if (result == DialogResult.OK) // test result
            {
                string selectedPath = this.scriptLocationOpenDialog.FileName;
                this.scriptLocationTextBox.Text = selectedPath;
            }
        }

        private void scriptLocationTextBox_TextChanged(object sender, EventArgs e)
        {
            configuration.config.javaHome = jvmDirectory.Text;
            saveConfig.Enabled = true;
        }

        private void credentialBrowseLocationButton_Click(object sender, EventArgs e)
        {
            DialogResult result = credentialLocationOpenDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                credentialLocationTextBox.Text = credentialLocationOpenDialog.FileName;
                saveConfig.Enabled = true;
            }
        }

        private void memoryLimitNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }
    }
}