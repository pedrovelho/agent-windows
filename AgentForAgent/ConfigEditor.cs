using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ConfigParser;

namespace AgentForAgent
{
    public partial class ConfigurationEditor : Form
    {
        private const uint MINIMAL_REQUIRED_MEMORY = 96; // 64 for initial heap size + 32 jvm internals
        private Configuration configuration;
        private string configurationLocation;
        private string agentLocation;
        private ConfigurationDialog hook;
        private Chart chart;                

        // Internal data used for jvm parameters list customization
        private const int DELTA = 5;
        private System.Windows.Forms.TextBox editBox;
        int itemSelected = -1;

        private IniFile iniConfiguration;
        //--Constructor
        public ConfigurationEditor(Configuration conf, string confLocation, string agentLocation, ConfigurationDialog hook)
        {
            // First initialize all widgets (gui-generated)
            InitializeComponent();            

            this.configuration = conf;
            this.configurationLocation = confLocation;
            this.agentLocation = agentLocation;
            this.hook = hook;

            // Load the proactive location from the configuration into the gui
            this.proactiveDirectory.Text = conf.agentConfig.proactiveLocation;

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

            // Load the On Runtime Exit script absolute path
            this.scriptLocationTextBox.Text = conf.agentConfig.onRuntimeExitScript;

            ///////////////////////////////////////////////////
            // Load memory management from the configuration //
            ///////////////////////////////////////////////////
            this.memoryManagementGroupBox.Enabled = this.enableMemoryManagementCheckBox.Checked = conf.agentConfig.enableMemoryManagement;
            // Get total physical memory
            System.Decimal val = ProActiveAgent.Utils.getAvailablePhysicalMemory();
            this.availablePhysicalMemoryValue.Text = val.ToString();
            // Set maximums
            this.javaMemoryNumericUpDown.Maximum = val;
            this.nativeMemoryNumericUpDown.Maximum = val;
            // Load memory limit values from the configuration
            this.javaMemoryNumericUpDown.Value = conf.agentConfig.javaMemory;
            this.nativeMemoryNumericUpDown.Value = conf.agentConfig.nativeMemory;
            this.totalProcessMemoryValue.Text = "" + (MINIMAL_REQUIRED_MEMORY + conf.agentConfig.javaMemory + conf.agentConfig.nativeMemory);

            ///////////////////////////////////////
            // Load Multi-Runtime related config //
            ///////////////////////////////////////
            this.availableCPUsValue.Text = "" + Environment.ProcessorCount;
            this.nbRuntimesNumericUpDown.Value = conf.agentConfig.nbProcesses;
            this.useAllAvailableCPUsCheckBox.Checked = conf.agentConfig.useAllCPUs;
            this.nbRuntimesNumericUpDown.Enabled = !conf.agentConfig.useAllCPUs;

            ////////////////////////////////////////
            // Load events from the configuration //
            ////////////////////////////////////////            

            foreach (Event ev in this.configuration.events)
            {
                CalendarEvent cEv = (CalendarEvent)ev;
                this.eventsList.Items.Add(cEv);  
                if (cEv.isAlwaysAvailable())
                {
                    this.alwaysAvailableCheckBox.Checked = true;                    
                }                              
            }

            // Init default values for list boxes
            this.weekdayStart.SelectedIndex = 0;
            this.processPriorityComboBox.SelectedIndex = 0;
            this.maxCpuUsageNumericUpDown.Value = this.maxCpuUsageNumericUpDown.Maximum;

            // Init default values for ProActive Communication Protocol and Port
            this.protocolComboBox.SelectedItem = Enum.GetName(typeof(ProActiveCommunicationProtocol), conf.agentConfig.proActiveCommunicationProtocol);
            this.portInitialValueNumericUpDown.Value = conf.agentConfig.proActiveCommunicationPortInitialValue;

            /////////////////////////////////////////////
            // Load the actions from the configuration //
            /////////////////////////////////////////////            

            // Iterate through all actions in the configuration then 
            // load them into the gui            
            foreach (ConfigParser.Action action in this.configuration.actions)
            {
                if (action.GetType() == typeof(AdvertAction))
                {
                    if (action.isEnabled)
                    {
                        this.rmiRegistrationRadioButton.Select();
                        this.connectionTypeTabControl.SelectedTab = this.rmiRegistrationTabPage;
                    }
                    if (action.javaStarterClass == null || action.javaStarterClass.Equals(""))
                    {
                        this.rmiRegistrationJavaActionClassTextBox.Text = AdvertAction.DEFAULT_JAVA_STARTER_CLASS;
                    }
                    else
                    {
                        this.rmiRegistrationJavaActionClassTextBox.Text = action.javaStarterClass;
                    }
                    AdvertAction advertAction = (AdvertAction)action;
                    this.rmiNodeEnabled.Checked = advertAction.nodeName != null && !advertAction.nodeName.Equals("");
                    this.rmiNodeName.Text = advertAction.nodeName;
                }
                else if (action.GetType() == typeof(RMAction))
                {
                    if (action.isEnabled){
                        this.resourceManagerRegistrationRadioButton.Select();
                        this.connectionTypeTabControl.SelectedTab = this.resourceManagerRegistrationTabPage;
                    }
                    if (action.javaStarterClass == null || action.javaStarterClass.Equals(""))
                    {
                        this.resourceManagerRegistrationJavaActionClassTextBox.Text = RMAction.DEFAULT_JAVA_STARTER_CLASS;
                    }
                    else
                    {
                        this.resourceManagerRegistrationJavaActionClassTextBox.Text = action.javaStarterClass;
                    }
                    RMAction rmAction = (RMAction)action;
                    this.rmUrl.Text = rmAction.url;
                    this.rmNodeName.Text = rmAction.nodeName;

                    if (rmAction.username.Equals(RMAction.ANONYMOUS_USERNAME) && rmAction.username.Equals(RMAction.ANONYMOUS_PASSWORD))
                    {
                        this.rmAnonymousCheckBox.Checked = true;
                    }
                    else {
                        this.rmUsernameTextBox.Text = rmAction.username;
                        this.rmPasswordTextBox.Text = rmAction.password;
                    }
                }
                else if (action.GetType() == typeof(CustomAction))
                {
                    if (action.isEnabled)
                    {
                        this.customRadioButton.Select();
                        this.connectionTypeTabControl.SelectedTab = this.customTabPage;
                    }
                    if (action.javaStarterClass == null || action.javaStarterClass.Equals(""))
                    {
                        this.customJavaActionClassTextBox.Text = CustomAction.DEFAULT_JAVA_STARTER_CLASS;
                    }
                    else
                    {
                        this.customJavaActionClassTextBox.Text = action.javaStarterClass;
                    }
                    CustomAction customAction = (CustomAction)action;
                    if (customAction.args != null)
                    {
                        this.customArgumentsListBox.Items.AddRange(customAction.args);
                    }
                }
                else
                {
                    // Unknown action
                }
            }

            //--Chart
            chart = new Chart();
            iniConfiguration = new IniFile("configuration.ini");

            this.saveConfig.Enabled = false;
        }

        private void CreateEditBox(object sender)
        {
            this.jvmParametersListBox = (ListBox)sender;
            this.itemSelected = this.jvmParametersListBox.SelectedIndex;
            Rectangle r = this.jvmParametersListBox.GetItemRectangle(this.itemSelected);
            string itemText = (string)this.jvmParametersListBox.Items[this.itemSelected];

            editBox.Location = new System.Drawing.Point(r.X /*+ DELTA*/, r.Y/* + DELTA*/);
            editBox.Size = new System.Drawing.Size(r.Width /*- 10*/, r.Height/* - DELTA*/);
            editBox.Show();
            this.jvmParametersListBox.Controls.AddRange(new System.Windows.Forms.Control[] { this.editBox });
            editBox.Text = itemText;
            editBox.Focus();
            editBox.SelectAll();
            editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            editBox.LostFocus += new System.EventHandler(this.FocusOver);

            this.saveConfig.Enabled = true;
        }

        private void FocusOver(object sender, System.EventArgs e)
        {
            this.jvmParametersListBox.Items[this.itemSelected] = editBox.Text;
            editBox.Hide();
        }

        private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //Keys.Enter
            {
                this.jvmParametersListBox.Items[this.itemSelected] = editBox.Text;
                this.editBox.Hide();
                this.jvmParametersListBox.Focus();
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
                // Ask the user to be sure to exit without saving the configuration
                DialogResult res = MessageBox.Show("Are you sure you want to exit without saving ?", "Exit Configuration Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
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
            // Copy all jvm parameters from listbox into the cofiguration
            string[] values = new string[this.jvmParametersListBox.Items.Count];
            this.jvmParametersListBox.Items.CopyTo(values, 0);
            this.configuration.agentConfig.jvmParameters = values;
            this.configuration.agentConfig.onRuntimeExitScript = this.scriptLocationTextBox.Text;
            // Save memory management configuration
            this.configuration.agentConfig.enableMemoryManagement = this.enableMemoryManagementCheckBox.Checked;
            this.configuration.agentConfig.javaMemory = System.Decimal.ToUInt32(this.javaMemoryNumericUpDown.Value);
            this.configuration.agentConfig.nativeMemory = System.Decimal.ToUInt32(this.nativeMemoryNumericUpDown.Value);            
            // Save multi process related config
            this.configuration.agentConfig.nbProcesses = Convert.ToInt32(this.nbRuntimesNumericUpDown.Value);
            this.configuration.agentConfig.useAllCPUs = this.useAllAvailableCPUsCheckBox.Checked;
            //--Events list                        
            this.internalCopyEventsList();
            // Save ProActive Communication Protocol and Port initial value
            this.configuration.agentConfig.proActiveCommunicationProtocol = (ProActiveCommunicationProtocol)Enum.Parse(typeof(ProActiveCommunicationProtocol), (string)this.protocolComboBox.SelectedItem);            
            this.configuration.agentConfig.proActiveCommunicationPortInitialValue = System.Decimal.ToInt32(this.portInitialValueNumericUpDown.Value);
            // Save all defined actions                        
            if (this.configuration.actions == null || this.configuration.actions.Length < 3)
            {
                this.configuration.actions = new ConfigParser.Action[3];
            }
            // Save rmi registration action definition
            AdvertAction advertAction = new AdvertAction();
            advertAction.nodeName = rmiNodeEnabled.Checked ? rmiNodeName.Text : "";
            advertAction.javaStarterClass = this.rmiRegistrationJavaActionClassTextBox.Text;
            advertAction.isEnabled = this.rmiRegistrationRadioButton.Checked;            
            this.configuration.actions[0] = advertAction;
            // Save resource manager registration action definition
            RMAction rmAction = new RMAction();
            rmAction.url = rmUrl.Text;
            rmAction.nodeName = rmNodeName.Text;
            rmAction.username = this.rmUsernameTextBox.Text;
            rmAction.password = this.rmPasswordTextBox.Text;
            rmAction.javaStarterClass = this.resourceManagerRegistrationJavaActionClassTextBox.Text;
            rmAction.isEnabled = this.resourceManagerRegistrationRadioButton.Checked;
            this.configuration.actions[1] = rmAction;
            // Save custom action definition
            CustomAction customAction = new CustomAction();
            string[] arguments = new string[this.customArgumentsListBox.Items.Count];
            customArgumentsListBox.Items.CopyTo(arguments, 0);
            customAction.args = arguments;           
            customAction.javaStarterClass = this.customJavaActionClassTextBox.Text;
            customAction.isEnabled = this.customRadioButton.Checked;
            this.configuration.actions[2] = customAction;                 
            // Save the configuration into a file
            try
            {
                ConfigurationParser.saveXml(internalLocation, this.configuration);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Could not save the configuration file: " + exception.ToString());
            }

            if (this.iniConfiguration.GetValue("params", "saveWarning") != "0")
            {
                DialogResult res = MessageBox.Show("Service must be restarted to apply changes.\nDisplay again this message ?", "Restart service", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.No)
                {
                    this.iniConfiguration.SetValue("params", "saveWarning", "0");
                    this.iniConfiguration.Save();
                }
            }
        }

        private void internalCopyEventsList() {
            this.configuration.events.Clear();
            foreach (object item in this.eventsList.Items)
            {
                if (((CalendarEvent)item).startDay != null)
                {
                    this.configuration.events.Add((CalendarEvent)item);
                }
            }
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
                configuration.agentConfig.javaHome = "";
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
                    ProActiveAgent.Utils.readClasspath(this.configuration.agentConfig);
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
            configuration.agentConfig.proactiveLocation = proactiveDirectory.Text;
            saveConfig.Enabled = true;
        }

        //--Update config if it change
        private void jvmDirectory_TextChanged(object sender, EventArgs e)
        {
            configuration.agentConfig.javaHome = jvmDirectory.Text;
            saveConfig.Enabled = true;
        }

        //--List Available network interfaces to get java style names
        private void listNetworkInterfacesButton_Click(object sender, EventArgs e)
        {            
            try
            {
                if (this.jvmDirectory.Text == null || this.jvmDirectory.Text.Equals(""))
                {
                    return;
                }
                string[] values = ProActiveAgent.JavaNetworkInterfaceLister.listJavaNetworkInterfaces(this.jvmDirectory.Text, this.agentLocation);
                this.networkInterfacesListBox.Items.Clear();
                this.networkInterfacesListBox.Items.AddRange(values);
                this.useNetworkInterfaceButton.Enabled = values.Length > 0;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void useNetworkInterfaceButton_Click(object sender, EventArgs e)
        {
            // Get the selected element in the network interfaces list box then 
            // extract the java interface name add add it as jvm parameter

            int selectedIndex = this.networkInterfacesListBox.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            else { 
                // Extract the interface name from the selected string
                string str = (string)this.networkInterfacesListBox.Items[selectedIndex];
                // The java interface name should be the word before the first white space
                string[] splittedStr = str.Split(new char[]{' '});
                string paramNetworkInterface = "-Dproactive.net.interface=" + splittedStr[0];

                // Search in the JVM parameters the -Dproactive.net.interface string
                int indexOfJvmParameter = -1;
                for (int i=0; i < this.jvmParametersListBox.Items.Count ; i++) {
                    string s = (string)this.jvmParametersListBox.Items[i];
                    if (s.StartsWith("-Dproactive.net.interface="))
                    {                        
                        indexOfJvmParameter = i;
                        break;
                    }
                }
                if(indexOfJvmParameter == -1) {
                    // No parameter was found so add a new one
                    this.jvmParametersListBox.Items.Add(paramNetworkInterface);
                } else {
                    this.jvmParametersListBox.Items[indexOfJvmParameter] = paramNetworkInterface;
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
            CalendarEvent cEv = (CalendarEvent)this.eventsList.SelectedItem;
            if (cEv == null) {return;}
            this.weekdayStart.SelectedIndex = cEv.resolveDay();
            this.hourStart.Value = cEv.startHour;
            this.minuteStart.Value = cEv.startMinute;
            this.secondStart.Value = cEv.startSecond;
            this.dayDuration.Value = cEv.durationDays;
            this.hoursDuration.Value = cEv.durationHours;
            this.minutesDuration.Value = cEv.durationMinutes;
            this.secondsDuration.Value = cEv.durationSeconds;
            this.processPriorityComboBox.SelectedItem = Enum.GetName(typeof(ProcessPriorityClass), cEv.processPriority);
            this.maxCpuUsageNumericUpDown.Value = cEv.maxCpuUsage;
            this.eventEditorGroup.Enabled = true;
        }


        //--Add event
        private void createEventButton_Click(object sender, EventArgs e)
        {
            CalendarEvent calEvent = new CalendarEvent();            
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
            System.Collections.Generic.List<Event> copyList = new System.Collections.Generic.List<Event>(this.eventsList.Items.Count);
            foreach(Event ev in this.eventsList.Items){
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
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.startDay = (string)weekdayStart.SelectedItem;            
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            this.saveConfig.Enabled = true;
        }

        //--LISTENERS
        private void startHourChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.startHour = (int)hourStart.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            saveConfig.Enabled = true;
        }

        private void startMinuteChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.startMinute = (int)minuteStart.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            saveConfig.Enabled = true;
        }

        private void startSecondChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.startSecond = (int)secondStart.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);

            saveConfig.Enabled = true;
        }

        private void durationDayChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.durationDays = (int)dayDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void durationHourChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.durationHours = (int)hoursDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void durationMinuteChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.durationMinutes = (int)minutesDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void durationSecondChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.durationSeconds = (int)secondsDuration.Value;
            // Refresh widget
            this.eventsList.RefreshItem(eventsList.SelectedIndex);
            this.alwaysAvailableCheckBox.Checked = calEvent.isAlwaysAvailable();

            this.saveConfig.Enabled = true;
        }

        private void processPriorityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.processPriority = (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), (string)this.processPriorityComboBox.SelectedItem);
            this.saveConfig.Enabled = true;
        }

        private void maxCpuUsageNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (eventsList.SelectedIndex == -1)
                return;
            CalendarEvent calEvent = (CalendarEvent)this.eventsList.SelectedItem;
            calEvent.maxCpuUsage = Convert.ToUInt32(this.maxCpuUsageNumericUpDown.Value);            
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
                foreach (Event ev in this.eventsList.Items)
                {
                    CalendarEvent cEv = (CalendarEvent)ev;                    
                    if (cEv.isAlwaysAvailable())
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist)
                {
                    //--We create the event if it doesnt exist
                    CalendarEvent cEv = new CalendarEvent();                                        
                    cEv.startHour = 0;
                    cEv.startMinute = 0;
                    cEv.startSecond = 0;

                    cEv.durationDays = 6;
                    cEv.durationHours = 23;
                    cEv.durationMinutes = 59;
                    cEv.durationSeconds = 59;                    
                    
                    this.eventsList.Items.Add(cEv);
                }

                // Disable buttons and group boxes                
                this.eventEditorGroup.Enabled = false;
                eventsList.Enabled = false;
                createEventButton.Enabled = false;
                deleteEventButton.Enabled = false;
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
                    CalendarEvent cEv = (CalendarEvent)this.eventsList.Items[i];
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
            this.connectionTypeTabControl.SelectedTab = this.rmiRegistrationTabPage;
            this.saveConfig.Enabled = true;
        }

        private void rmiNodeName_TextChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        //--Checkbox (Define a nodeName for RMI registration)
        private void rmiNodeEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (rmiNodeEnabled.Checked)
                this.rmiNodeName.Enabled = true;
            else
            {
                // Empty TextBox
                this.rmiNodeName.Text = "";
                // Disable node name TextBox
                this.rmiNodeName.Enabled = false;
            }
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

        private void rmAnonymousCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rmAnonymousCheckBox.Checked)
            {
                // Fill the text boxes with predef values
                this.rmUsernameTextBox.Text = RMAction.ANONYMOUS_USERNAME;
                this.rmPasswordTextBox.Text = RMAction.ANONYMOUS_PASSWORD;
                this.rmUsernameTextBox.Enabled = this.rmPasswordTextBox.Enabled = false;
            }
            else {
                // Fill the text boxes with predef values
                this.rmUsernameTextBox.Text = "";
                this.rmPasswordTextBox.Text = "";
                this.rmUsernameTextBox.Enabled = this.rmPasswordTextBox.Enabled = true;
            }                        
            this.saveConfig.Enabled = true;
        }

        private void rmUsernameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.saveConfig.Enabled = true;
        }

        private void rmPasswordTextBox_TextChanged(object sender, EventArgs e)
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
            else {
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

        private void addJvmParameterButton_Click(object sender, EventArgs e)
        {
            // Add a new entry in the jvm parameter list box
            int i = this.jvmParametersListBox.Items.Add("New parameter");
            this.jvmParametersListBox.SetSelected(i, true);
            saveConfig.Enabled = true;
        }

        private void removeJvmParameterButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.jvmParametersListBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                this.jvmParametersListBox.Items.RemoveAt(selectedIndex--);
                // After deletion automatically select precedent if there is one
                if (selectedIndex > -1)
                {
                    this.jvmParametersListBox.SelectedIndex = selectedIndex;
                }
                else
                {
                    // Try to select the last if there is one
                    if (this.jvmParametersListBox.Items.Count > 0)
                    {
                        this.jvmParametersListBox.SelectedIndex = this.jvmParametersListBox.Items.Count - 1;
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
            this.jvmParametersListBox.Controls.AddRange(new System.Windows.Forms.Control[] { this.editBox });
            this.editBox.Text = "";
            this.editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            this.editBox.LostFocus += new System.EventHandler(this.FocusOver);
            //this.editBox.BackColor = Color.Beige;
            //this.editBox.Font = new Font("Varanda", 15, FontStyle.Regular | FontStyle.Underline, GraphicsUnit.Pixel);
            //this.editBox.ForeColor = Color.Blue;
            this.editBox.BorderStyle = BorderStyle.FixedSingle;

            if (this.configuration.agentConfig.jvmParameters != null)
            {
                this.jvmParametersListBox.Items.AddRange(this.configuration.agentConfig.jvmParameters);
            }
        }

        private void jvmParametersListBox_DoubleClick(object sender, EventArgs e)
        {
            // If no selected items and list box is empty add one item and select it
            // otherwise select current item
            if (this.jvmParametersListBox.SelectedIndex == -1)
            {
                if (this.jvmParametersListBox.Items.Count == 0)
                {
                    this.jvmParametersListBox.SelectedIndex = this.jvmParametersListBox.Items.Add("New parameter");
                }
                else
                {
                    this.jvmParametersListBox.SelectedIndex = this.jvmParametersListBox.Items.Count - 1;
                }
            }
            this.CreateEditBox(sender);
        }

        private void jvmParametersListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 ) // Enter key
            {
                if (this.jvmParametersListBox.SelectedIndex > -1) 
                {
                    this.CreateEditBox(sender);
                }                
            }            
        }

        /********************************************/
        /** MEMORY MANAGEMENT GUI HANDLING METHODS **/
        /********************************************/

        private void enableMemoryManagementCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.memoryManagementGroupBox.Enabled = this.enableMemoryManagementCheckBox.Checked;
            this.saveConfig.Enabled = true;
        }

        private void javaMemoryNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.totalProcessMemoryValue.Text = "" + (MINIMAL_REQUIRED_MEMORY + this.javaMemoryNumericUpDown.Value + this.nativeMemoryNumericUpDown.Value);
            saveConfig.Enabled = true;
        }

        private void nativeMemoryNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.totalProcessMemoryValue.Text = "" + (MINIMAL_REQUIRED_MEMORY + this.javaMemoryNumericUpDown.Value + this.nativeMemoryNumericUpDown.Value);
            saveConfig.Enabled = true;
        }

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
            this.nbRuntimesNumericUpDown.Enabled = !this.useAllAvailableCPUsCheckBox.Checked;
            this.saveConfig.Enabled = true;
        }

        /*************************************************/
        /** ON RUNTIME EXIT SCRIPT GUI HANDLING METHODS **/
        /*************************************************/

        private void scriptLocationButton_Click(object sender, EventArgs e)
        {            
            DialogResult result = this.scriptLocationFileDialog.ShowDialog();            
            if (result == DialogResult.OK) // test result
            {
                string selectedPath = this.scriptLocationFileDialog.FileName;
                this.scriptLocationTextBox.Text = selectedPath;            
            }
        }

        private void scriptLocationTextBox_TextChanged(object sender, EventArgs e)
        {
            configuration.agentConfig.javaHome = jvmDirectory.Text;
            saveConfig.Enabled = true;
        }
    }
}