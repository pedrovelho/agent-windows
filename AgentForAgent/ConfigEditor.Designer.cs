using System;
using System.Management;

namespace AgentForAgent
{
    public class RefreshingListBox : System.Windows.Forms.ListBox
    {
        public new void RefreshItem(int index)
        {
            base.RefreshItem(index);
        }
    }

    partial class ConfigurationEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationEditor));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.jvmParametersListBox = new System.Windows.Forms.ListBox();
            this.removeJvmParameterButton = new System.Windows.Forms.Button();
            this.addJvmParameterButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.jvmLocationButton = new System.Windows.Forms.Button();
            this.proactiveLocationButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.jvmDirectory = new System.Windows.Forms.TextBox();
            this.proactiveDirectory = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.onRuntimeExitGroupBox = new System.Windows.Forms.GroupBox();
            this.scriptLocationButton = new System.Windows.Forms.Button();
            this.scriptLocationLabel = new System.Windows.Forms.Label();
            this.scriptLocationTextBox = new System.Windows.Forms.TextBox();
            this.multiRuntimeGroupBox = new System.Windows.Forms.GroupBox();
            this.useAllAvailableCPUsCheckBox = new System.Windows.Forms.CheckBox();
            this.nbRuntimesLabel = new System.Windows.Forms.Label();
            this.availableCPUsValue = new System.Windows.Forms.Label();
            this.availableCPUsLabel = new System.Windows.Forms.Label();
            this.nbRuntimesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.networkInterfaceListGroupBox = new System.Windows.Forms.GroupBox();
            this.useNetworkInterfaceButton = new System.Windows.Forms.Button();
            this.networkInterfacesListBox = new System.Windows.Forms.ListBox();
            this.refreshNetworkInterfacesButton = new System.Windows.Forms.Button();
            this.enableMemoryManagementCheckBox = new System.Windows.Forms.CheckBox();
            this.memoryManagementGroupBox = new System.Windows.Forms.GroupBox();
            this.totalProcessMemoryValue = new System.Windows.Forms.Label();
            this.totalProcessMemoryLabel = new System.Windows.Forms.Label();
            this.javaMemoryNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.javaMemoryLabel = new System.Windows.Forms.Label();
            this.availablePhysicalMemoryValue = new System.Windows.Forms.Label();
            this.availablePhysicalMemoryLabel = new System.Windows.Forms.Label();
            this.nativeMemoryNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.nativeMemoryLabel = new System.Windows.Forms.Label();
            this.connectionTabPage = new System.Windows.Forms.TabPage();
            this.proActiveCommunicationProtocolGroupBox = new System.Windows.Forms.GroupBox();
            this.protocolComboBox = new System.Windows.Forms.ComboBox();
            this.protocolLabel = new System.Windows.Forms.Label();
            this.portInitialValue = new System.Windows.Forms.Label();
            this.portInitialValueNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.enabledConnectionGroupBox = new System.Windows.Forms.GroupBox();
            this.resourceManagerRegistrationRadioButton = new System.Windows.Forms.RadioButton();
            this.rmiRegistrationRadioButton = new System.Windows.Forms.RadioButton();
            this.customRadioButton = new System.Windows.Forms.RadioButton();
            this.connectionTypeTabControl = new System.Windows.Forms.TabControl();
            this.rmiRegistrationTabPage = new System.Windows.Forms.TabPage();
            this.rmiRegistrationAdditionalConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.rmiRegistrationJavaActionClassTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.rmiActionGroup = new System.Windows.Forms.GroupBox();
            this.rmiNodeName = new System.Windows.Forms.TextBox();
            this.rmiNodeEnabled = new System.Windows.Forms.CheckBox();
            this.resourceManagerRegistrationTabPage = new System.Windows.Forms.TabPage();
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.resourceManagerRegistrationJavaActionClassTextBox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.rmActionGroup = new System.Windows.Forms.GroupBox();
            this.rmAnonymousCheckBox = new System.Windows.Forms.CheckBox();
            this.rmPasswordTextBox = new System.Windows.Forms.TextBox();
            this.rmPasswordLabel = new System.Windows.Forms.Label();
            this.rmUsernameTextBox = new System.Windows.Forms.TextBox();
            this.rmUsernameLabel = new System.Windows.Forms.Label();
            this.rmNodeName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.rmUrl = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.customTabPage = new System.Windows.Forms.TabPage();
            this.customAdditionalConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.customJavaActionClassTextBox = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.customActionGroup = new System.Windows.Forms.GroupBox();
            this.customSaveArgumentButton = new System.Windows.Forms.Button();
            this.customArgumentTextBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.customDeleteButton = new System.Windows.Forms.Button();
            this.customAddButton = new System.Windows.Forms.Button();
            this.customArgumentsListBox = new System.Windows.Forms.ListBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.alwaysAvailableCheckBox = new System.Windows.Forms.CheckBox();
            this.eventEditorGroup = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.maxCpuUsageNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.maxCpuUsageLabel = new System.Windows.Forms.Label();
            this.processPriorityLabel = new System.Windows.Forms.Label();
            this.processPriorityComboBox = new System.Windows.Forms.ComboBox();
            this.startTimeGroupBox = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.secondStart = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.weekdayStart = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.hourStart = new System.Windows.Forms.NumericUpDown();
            this.minuteStart = new System.Windows.Forms.NumericUpDown();
            this.durationGroupBox = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.secondsDuration = new System.Windows.Forms.NumericUpDown();
            this.dayDuration = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.minutesDuration = new System.Windows.Forms.NumericUpDown();
            this.hoursDuration = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.showButton = new System.Windows.Forms.Button();
            this.createEventButton = new System.Windows.Forms.Button();
            this.deleteEventButton = new System.Windows.Forms.Button();
            this.saveConfig = new System.Windows.Forms.Button();
            this.closeConfig = new System.Windows.Forms.Button();
            this.proActiveLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.jvmLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.saveConfigAs = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.scriptLocationFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.eventsList = new AgentForAgent.RefreshingListBox();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.onRuntimeExitGroupBox.SuspendLayout();
            this.multiRuntimeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nbRuntimesNumericUpDown)).BeginInit();
            this.networkInterfaceListGroupBox.SuspendLayout();
            this.memoryManagementGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.javaMemoryNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nativeMemoryNumericUpDown)).BeginInit();
            this.connectionTabPage.SuspendLayout();
            this.proActiveCommunicationProtocolGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portInitialValueNumericUpDown)).BeginInit();
            this.enabledConnectionGroupBox.SuspendLayout();
            this.connectionTypeTabControl.SuspendLayout();
            this.rmiRegistrationTabPage.SuspendLayout();
            this.rmiRegistrationAdditionalConfigurationGroupBox.SuspendLayout();
            this.rmiActionGroup.SuspendLayout();
            this.resourceManagerRegistrationTabPage.SuspendLayout();
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.SuspendLayout();
            this.rmActionGroup.SuspendLayout();
            this.customTabPage.SuspendLayout();
            this.customAdditionalConfigurationGroupBox.SuspendLayout();
            this.customActionGroup.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.eventEditorGroup.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxCpuUsageNumericUpDown)).BeginInit();
            this.startTimeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minuteStart)).BeginInit();
            this.durationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondsDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutesDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hoursDuration)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.jvmParametersListBox);
            this.groupBox1.Controls.Add(this.removeJvmParameterButton);
            this.groupBox1.Controls.Add(this.addJvmParameterButton);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.jvmLocationButton);
            this.groupBox1.Controls.Add(this.proactiveLocationButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.jvmDirectory);
            this.groupBox1.Controls.Add(this.proactiveDirectory);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(665, 175);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ProActive Configuration";
            // 
            // jvmParametersListBox
            // 
            this.jvmParametersListBox.FormattingEnabled = true;
            this.jvmParametersListBox.Location = new System.Drawing.Point(112, 95);
            this.jvmParametersListBox.Name = "jvmParametersListBox";
            this.jvmParametersListBox.Size = new System.Drawing.Size(466, 69);
            this.jvmParametersListBox.TabIndex = 12;
            this.toolTip.SetToolTip(this.jvmParametersListBox, "If the parameter contains ${rank} it will be dynamically replaced by the Runtime " +
                    "rank.");
            this.jvmParametersListBox.DoubleClick += new System.EventHandler(this.jvmParametersListBox_DoubleClick);
            this.jvmParametersListBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.jvmParametersListBox_KeyPress);
            // 
            // removeJvmParameterButton
            // 
            this.removeJvmParameterButton.Location = new System.Drawing.Point(584, 124);
            this.removeJvmParameterButton.Name = "removeJvmParameterButton";
            this.removeJvmParameterButton.Size = new System.Drawing.Size(75, 23);
            this.removeJvmParameterButton.TabIndex = 11;
            this.removeJvmParameterButton.Text = "Remove";
            this.removeJvmParameterButton.UseVisualStyleBackColor = true;
            this.removeJvmParameterButton.Click += new System.EventHandler(this.removeJvmParameterButton_Click);
            // 
            // addJvmParameterButton
            // 
            this.addJvmParameterButton.Location = new System.Drawing.Point(584, 95);
            this.addJvmParameterButton.Name = "addJvmParameterButton";
            this.addJvmParameterButton.Size = new System.Drawing.Size(75, 23);
            this.addJvmParameterButton.TabIndex = 10;
            this.addJvmParameterButton.Text = "Add";
            this.addJvmParameterButton.UseVisualStyleBackColor = true;
            this.addJvmParameterButton.Click += new System.EventHandler(this.addJvmParameterButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(112, 72);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(169, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Use system-wide JVM location";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // jvmLocationButton
            // 
            this.jvmLocationButton.Location = new System.Drawing.Point(584, 42);
            this.jvmLocationButton.Name = "jvmLocationButton";
            this.jvmLocationButton.Size = new System.Drawing.Size(75, 23);
            this.jvmLocationButton.TabIndex = 8;
            this.jvmLocationButton.Text = "Browse...";
            this.jvmLocationButton.UseVisualStyleBackColor = true;
            this.jvmLocationButton.Click += new System.EventHandler(this.jvmLocationButton_Click);
            // 
            // proactiveLocationButton
            // 
            this.proactiveLocationButton.Location = new System.Drawing.Point(584, 14);
            this.proactiveLocationButton.Name = "proactiveLocationButton";
            this.proactiveLocationButton.Size = new System.Drawing.Size(75, 23);
            this.proactiveLocationButton.TabIndex = 7;
            this.proactiveLocationButton.Text = "Browse...";
            this.proactiveLocationButton.UseVisualStyleBackColor = true;
            this.proactiveLocationButton.Click += new System.EventHandler(this.proactiveLocationButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "JVM Parameters:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "JVM Directory:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "ProActive Directory:";
            // 
            // jvmDirectory
            // 
            this.jvmDirectory.Location = new System.Drawing.Point(112, 44);
            this.jvmDirectory.Name = "jvmDirectory";
            this.jvmDirectory.Size = new System.Drawing.Size(466, 20);
            this.jvmDirectory.TabIndex = 1;
            this.jvmDirectory.TextChanged += new System.EventHandler(this.jvmDirectory_TextChanged);
            // 
            // proactiveDirectory
            // 
            this.proactiveDirectory.Location = new System.Drawing.Point(112, 16);
            this.proactiveDirectory.Name = "proactiveDirectory";
            this.proactiveDirectory.Size = new System.Drawing.Size(466, 20);
            this.proactiveDirectory.TabIndex = 0;
            this.proactiveDirectory.TextChanged += new System.EventHandler(this.proactiveLocation_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalTabPage);
            this.tabControl1.Controls.Add(this.connectionTabPage);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(10, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(682, 400);
            this.tabControl1.TabIndex = 1;
            // 
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.onRuntimeExitGroupBox);
            this.generalTabPage.Controls.Add(this.multiRuntimeGroupBox);
            this.generalTabPage.Controls.Add(this.networkInterfaceListGroupBox);
            this.generalTabPage.Controls.Add(this.enableMemoryManagementCheckBox);
            this.generalTabPage.Controls.Add(this.memoryManagementGroupBox);
            this.generalTabPage.Controls.Add(this.groupBox1);
            this.generalTabPage.Location = new System.Drawing.Point(4, 22);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(674, 374);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "General";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // onRuntimeExitGroupBox
            // 
            this.onRuntimeExitGroupBox.Controls.Add(this.scriptLocationButton);
            this.onRuntimeExitGroupBox.Controls.Add(this.scriptLocationLabel);
            this.onRuntimeExitGroupBox.Controls.Add(this.scriptLocationTextBox);
            this.onRuntimeExitGroupBox.Location = new System.Drawing.Point(4, 185);
            this.onRuntimeExitGroupBox.Name = "onRuntimeExitGroupBox";
            this.onRuntimeExitGroupBox.Size = new System.Drawing.Size(384, 43);
            this.onRuntimeExitGroupBox.TabIndex = 5;
            this.onRuntimeExitGroupBox.TabStop = false;
            this.onRuntimeExitGroupBox.Text = "On Runtime Exit";
            // 
            // scriptLocationButton
            // 
            this.scriptLocationButton.Location = new System.Drawing.Point(303, 12);
            this.scriptLocationButton.Name = "scriptLocationButton";
            this.scriptLocationButton.Size = new System.Drawing.Size(75, 25);
            this.scriptLocationButton.TabIndex = 2;
            this.scriptLocationButton.Text = "Browse...";
            this.scriptLocationButton.UseVisualStyleBackColor = true;
            this.scriptLocationButton.Click += new System.EventHandler(this.scriptLocationButton_Click);
            // 
            // scriptLocationLabel
            // 
            this.scriptLocationLabel.AutoSize = true;
            this.scriptLocationLabel.Location = new System.Drawing.Point(6, 17);
            this.scriptLocationLabel.Name = "scriptLocationLabel";
            this.scriptLocationLabel.Size = new System.Drawing.Size(81, 13);
            this.scriptLocationLabel.TabIndex = 1;
            this.scriptLocationLabel.Text = "Script Location:";
            // 
            // scriptLocationTextBox
            // 
            this.scriptLocationTextBox.Location = new System.Drawing.Point(111, 14);
            this.scriptLocationTextBox.Name = "scriptLocationTextBox";
            this.scriptLocationTextBox.Size = new System.Drawing.Size(186, 20);
            this.scriptLocationTextBox.TabIndex = 0;
            this.scriptLocationTextBox.TextChanged += new System.EventHandler(this.scriptLocationTextBox_TextChanged);
            // 
            // multiRuntimeGroupBox
            // 
            this.multiRuntimeGroupBox.Controls.Add(this.useAllAvailableCPUsCheckBox);
            this.multiRuntimeGroupBox.Controls.Add(this.nbRuntimesLabel);
            this.multiRuntimeGroupBox.Controls.Add(this.availableCPUsValue);
            this.multiRuntimeGroupBox.Controls.Add(this.availableCPUsLabel);
            this.multiRuntimeGroupBox.Controls.Add(this.nbRuntimesNumericUpDown);
            this.multiRuntimeGroupBox.Location = new System.Drawing.Point(235, 257);
            this.multiRuntimeGroupBox.Name = "multiRuntimeGroupBox";
            this.multiRuntimeGroupBox.Size = new System.Drawing.Size(153, 111);
            this.multiRuntimeGroupBox.TabIndex = 4;
            this.multiRuntimeGroupBox.TabStop = false;
            this.multiRuntimeGroupBox.Text = "Multi-Runtime";
            // 
            // useAllAvailableCPUsCheckBox
            // 
            this.useAllAvailableCPUsCheckBox.AutoSize = true;
            this.useAllAvailableCPUsCheckBox.Location = new System.Drawing.Point(9, 89);
            this.useAllAvailableCPUsCheckBox.Name = "useAllAvailableCPUsCheckBox";
            this.useAllAvailableCPUsCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.useAllAvailableCPUsCheckBox.Size = new System.Drawing.Size(133, 17);
            this.useAllAvailableCPUsCheckBox.TabIndex = 4;
            this.useAllAvailableCPUsCheckBox.Text = "Use all available CPUs";
            this.useAllAvailableCPUsCheckBox.UseVisualStyleBackColor = true;
            this.useAllAvailableCPUsCheckBox.CheckedChanged += new System.EventHandler(this.useAllAvailableCPUsCheckBox_CheckedChanged);
            // 
            // nbRuntimesLabel
            // 
            this.nbRuntimesLabel.AutoSize = true;
            this.nbRuntimesLabel.Location = new System.Drawing.Point(15, 45);
            this.nbRuntimesLabel.Name = "nbRuntimesLabel";
            this.nbRuntimesLabel.Size = new System.Drawing.Size(71, 13);
            this.nbRuntimesLabel.TabIndex = 3;
            this.nbRuntimesLabel.Text = "Nb Runtimes:";
            // 
            // availableCPUsValue
            // 
            this.availableCPUsValue.AutoSize = true;
            this.availableCPUsValue.Location = new System.Drawing.Point(97, 21);
            this.availableCPUsValue.Name = "availableCPUsValue";
            this.availableCPUsValue.Size = new System.Drawing.Size(13, 13);
            this.availableCPUsValue.TabIndex = 2;
            this.availableCPUsValue.Text = "0";
            // 
            // availableCPUsLabel
            // 
            this.availableCPUsLabel.AutoSize = true;
            this.availableCPUsLabel.Location = new System.Drawing.Point(8, 21);
            this.availableCPUsLabel.Name = "availableCPUsLabel";
            this.availableCPUsLabel.Size = new System.Drawing.Size(83, 13);
            this.availableCPUsLabel.TabIndex = 1;
            this.availableCPUsLabel.Text = "Available CPUs:";
            // 
            // nbRuntimesNumericUpDown
            // 
            this.nbRuntimesNumericUpDown.Location = new System.Drawing.Point(97, 43);
            this.nbRuntimesNumericUpDown.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nbRuntimesNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nbRuntimesNumericUpDown.Name = "nbRuntimesNumericUpDown";
            this.nbRuntimesNumericUpDown.Size = new System.Drawing.Size(48, 20);
            this.nbRuntimesNumericUpDown.TabIndex = 0;
            this.nbRuntimesNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nbRuntimesNumericUpDown.ValueChanged += new System.EventHandler(this.nbRuntimesNumericUpDown_ValueChanged);
            // 
            // networkInterfaceListGroupBox
            // 
            this.networkInterfaceListGroupBox.Controls.Add(this.useNetworkInterfaceButton);
            this.networkInterfaceListGroupBox.Controls.Add(this.networkInterfacesListBox);
            this.networkInterfaceListGroupBox.Controls.Add(this.refreshNetworkInterfacesButton);
            this.networkInterfaceListGroupBox.Location = new System.Drawing.Point(394, 185);
            this.networkInterfaceListGroupBox.Name = "networkInterfaceListGroupBox";
            this.networkInterfaceListGroupBox.Size = new System.Drawing.Size(274, 183);
            this.networkInterfaceListGroupBox.TabIndex = 3;
            this.networkInterfaceListGroupBox.TabStop = false;
            this.networkInterfaceListGroupBox.Text = "Available Network Interfaces (Java 6 only)";
            // 
            // useNetworkInterfaceButton
            // 
            this.useNetworkInterfaceButton.Enabled = false;
            this.useNetworkInterfaceButton.Location = new System.Drawing.Point(112, 154);
            this.useNetworkInterfaceButton.Name = "useNetworkInterfaceButton";
            this.useNetworkInterfaceButton.Size = new System.Drawing.Size(75, 23);
            this.useNetworkInterfaceButton.TabIndex = 3;
            this.useNetworkInterfaceButton.Text = "Use";
            this.useNetworkInterfaceButton.UseVisualStyleBackColor = true;
            this.useNetworkInterfaceButton.Click += new System.EventHandler(this.useNetworkInterfaceButton_Click);
            // 
            // networkInterfacesListBox
            // 
            this.networkInterfacesListBox.FormattingEnabled = true;
            this.networkInterfacesListBox.HorizontalScrollbar = true;
            this.networkInterfacesListBox.Location = new System.Drawing.Point(6, 19);
            this.networkInterfacesListBox.Name = "networkInterfacesListBox";
            this.networkInterfacesListBox.Size = new System.Drawing.Size(262, 121);
            this.networkInterfacesListBox.TabIndex = 2;
            // 
            // refreshNetworkInterfacesButton
            // 
            this.refreshNetworkInterfacesButton.Location = new System.Drawing.Point(193, 154);
            this.refreshNetworkInterfacesButton.Name = "refreshNetworkInterfacesButton";
            this.refreshNetworkInterfacesButton.Size = new System.Drawing.Size(75, 23);
            this.refreshNetworkInterfacesButton.TabIndex = 1;
            this.refreshNetworkInterfacesButton.Text = "Refresh";
            this.refreshNetworkInterfacesButton.UseVisualStyleBackColor = true;
            this.refreshNetworkInterfacesButton.Click += new System.EventHandler(this.listNetworkInterfacesButton_Click);
            // 
            // enableMemoryManagementCheckBox
            // 
            this.enableMemoryManagementCheckBox.AutoSize = true;
            this.enableMemoryManagementCheckBox.Location = new System.Drawing.Point(3, 234);
            this.enableMemoryManagementCheckBox.Name = "enableMemoryManagementCheckBox";
            this.enableMemoryManagementCheckBox.Size = new System.Drawing.Size(164, 17);
            this.enableMemoryManagementCheckBox.TabIndex = 2;
            this.enableMemoryManagementCheckBox.Text = "Enable Memory Management";
            this.enableMemoryManagementCheckBox.UseVisualStyleBackColor = true;
            this.enableMemoryManagementCheckBox.CheckedChanged += new System.EventHandler(this.enableMemoryManagementCheckBox_CheckedChanged);
            // 
            // memoryManagementGroupBox
            // 
            this.memoryManagementGroupBox.Controls.Add(this.totalProcessMemoryValue);
            this.memoryManagementGroupBox.Controls.Add(this.totalProcessMemoryLabel);
            this.memoryManagementGroupBox.Controls.Add(this.javaMemoryNumericUpDown);
            this.memoryManagementGroupBox.Controls.Add(this.javaMemoryLabel);
            this.memoryManagementGroupBox.Controls.Add(this.availablePhysicalMemoryValue);
            this.memoryManagementGroupBox.Controls.Add(this.availablePhysicalMemoryLabel);
            this.memoryManagementGroupBox.Controls.Add(this.nativeMemoryNumericUpDown);
            this.memoryManagementGroupBox.Controls.Add(this.nativeMemoryLabel);
            this.memoryManagementGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryManagementGroupBox.Location = new System.Drawing.Point(3, 257);
            this.memoryManagementGroupBox.Name = "memoryManagementGroupBox";
            this.memoryManagementGroupBox.Size = new System.Drawing.Size(226, 111);
            this.memoryManagementGroupBox.TabIndex = 1;
            this.memoryManagementGroupBox.TabStop = false;
            this.memoryManagementGroupBox.Text = "Memory Management (Mbytes)";
            // 
            // totalProcessMemoryValue
            // 
            this.totalProcessMemoryValue.AutoSize = true;
            this.totalProcessMemoryValue.Location = new System.Drawing.Point(151, 93);
            this.totalProcessMemoryValue.Name = "totalProcessMemoryValue";
            this.totalProcessMemoryValue.Size = new System.Drawing.Size(13, 13);
            this.totalProcessMemoryValue.TabIndex = 8;
            this.totalProcessMemoryValue.Text = "0";
            // 
            // totalProcessMemoryLabel
            // 
            this.totalProcessMemoryLabel.AutoSize = true;
            this.totalProcessMemoryLabel.Location = new System.Drawing.Point(6, 93);
            this.totalProcessMemoryLabel.Name = "totalProcessMemoryLabel";
            this.totalProcessMemoryLabel.Size = new System.Drawing.Size(139, 13);
            this.totalProcessMemoryLabel.TabIndex = 7;
            this.totalProcessMemoryLabel.Text = "Total Process Memory Limit:";
            // 
            // javaMemoryNumericUpDown
            // 
            this.javaMemoryNumericUpDown.Location = new System.Drawing.Point(152, 43);
            this.javaMemoryNumericUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.javaMemoryNumericUpDown.Name = "javaMemoryNumericUpDown";
            this.javaMemoryNumericUpDown.Size = new System.Drawing.Size(67, 20);
            this.javaMemoryNumericUpDown.TabIndex = 5;
            this.javaMemoryNumericUpDown.ValueChanged += new System.EventHandler(this.javaMemoryNumericUpDown_ValueChanged);
            // 
            // javaMemoryLabel
            // 
            this.javaMemoryLabel.AutoSize = true;
            this.javaMemoryLabel.Location = new System.Drawing.Point(23, 45);
            this.javaMemoryLabel.Name = "javaMemoryLabel";
            this.javaMemoryLabel.Size = new System.Drawing.Size(122, 13);
            this.javaMemoryLabel.TabIndex = 4;
            this.javaMemoryLabel.Text = "Additional Java Memory:";
            // 
            // availablePhysicalMemoryValue
            // 
            this.availablePhysicalMemoryValue.AutoSize = true;
            this.availablePhysicalMemoryValue.Location = new System.Drawing.Point(151, 21);
            this.availablePhysicalMemoryValue.Name = "availablePhysicalMemoryValue";
            this.availablePhysicalMemoryValue.Size = new System.Drawing.Size(13, 13);
            this.availablePhysicalMemoryValue.TabIndex = 3;
            this.availablePhysicalMemoryValue.Text = "0";
            // 
            // availablePhysicalMemoryLabel
            // 
            this.availablePhysicalMemoryLabel.AutoSize = true;
            this.availablePhysicalMemoryLabel.Location = new System.Drawing.Point(10, 21);
            this.availablePhysicalMemoryLabel.Name = "availablePhysicalMemoryLabel";
            this.availablePhysicalMemoryLabel.Size = new System.Drawing.Size(135, 13);
            this.availablePhysicalMemoryLabel.TabIndex = 2;
            this.availablePhysicalMemoryLabel.Text = "Available Physical Memory:";
            // 
            // nativeMemoryNumericUpDown
            // 
            this.nativeMemoryNumericUpDown.Location = new System.Drawing.Point(152, 67);
            this.nativeMemoryNumericUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nativeMemoryNumericUpDown.Name = "nativeMemoryNumericUpDown";
            this.nativeMemoryNumericUpDown.Size = new System.Drawing.Size(67, 20);
            this.nativeMemoryNumericUpDown.TabIndex = 1;
            this.nativeMemoryNumericUpDown.ValueChanged += new System.EventHandler(this.nativeMemoryNumericUpDown_ValueChanged);
            // 
            // nativeMemoryLabel
            // 
            this.nativeMemoryLabel.AutoSize = true;
            this.nativeMemoryLabel.Location = new System.Drawing.Point(15, 69);
            this.nativeMemoryLabel.Name = "nativeMemoryLabel";
            this.nativeMemoryLabel.Size = new System.Drawing.Size(130, 13);
            this.nativeMemoryLabel.TabIndex = 0;
            this.nativeMemoryLabel.Text = "Additional Native Memory:";
            // 
            // connectionTabPage
            // 
            this.connectionTabPage.Controls.Add(this.proActiveCommunicationProtocolGroupBox);
            this.connectionTabPage.Controls.Add(this.enabledConnectionGroupBox);
            this.connectionTabPage.Controls.Add(this.connectionTypeTabControl);
            this.connectionTabPage.Location = new System.Drawing.Point(4, 22);
            this.connectionTabPage.Name = "connectionTabPage";
            this.connectionTabPage.Size = new System.Drawing.Size(674, 374);
            this.connectionTabPage.TabIndex = 3;
            this.connectionTabPage.Text = "Connection";
            this.connectionTabPage.UseVisualStyleBackColor = true;
            // 
            // proActiveCommunicationProtocolGroupBox
            // 
            this.proActiveCommunicationProtocolGroupBox.Controls.Add(this.protocolComboBox);
            this.proActiveCommunicationProtocolGroupBox.Controls.Add(this.protocolLabel);
            this.proActiveCommunicationProtocolGroupBox.Controls.Add(this.portInitialValue);
            this.proActiveCommunicationProtocolGroupBox.Controls.Add(this.portInitialValueNumericUpDown);
            this.proActiveCommunicationProtocolGroupBox.Location = new System.Drawing.Point(366, 6);
            this.proActiveCommunicationProtocolGroupBox.Name = "proActiveCommunicationProtocolGroupBox";
            this.proActiveCommunicationProtocolGroupBox.Size = new System.Drawing.Size(305, 45);
            this.proActiveCommunicationProtocolGroupBox.TabIndex = 5;
            this.proActiveCommunicationProtocolGroupBox.TabStop = false;
            this.proActiveCommunicationProtocolGroupBox.Text = "ProActive Communication Protocol";
            // 
            // protocolComboBox
            // 
            this.protocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.protocolComboBox.FormattingEnabled = true;
            this.protocolComboBox.Items.AddRange(new object[] {
            "undefined",
            "rmi",
            "http"});
            this.protocolComboBox.Location = new System.Drawing.Point(61, 18);
            this.protocolComboBox.Name = "protocolComboBox";
            this.protocolComboBox.Size = new System.Drawing.Size(74, 21);
            this.protocolComboBox.TabIndex = 3;
            this.protocolComboBox.SelectedIndexChanged += new System.EventHandler(this.protocolComboBox_SelectedIndexChanged);
            // 
            // protocolLabel
            // 
            this.protocolLabel.AutoSize = true;
            this.protocolLabel.Location = new System.Drawing.Point(6, 21);
            this.protocolLabel.Name = "protocolLabel";
            this.protocolLabel.Size = new System.Drawing.Size(49, 13);
            this.protocolLabel.TabIndex = 2;
            this.protocolLabel.Text = "Protocol:";
            // 
            // portInitialValue
            // 
            this.portInitialValue.AutoSize = true;
            this.portInitialValue.Location = new System.Drawing.Point(150, 21);
            this.portInitialValue.Name = "portInitialValue";
            this.portInitialValue.Size = new System.Drawing.Size(86, 13);
            this.portInitialValue.TabIndex = 1;
            this.portInitialValue.Text = "Port Initial Value:";
            // 
            // portInitialValueNumericUpDown
            // 
            this.portInitialValueNumericUpDown.Location = new System.Drawing.Point(242, 19);
            this.portInitialValueNumericUpDown.Maximum = new decimal(new int[] {
            65534,
            0,
            0,
            0});
            this.portInitialValueNumericUpDown.Minimum = new decimal(new int[] {
            1099,
            0,
            0,
            0});
            this.portInitialValueNumericUpDown.Name = "portInitialValueNumericUpDown";
            this.portInitialValueNumericUpDown.Size = new System.Drawing.Size(57, 20);
            this.portInitialValueNumericUpDown.TabIndex = 0;
            this.toolTip.SetToolTip(this.portInitialValueNumericUpDown, "Defines the value of the \"-Dproactive.rmi.port\" property. This value will always " +
                    "be increased by 1 or more if there is more than one Runtime.");
            this.portInitialValueNumericUpDown.Value = new decimal(new int[] {
            1099,
            0,
            0,
            0});
            this.portInitialValueNumericUpDown.ValueChanged += new System.EventHandler(this.initialValueNumericUpDown_ValueChanged);
            // 
            // enabledConnectionGroupBox
            // 
            this.enabledConnectionGroupBox.Controls.Add(this.resourceManagerRegistrationRadioButton);
            this.enabledConnectionGroupBox.Controls.Add(this.rmiRegistrationRadioButton);
            this.enabledConnectionGroupBox.Controls.Add(this.customRadioButton);
            this.enabledConnectionGroupBox.Location = new System.Drawing.Point(3, 6);
            this.enabledConnectionGroupBox.Name = "enabledConnectionGroupBox";
            this.enabledConnectionGroupBox.Size = new System.Drawing.Size(359, 45);
            this.enabledConnectionGroupBox.TabIndex = 4;
            this.enabledConnectionGroupBox.TabStop = false;
            this.enabledConnectionGroupBox.Text = "Enabled Connection";
            // 
            // resourceManagerRegistrationRadioButton
            // 
            this.resourceManagerRegistrationRadioButton.AutoSize = true;
            this.resourceManagerRegistrationRadioButton.Location = new System.Drawing.Point(116, 19);
            this.resourceManagerRegistrationRadioButton.Name = "resourceManagerRegistrationRadioButton";
            this.resourceManagerRegistrationRadioButton.Size = new System.Drawing.Size(175, 17);
            this.resourceManagerRegistrationRadioButton.TabIndex = 2;
            this.resourceManagerRegistrationRadioButton.TabStop = true;
            this.resourceManagerRegistrationRadioButton.Text = "Resource Manager Registration";
            this.resourceManagerRegistrationRadioButton.UseVisualStyleBackColor = true;
            this.resourceManagerRegistrationRadioButton.CheckedChanged += new System.EventHandler(this.resourceManagerRegistrationRadioButton_CheckedChanged);
            // 
            // rmiRegistrationRadioButton
            // 
            this.rmiRegistrationRadioButton.AutoSize = true;
            this.rmiRegistrationRadioButton.Location = new System.Drawing.Point(6, 19);
            this.rmiRegistrationRadioButton.Name = "rmiRegistrationRadioButton";
            this.rmiRegistrationRadioButton.Size = new System.Drawing.Size(104, 17);
            this.rmiRegistrationRadioButton.TabIndex = 1;
            this.rmiRegistrationRadioButton.TabStop = true;
            this.rmiRegistrationRadioButton.Text = "RMI Registration";
            this.rmiRegistrationRadioButton.UseVisualStyleBackColor = true;
            this.rmiRegistrationRadioButton.CheckedChanged += new System.EventHandler(this.rmiRegistrationRadioButton_CheckedChanged);
            // 
            // customRadioButton
            // 
            this.customRadioButton.AutoSize = true;
            this.customRadioButton.Location = new System.Drawing.Point(297, 19);
            this.customRadioButton.Name = "customRadioButton";
            this.customRadioButton.Size = new System.Drawing.Size(60, 17);
            this.customRadioButton.TabIndex = 3;
            this.customRadioButton.TabStop = true;
            this.customRadioButton.Text = "Custom";
            this.customRadioButton.UseVisualStyleBackColor = true;
            this.customRadioButton.CheckedChanged += new System.EventHandler(this.customRadioButton_CheckedChanged);
            // 
            // connectionTypeTabControl
            // 
            this.connectionTypeTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.connectionTypeTabControl.Controls.Add(this.rmiRegistrationTabPage);
            this.connectionTypeTabControl.Controls.Add(this.resourceManagerRegistrationTabPage);
            this.connectionTypeTabControl.Controls.Add(this.customTabPage);
            this.connectionTypeTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.connectionTypeTabControl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.connectionTypeTabControl.ItemSize = new System.Drawing.Size(19, 175);
            this.connectionTypeTabControl.Location = new System.Drawing.Point(3, 57);
            this.connectionTypeTabControl.Multiline = true;
            this.connectionTypeTabControl.Name = "connectionTypeTabControl";
            this.connectionTypeTabControl.SelectedIndex = 0;
            this.connectionTypeTabControl.Size = new System.Drawing.Size(668, 314);
            this.connectionTypeTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.connectionTypeTabControl.TabIndex = 0;
            this.connectionTypeTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.actionTypeTabControl_DrawItem);
            // 
            // rmiRegistrationTabPage
            // 
            this.rmiRegistrationTabPage.Controls.Add(this.rmiRegistrationAdditionalConfigurationGroupBox);
            this.rmiRegistrationTabPage.Controls.Add(this.rmiActionGroup);
            this.rmiRegistrationTabPage.Location = new System.Drawing.Point(179, 4);
            this.rmiRegistrationTabPage.Name = "rmiRegistrationTabPage";
            this.rmiRegistrationTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.rmiRegistrationTabPage.Size = new System.Drawing.Size(485, 306);
            this.rmiRegistrationTabPage.TabIndex = 0;
            this.rmiRegistrationTabPage.Text = "RMI Registration";
            this.rmiRegistrationTabPage.UseVisualStyleBackColor = true;
            // 
            // rmiRegistrationAdditionalConfigurationGroupBox
            // 
            this.rmiRegistrationAdditionalConfigurationGroupBox.Controls.Add(this.rmiRegistrationJavaActionClassTextBox);
            this.rmiRegistrationAdditionalConfigurationGroupBox.Controls.Add(this.label13);
            this.rmiRegistrationAdditionalConfigurationGroupBox.Location = new System.Drawing.Point(6, 252);
            this.rmiRegistrationAdditionalConfigurationGroupBox.Name = "rmiRegistrationAdditionalConfigurationGroupBox";
            this.rmiRegistrationAdditionalConfigurationGroupBox.Size = new System.Drawing.Size(473, 48);
            this.rmiRegistrationAdditionalConfigurationGroupBox.TabIndex = 2;
            this.rmiRegistrationAdditionalConfigurationGroupBox.TabStop = false;
            this.rmiRegistrationAdditionalConfigurationGroupBox.Text = "Additional Configuration";
            // 
            // rmiRegistrationJavaActionClassTextBox
            // 
            this.rmiRegistrationJavaActionClassTextBox.Location = new System.Drawing.Point(106, 19);
            this.rmiRegistrationJavaActionClassTextBox.Name = "rmiRegistrationJavaActionClassTextBox";
            this.rmiRegistrationJavaActionClassTextBox.Size = new System.Drawing.Size(361, 20);
            this.rmiRegistrationJavaActionClassTextBox.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(95, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Java Starter Class:";
            // 
            // rmiActionGroup
            // 
            this.rmiActionGroup.Controls.Add(this.rmiNodeName);
            this.rmiActionGroup.Controls.Add(this.rmiNodeEnabled);
            this.rmiActionGroup.Location = new System.Drawing.Point(6, 6);
            this.rmiActionGroup.Name = "rmiActionGroup";
            this.rmiActionGroup.Size = new System.Drawing.Size(436, 53);
            this.rmiActionGroup.TabIndex = 1;
            this.rmiActionGroup.TabStop = false;
            this.rmiActionGroup.Text = "RMI Registration";
            // 
            // rmiNodeName
            // 
            this.rmiNodeName.Enabled = false;
            this.rmiNodeName.Location = new System.Drawing.Point(96, 19);
            this.rmiNodeName.Name = "rmiNodeName";
            this.rmiNodeName.Size = new System.Drawing.Size(333, 20);
            this.rmiNodeName.TabIndex = 1;
            this.rmiNodeName.TextChanged += new System.EventHandler(this.rmiNodeName_TextChanged);
            // 
            // rmiNodeEnabled
            // 
            this.rmiNodeEnabled.AutoSize = true;
            this.rmiNodeEnabled.Location = new System.Drawing.Point(9, 21);
            this.rmiNodeEnabled.Name = "rmiNodeEnabled";
            this.rmiNodeEnabled.Size = new System.Drawing.Size(84, 17);
            this.rmiNodeEnabled.TabIndex = 0;
            this.rmiNodeEnabled.Text = "Node name:";
            this.rmiNodeEnabled.UseVisualStyleBackColor = true;
            this.rmiNodeEnabled.CheckedChanged += new System.EventHandler(this.rmiNodeEnabled_CheckedChanged);
            // 
            // resourceManagerRegistrationTabPage
            // 
            this.resourceManagerRegistrationTabPage.Controls.Add(this.resourceManagerRegistrationAdditionalConfigurationGroupBox);
            this.resourceManagerRegistrationTabPage.Controls.Add(this.rmActionGroup);
            this.resourceManagerRegistrationTabPage.Location = new System.Drawing.Point(179, 4);
            this.resourceManagerRegistrationTabPage.Name = "resourceManagerRegistrationTabPage";
            this.resourceManagerRegistrationTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.resourceManagerRegistrationTabPage.Size = new System.Drawing.Size(485, 306);
            this.resourceManagerRegistrationTabPage.TabIndex = 1;
            this.resourceManagerRegistrationTabPage.Text = "Resource Manager Registration";
            this.resourceManagerRegistrationTabPage.UseVisualStyleBackColor = true;
            // 
            // resourceManagerRegistrationAdditionalConfigurationGroupBox
            // 
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.Controls.Add(this.resourceManagerRegistrationJavaActionClassTextBox);
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.Controls.Add(this.label18);
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.Location = new System.Drawing.Point(6, 252);
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.Name = "resourceManagerRegistrationAdditionalConfigurationGroupBox";
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.Size = new System.Drawing.Size(473, 48);
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.TabIndex = 4;
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.TabStop = false;
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.Text = "Additional Configuration";
            // 
            // resourceManagerRegistrationJavaActionClassTextBox
            // 
            this.resourceManagerRegistrationJavaActionClassTextBox.Location = new System.Drawing.Point(106, 19);
            this.resourceManagerRegistrationJavaActionClassTextBox.Name = "resourceManagerRegistrationJavaActionClassTextBox";
            this.resourceManagerRegistrationJavaActionClassTextBox.Size = new System.Drawing.Size(361, 20);
            this.resourceManagerRegistrationJavaActionClassTextBox.TabIndex = 1;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 22);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(95, 13);
            this.label18.TabIndex = 0;
            this.label18.Text = "Java Starter Class:";
            // 
            // rmActionGroup
            // 
            this.rmActionGroup.Controls.Add(this.rmAnonymousCheckBox);
            this.rmActionGroup.Controls.Add(this.rmPasswordTextBox);
            this.rmActionGroup.Controls.Add(this.rmPasswordLabel);
            this.rmActionGroup.Controls.Add(this.rmUsernameTextBox);
            this.rmActionGroup.Controls.Add(this.rmUsernameLabel);
            this.rmActionGroup.Controls.Add(this.rmNodeName);
            this.rmActionGroup.Controls.Add(this.label16);
            this.rmActionGroup.Controls.Add(this.rmUrl);
            this.rmActionGroup.Controls.Add(this.label4);
            this.rmActionGroup.Location = new System.Drawing.Point(6, 6);
            this.rmActionGroup.Name = "rmActionGroup";
            this.rmActionGroup.Size = new System.Drawing.Size(436, 154);
            this.rmActionGroup.TabIndex = 3;
            this.rmActionGroup.TabStop = false;
            this.rmActionGroup.Text = "Resource Manager Registration";
            // 
            // rmAnonymousCheckBox
            // 
            this.rmAnonymousCheckBox.AutoSize = true;
            this.rmAnonymousCheckBox.Location = new System.Drawing.Point(138, 104);
            this.rmAnonymousCheckBox.Name = "rmAnonymousCheckBox";
            this.rmAnonymousCheckBox.Size = new System.Drawing.Size(81, 17);
            this.rmAnonymousCheckBox.TabIndex = 8;
            this.rmAnonymousCheckBox.Text = "Anonymous";
            this.rmAnonymousCheckBox.UseVisualStyleBackColor = true;
            this.rmAnonymousCheckBox.CheckedChanged += new System.EventHandler(this.rmAnonymousCheckBox_CheckedChanged);
            // 
            // rmPasswordTextBox
            // 
            this.rmPasswordTextBox.Location = new System.Drawing.Point(311, 128);
            this.rmPasswordTextBox.Name = "rmPasswordTextBox";
            this.rmPasswordTextBox.PasswordChar = '*';
            this.rmPasswordTextBox.Size = new System.Drawing.Size(119, 20);
            this.rmPasswordTextBox.TabIndex = 7;
            this.rmPasswordTextBox.TextChanged += new System.EventHandler(this.rmPasswordTextBox_TextChanged);
            // 
            // rmPasswordLabel
            // 
            this.rmPasswordLabel.AutoSize = true;
            this.rmPasswordLabel.Location = new System.Drawing.Point(249, 131);
            this.rmPasswordLabel.Name = "rmPasswordLabel";
            this.rmPasswordLabel.Size = new System.Drawing.Size(56, 13);
            this.rmPasswordLabel.TabIndex = 6;
            this.rmPasswordLabel.Text = "Password:";
            // 
            // rmUsernameTextBox
            // 
            this.rmUsernameTextBox.Location = new System.Drawing.Point(311, 102);
            this.rmUsernameTextBox.Name = "rmUsernameTextBox";
            this.rmUsernameTextBox.Size = new System.Drawing.Size(119, 20);
            this.rmUsernameTextBox.TabIndex = 5;
            this.rmUsernameTextBox.TextChanged += new System.EventHandler(this.rmUsernameTextBox_TextChanged);
            // 
            // rmUsernameLabel
            // 
            this.rmUsernameLabel.AutoSize = true;
            this.rmUsernameLabel.Location = new System.Drawing.Point(247, 105);
            this.rmUsernameLabel.Name = "rmUsernameLabel";
            this.rmUsernameLabel.Size = new System.Drawing.Size(58, 13);
            this.rmUsernameLabel.TabIndex = 4;
            this.rmUsernameLabel.Text = "Username:";
            // 
            // rmNodeName
            // 
            this.rmNodeName.Location = new System.Drawing.Point(138, 45);
            this.rmNodeName.Name = "rmNodeName";
            this.rmNodeName.Size = new System.Drawing.Size(292, 20);
            this.rmNodeName.TabIndex = 3;
            this.rmNodeName.TextChanged += new System.EventHandler(this.rmNodeName_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(65, 48);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Node Name:";
            // 
            // rmUrl
            // 
            this.rmUrl.Location = new System.Drawing.Point(138, 19);
            this.rmUrl.Name = "rmUrl";
            this.rmUrl.Size = new System.Drawing.Size(292, 20);
            this.rmUrl.TabIndex = 1;
            this.rmUrl.TextChanged += new System.EventHandler(this.rmUrl_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Resource Manager URL:";
            // 
            // customTabPage
            // 
            this.customTabPage.Controls.Add(this.customAdditionalConfigurationGroupBox);
            this.customTabPage.Controls.Add(this.customActionGroup);
            this.customTabPage.Location = new System.Drawing.Point(179, 4);
            this.customTabPage.Name = "customTabPage";
            this.customTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.customTabPage.Size = new System.Drawing.Size(485, 306);
            this.customTabPage.TabIndex = 2;
            this.customTabPage.Text = "Custom";
            this.customTabPage.UseVisualStyleBackColor = true;
            // 
            // customAdditionalConfigurationGroupBox
            // 
            this.customAdditionalConfigurationGroupBox.Controls.Add(this.customJavaActionClassTextBox);
            this.customAdditionalConfigurationGroupBox.Controls.Add(this.label19);
            this.customAdditionalConfigurationGroupBox.Location = new System.Drawing.Point(6, 252);
            this.customAdditionalConfigurationGroupBox.Name = "customAdditionalConfigurationGroupBox";
            this.customAdditionalConfigurationGroupBox.Size = new System.Drawing.Size(473, 48);
            this.customAdditionalConfigurationGroupBox.TabIndex = 5;
            this.customAdditionalConfigurationGroupBox.TabStop = false;
            this.customAdditionalConfigurationGroupBox.Text = "Additional Configuration";
            // 
            // customJavaActionClassTextBox
            // 
            this.customJavaActionClassTextBox.Location = new System.Drawing.Point(106, 19);
            this.customJavaActionClassTextBox.Name = "customJavaActionClassTextBox";
            this.customJavaActionClassTextBox.Size = new System.Drawing.Size(361, 20);
            this.customJavaActionClassTextBox.TabIndex = 1;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 22);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(95, 13);
            this.label19.TabIndex = 0;
            this.label19.Text = "Java Starter Class:";
            // 
            // customActionGroup
            // 
            this.customActionGroup.Controls.Add(this.customSaveArgumentButton);
            this.customActionGroup.Controls.Add(this.customArgumentTextBox);
            this.customActionGroup.Controls.Add(this.label15);
            this.customActionGroup.Controls.Add(this.customDeleteButton);
            this.customActionGroup.Controls.Add(this.customAddButton);
            this.customActionGroup.Controls.Add(this.customArgumentsListBox);
            this.customActionGroup.Controls.Add(this.label14);
            this.customActionGroup.Location = new System.Drawing.Point(6, 6);
            this.customActionGroup.Name = "customActionGroup";
            this.customActionGroup.Size = new System.Drawing.Size(436, 181);
            this.customActionGroup.TabIndex = 3;
            this.customActionGroup.TabStop = false;
            this.customActionGroup.Text = "Custom";
            // 
            // customSaveArgumentButton
            // 
            this.customSaveArgumentButton.Enabled = false;
            this.customSaveArgumentButton.Location = new System.Drawing.Point(350, 93);
            this.customSaveArgumentButton.Name = "customSaveArgumentButton";
            this.customSaveArgumentButton.Size = new System.Drawing.Size(75, 23);
            this.customSaveArgumentButton.TabIndex = 6;
            this.customSaveArgumentButton.Text = "Save Arg";
            this.customSaveArgumentButton.UseVisualStyleBackColor = true;
            this.customSaveArgumentButton.Click += new System.EventHandler(this.customSaveArgumentButton_Click);
            // 
            // customArgumentTextBox
            // 
            this.customArgumentTextBox.Location = new System.Drawing.Point(69, 122);
            this.customArgumentTextBox.Name = "customArgumentTextBox";
            this.customArgumentTextBox.Size = new System.Drawing.Size(356, 20);
            this.customArgumentTextBox.TabIndex = 5;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 125);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "Argument:";
            // 
            // customDeleteButton
            // 
            this.customDeleteButton.Location = new System.Drawing.Point(350, 63);
            this.customDeleteButton.Name = "customDeleteButton";
            this.customDeleteButton.Size = new System.Drawing.Size(75, 23);
            this.customDeleteButton.TabIndex = 3;
            this.customDeleteButton.Text = "Delete";
            this.customDeleteButton.UseVisualStyleBackColor = true;
            this.customDeleteButton.Click += new System.EventHandler(this.customDeleteArgumentButton_Click);
            // 
            // customAddButton
            // 
            this.customAddButton.Location = new System.Drawing.Point(350, 34);
            this.customAddButton.Name = "customAddButton";
            this.customAddButton.Size = new System.Drawing.Size(75, 23);
            this.customAddButton.TabIndex = 2;
            this.customAddButton.Text = "Add";
            this.customAddButton.UseVisualStyleBackColor = true;
            this.customAddButton.Click += new System.EventHandler(this.customAddHostButton_Click);
            // 
            // customArgumentsListBox
            // 
            this.customArgumentsListBox.FormattingEnabled = true;
            this.customArgumentsListBox.Location = new System.Drawing.Point(9, 34);
            this.customArgumentsListBox.Name = "customArgumentsListBox";
            this.customArgumentsListBox.Size = new System.Drawing.Size(335, 82);
            this.customArgumentsListBox.TabIndex = 1;
            this.customArgumentsListBox.SelectedIndexChanged += new System.EventHandler(this.customArgumentsListBox_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(60, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Arguments:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.alwaysAvailableCheckBox);
            this.tabPage2.Controls.Add(this.eventEditorGroup);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(674, 374);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Planning";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // alwaysAvailableCheckBox
            // 
            this.alwaysAvailableCheckBox.AutoSize = true;
            this.alwaysAvailableCheckBox.Location = new System.Drawing.Point(488, 351);
            this.alwaysAvailableCheckBox.Name = "alwaysAvailableCheckBox";
            this.alwaysAvailableCheckBox.Size = new System.Drawing.Size(180, 17);
            this.alwaysAvailableCheckBox.TabIndex = 18;
            this.alwaysAvailableCheckBox.Text = "Always available (Normal Priority)";
            this.alwaysAvailableCheckBox.UseVisualStyleBackColor = true;
            this.alwaysAvailableCheckBox.CheckStateChanged += new System.EventHandler(this.alwaysAvailableCheckBox_CheckStateChanged);
            // 
            // eventEditorGroup
            // 
            this.eventEditorGroup.Controls.Add(this.groupBox4);
            this.eventEditorGroup.Controls.Add(this.startTimeGroupBox);
            this.eventEditorGroup.Controls.Add(this.durationGroupBox);
            this.eventEditorGroup.Enabled = false;
            this.eventEditorGroup.Location = new System.Drawing.Point(259, 6);
            this.eventEditorGroup.Name = "eventEditorGroup";
            this.eventEditorGroup.Size = new System.Drawing.Size(409, 192);
            this.eventEditorGroup.TabIndex = 1;
            this.eventEditorGroup.TabStop = false;
            this.eventEditorGroup.Text = "Plan Editor";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.maxCpuUsageNumericUpDown);
            this.groupBox4.Controls.Add(this.maxCpuUsageLabel);
            this.groupBox4.Controls.Add(this.processPriorityLabel);
            this.groupBox4.Controls.Add(this.processPriorityComboBox);
            this.groupBox4.Location = new System.Drawing.Point(6, 133);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(397, 51);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Process Management";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(376, 22);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(15, 13);
            this.label20.TabIndex = 4;
            this.label20.Text = "%";
            // 
            // maxCpuUsageNumericUpDown
            // 
            this.maxCpuUsageNumericUpDown.Location = new System.Drawing.Point(321, 20);
            this.maxCpuUsageNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxCpuUsageNumericUpDown.Name = "maxCpuUsageNumericUpDown";
            this.maxCpuUsageNumericUpDown.Size = new System.Drawing.Size(49, 20);
            this.maxCpuUsageNumericUpDown.TabIndex = 3;
            this.maxCpuUsageNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxCpuUsageNumericUpDown.ValueChanged += new System.EventHandler(this.maxCpuUsageNumericUpDown_ValueChanged);
            // 
            // maxCpuUsageLabel
            // 
            this.maxCpuUsageLabel.AutoSize = true;
            this.maxCpuUsageLabel.Location = new System.Drawing.Point(223, 22);
            this.maxCpuUsageLabel.Name = "maxCpuUsageLabel";
            this.maxCpuUsageLabel.Size = new System.Drawing.Size(92, 13);
            this.maxCpuUsageLabel.TabIndex = 2;
            this.maxCpuUsageLabel.Text = "Max CPU Usage: ";
            // 
            // processPriorityLabel
            // 
            this.processPriorityLabel.AutoSize = true;
            this.processPriorityLabel.Location = new System.Drawing.Point(6, 22);
            this.processPriorityLabel.Name = "processPriorityLabel";
            this.processPriorityLabel.Size = new System.Drawing.Size(82, 13);
            this.processPriorityLabel.TabIndex = 1;
            this.processPriorityLabel.Text = "Process Priority:";
            // 
            // processPriorityComboBox
            // 
            this.processPriorityComboBox.FormattingEnabled = true;
            this.processPriorityComboBox.Items.AddRange(new object[] {
            "Normal",
            "Idle",
            "High",
            "RealTime",
            "BelowNormal",
            "AboveNormal"});
            this.processPriorityComboBox.Location = new System.Drawing.Point(94, 19);
            this.processPriorityComboBox.Name = "processPriorityComboBox";
            this.processPriorityComboBox.Size = new System.Drawing.Size(102, 21);
            this.processPriorityComboBox.TabIndex = 0;
            this.processPriorityComboBox.SelectedIndexChanged += new System.EventHandler(this.processPriorityComboBox_SelectedIndexChanged);
            // 
            // startTimeGroupBox
            // 
            this.startTimeGroupBox.Controls.Add(this.label8);
            this.startTimeGroupBox.Controls.Add(this.label7);
            this.startTimeGroupBox.Controls.Add(this.secondStart);
            this.startTimeGroupBox.Controls.Add(this.label5);
            this.startTimeGroupBox.Controls.Add(this.weekdayStart);
            this.startTimeGroupBox.Controls.Add(this.label6);
            this.startTimeGroupBox.Controls.Add(this.hourStart);
            this.startTimeGroupBox.Controls.Add(this.minuteStart);
            this.startTimeGroupBox.Location = new System.Drawing.Point(6, 19);
            this.startTimeGroupBox.Name = "startTimeGroupBox";
            this.startTimeGroupBox.Size = new System.Drawing.Size(397, 51);
            this.startTimeGroupBox.TabIndex = 19;
            this.startTimeGroupBox.TabStop = false;
            this.startTimeGroupBox.Text = "Start Time";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Day:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(297, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Seconds:";
            // 
            // secondStart
            // 
            this.secondStart.Location = new System.Drawing.Point(355, 19);
            this.secondStart.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.secondStart.Name = "secondStart";
            this.secondStart.Size = new System.Drawing.Size(36, 20);
            this.secondStart.TabIndex = 13;
            this.secondStart.ValueChanged += new System.EventHandler(this.startSecondChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(116, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Hours:";
            // 
            // weekdayStart
            // 
            this.weekdayStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.weekdayStart.FormattingEnabled = true;
            this.weekdayStart.Items.AddRange(new object[] {
            "sunday",
            "monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday"});
            this.weekdayStart.Location = new System.Drawing.Point(45, 18);
            this.weekdayStart.Name = "weekdayStart";
            this.weekdayStart.Size = new System.Drawing.Size(65, 21);
            this.weekdayStart.TabIndex = 10;
            this.weekdayStart.SelectedIndexChanged += new System.EventHandler(this.weekdayStart_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(202, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Minutes:";
            // 
            // hourStart
            // 
            this.hourStart.Location = new System.Drawing.Point(160, 19);
            this.hourStart.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.hourStart.Name = "hourStart";
            this.hourStart.Size = new System.Drawing.Size(36, 20);
            this.hourStart.TabIndex = 11;
            this.hourStart.ValueChanged += new System.EventHandler(this.startHourChanged);
            // 
            // minuteStart
            // 
            this.minuteStart.Location = new System.Drawing.Point(255, 19);
            this.minuteStart.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.minuteStart.Name = "minuteStart";
            this.minuteStart.Size = new System.Drawing.Size(36, 20);
            this.minuteStart.TabIndex = 12;
            this.minuteStart.ValueChanged += new System.EventHandler(this.startMinuteChanged);
            // 
            // durationGroupBox
            // 
            this.durationGroupBox.Controls.Add(this.label9);
            this.durationGroupBox.Controls.Add(this.secondsDuration);
            this.durationGroupBox.Controls.Add(this.dayDuration);
            this.durationGroupBox.Controls.Add(this.label12);
            this.durationGroupBox.Controls.Add(this.label10);
            this.durationGroupBox.Controls.Add(this.minutesDuration);
            this.durationGroupBox.Controls.Add(this.hoursDuration);
            this.durationGroupBox.Controls.Add(this.label11);
            this.durationGroupBox.Location = new System.Drawing.Point(6, 76);
            this.durationGroupBox.Name = "durationGroupBox";
            this.durationGroupBox.Size = new System.Drawing.Size(397, 51);
            this.durationGroupBox.TabIndex = 18;
            this.durationGroupBox.TabStop = false;
            this.durationGroupBox.Text = "Duration";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Days:";
            // 
            // secondsDuration
            // 
            this.secondsDuration.Location = new System.Drawing.Point(355, 19);
            this.secondsDuration.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.secondsDuration.Name = "secondsDuration";
            this.secondsDuration.Size = new System.Drawing.Size(36, 20);
            this.secondsDuration.TabIndex = 17;
            this.secondsDuration.ValueChanged += new System.EventHandler(this.durationSecondChanged);
            // 
            // dayDuration
            // 
            this.dayDuration.Location = new System.Drawing.Point(74, 19);
            this.dayDuration.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.dayDuration.Name = "dayDuration";
            this.dayDuration.Size = new System.Drawing.Size(36, 20);
            this.dayDuration.TabIndex = 14;
            this.dayDuration.ValueChanged += new System.EventHandler(this.durationDayChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(297, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Seconds:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(116, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Hours:";
            // 
            // minutesDuration
            // 
            this.minutesDuration.Location = new System.Drawing.Point(255, 19);
            this.minutesDuration.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.minutesDuration.Name = "minutesDuration";
            this.minutesDuration.Size = new System.Drawing.Size(36, 20);
            this.minutesDuration.TabIndex = 16;
            this.minutesDuration.ValueChanged += new System.EventHandler(this.durationMinuteChanged);
            // 
            // hoursDuration
            // 
            this.hoursDuration.Location = new System.Drawing.Point(160, 19);
            this.hoursDuration.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.hoursDuration.Name = "hoursDuration";
            this.hoursDuration.Size = new System.Drawing.Size(36, 20);
            this.hoursDuration.TabIndex = 15;
            this.hoursDuration.ValueChanged += new System.EventHandler(this.durationHourChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(202, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Minutes:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.showButton);
            this.groupBox2.Controls.Add(this.createEventButton);
            this.groupBox2.Controls.Add(this.deleteEventButton);
            this.groupBox2.Controls.Add(this.eventsList);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 362);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Weekly Planning";
            // 
            // showButton
            // 
            this.showButton.Location = new System.Drawing.Point(171, 333);
            this.showButton.Name = "showButton";
            this.showButton.Size = new System.Drawing.Size(70, 23);
            this.showButton.TabIndex = 6;
            this.showButton.Text = "Show";
            this.showButton.UseVisualStyleBackColor = true;
            this.showButton.Click += new System.EventHandler(this.showButton_Click);
            // 
            // createEventButton
            // 
            this.createEventButton.Location = new System.Drawing.Point(6, 333);
            this.createEventButton.Name = "createEventButton";
            this.createEventButton.Size = new System.Drawing.Size(87, 23);
            this.createEventButton.TabIndex = 3;
            this.createEventButton.Text = "Create plan";
            this.createEventButton.UseVisualStyleBackColor = true;
            this.createEventButton.Click += new System.EventHandler(this.createEventButton_Click);
            // 
            // deleteEventButton
            // 
            this.deleteEventButton.Location = new System.Drawing.Point(99, 333);
            this.deleteEventButton.Name = "deleteEventButton";
            this.deleteEventButton.Size = new System.Drawing.Size(66, 23);
            this.deleteEventButton.TabIndex = 1;
            this.deleteEventButton.Text = "Delete";
            this.deleteEventButton.UseVisualStyleBackColor = true;
            this.deleteEventButton.Click += new System.EventHandler(this.deleteEventButton_Click);
            // 
            // saveConfig
            // 
            this.saveConfig.Enabled = false;
            this.saveConfig.Location = new System.Drawing.Point(538, 418);
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.Size = new System.Drawing.Size(75, 23);
            this.saveConfig.TabIndex = 20;
            this.saveConfig.Text = "Save";
            this.saveConfig.UseVisualStyleBackColor = true;
            this.saveConfig.Click += new System.EventHandler(this.saveConfig_Click);
            // 
            // closeConfig
            // 
            this.closeConfig.Location = new System.Drawing.Point(619, 418);
            this.closeConfig.Name = "closeConfig";
            this.closeConfig.Size = new System.Drawing.Size(75, 23);
            this.closeConfig.TabIndex = 21;
            this.closeConfig.Text = "Close";
            this.closeConfig.UseVisualStyleBackColor = true;
            this.closeConfig.Click += new System.EventHandler(this.closeConfig_Click);
            // 
            // proActiveLocationBrowser
            // 
            this.proActiveLocationBrowser.Description = "Choose location for ProActive";
            // 
            // jvmLocationBrowser
            // 
            this.jvmLocationBrowser.Description = "Choose location for JVM";
            // 
            // saveConfigAs
            // 
            this.saveConfigAs.Location = new System.Drawing.Point(457, 418);
            this.saveConfigAs.Name = "saveConfigAs";
            this.saveConfigAs.Size = new System.Drawing.Size(75, 23);
            this.saveConfigAs.TabIndex = 19;
            this.saveConfigAs.Text = "Save as ...";
            this.saveConfigAs.UseVisualStyleBackColor = true;
            this.saveConfigAs.Click += new System.EventHandler(this.saveConfigAs_Click);
            // 
            // scriptLocationFileDialog
            // 
            this.scriptLocationFileDialog.DefaultExt = "bat";
            this.scriptLocationFileDialog.Filter = "Scripts .bat/.cmd|*.bat;*.cmd|Executables .exe|*.exe";
            // 
            // eventsList
            // 
            this.eventsList.FormattingEnabled = true;
            this.eventsList.Location = new System.Drawing.Point(6, 19);
            this.eventsList.Name = "eventsList";
            this.eventsList.Size = new System.Drawing.Size(235, 303);
            this.eventsList.TabIndex = 0;
            this.eventsList.SelectedIndexChanged += new System.EventHandler(this.eventsList_SelectedIndexChanged);
            // 
            // ConfigurationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 453);
            this.Controls.Add(this.closeConfig);
            this.Controls.Add(this.saveConfigAs);
            this.Controls.Add(this.saveConfig);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigurationEditor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration Editor";
            this.Load += new System.EventHandler(this.ConfigEditor_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigEditor_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.generalTabPage.PerformLayout();
            this.onRuntimeExitGroupBox.ResumeLayout(false);
            this.onRuntimeExitGroupBox.PerformLayout();
            this.multiRuntimeGroupBox.ResumeLayout(false);
            this.multiRuntimeGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nbRuntimesNumericUpDown)).EndInit();
            this.networkInterfaceListGroupBox.ResumeLayout(false);
            this.memoryManagementGroupBox.ResumeLayout(false);
            this.memoryManagementGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.javaMemoryNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nativeMemoryNumericUpDown)).EndInit();
            this.connectionTabPage.ResumeLayout(false);
            this.proActiveCommunicationProtocolGroupBox.ResumeLayout(false);
            this.proActiveCommunicationProtocolGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portInitialValueNumericUpDown)).EndInit();
            this.enabledConnectionGroupBox.ResumeLayout(false);
            this.enabledConnectionGroupBox.PerformLayout();
            this.connectionTypeTabControl.ResumeLayout(false);
            this.rmiRegistrationTabPage.ResumeLayout(false);
            this.rmiRegistrationAdditionalConfigurationGroupBox.ResumeLayout(false);
            this.rmiRegistrationAdditionalConfigurationGroupBox.PerformLayout();
            this.rmiActionGroup.ResumeLayout(false);
            this.rmiActionGroup.PerformLayout();
            this.resourceManagerRegistrationTabPage.ResumeLayout(false);
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.ResumeLayout(false);
            this.resourceManagerRegistrationAdditionalConfigurationGroupBox.PerformLayout();
            this.rmActionGroup.ResumeLayout(false);
            this.rmActionGroup.PerformLayout();
            this.customTabPage.ResumeLayout(false);
            this.customAdditionalConfigurationGroupBox.ResumeLayout(false);
            this.customAdditionalConfigurationGroupBox.PerformLayout();
            this.customActionGroup.ResumeLayout(false);
            this.customActionGroup.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.eventEditorGroup.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxCpuUsageNumericUpDown)).EndInit();
            this.startTimeGroupBox.ResumeLayout(false);
            this.startTimeGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minuteStart)).EndInit();
            this.durationGroupBox.ResumeLayout(false);
            this.durationGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondsDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutesDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hoursDuration)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox jvmDirectory;
        private System.Windows.Forms.TextBox proactiveDirectory;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button jvmLocationButton;
        private System.Windows.Forms.Button proactiveLocationButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button saveConfig;
        private System.Windows.Forms.Button closeConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private RefreshingListBox eventsList;
        private System.Windows.Forms.GroupBox eventEditorGroup;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown hourStart;
        private System.Windows.Forms.ComboBox weekdayStart;
        private System.Windows.Forms.Button createEventButton;
        private System.Windows.Forms.Button deleteEventButton;
        private System.Windows.Forms.NumericUpDown secondStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown minuteStart;
        private System.Windows.Forms.NumericUpDown dayDuration;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown minutesDuration;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown hoursDuration;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown secondsDuration;
        private System.Windows.Forms.FolderBrowserDialog proActiveLocationBrowser;
        private System.Windows.Forms.FolderBrowserDialog jvmLocationBrowser;
        private System.Windows.Forms.Button saveConfigAs;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button showButton;
        private System.Windows.Forms.CheckBox alwaysAvailableCheckBox;
        private System.Windows.Forms.GroupBox memoryManagementGroupBox;
        private System.Windows.Forms.Label nativeMemoryLabel;
        private System.Windows.Forms.NumericUpDown nativeMemoryNumericUpDown;
        private System.Windows.Forms.Label availablePhysicalMemoryLabel;
        private System.Windows.Forms.Label availablePhysicalMemoryValue;
        private System.Windows.Forms.Button removeJvmParameterButton;
        private System.Windows.Forms.Button addJvmParameterButton;
        private System.Windows.Forms.ListBox jvmParametersListBox;
        private System.Windows.Forms.Label javaMemoryLabel;
        private System.Windows.Forms.NumericUpDown javaMemoryNumericUpDown;
        private System.Windows.Forms.Label totalProcessMemoryLabel;
        private System.Windows.Forms.Label totalProcessMemoryValue;
        private System.Windows.Forms.GroupBox durationGroupBox;
        private System.Windows.Forms.GroupBox startTimeGroupBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label processPriorityLabel;
        private System.Windows.Forms.ComboBox processPriorityComboBox;
        private System.Windows.Forms.CheckBox enableMemoryManagementCheckBox;
        private System.Windows.Forms.TabPage connectionTabPage;
        private System.Windows.Forms.TabPage rmiRegistrationTabPage;
        private System.Windows.Forms.TabPage resourceManagerRegistrationTabPage;
        private System.Windows.Forms.TabPage customTabPage;
        private System.Windows.Forms.RadioButton rmiRegistrationRadioButton;
        private System.Windows.Forms.RadioButton customRadioButton;
        private System.Windows.Forms.RadioButton resourceManagerRegistrationRadioButton;
        private System.Windows.Forms.TabControl connectionTypeTabControl;
        private System.Windows.Forms.GroupBox customActionGroup;
        private System.Windows.Forms.Button customSaveArgumentButton;
        private System.Windows.Forms.TextBox customArgumentTextBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button customDeleteButton;
        private System.Windows.Forms.Button customAddButton;
        private System.Windows.Forms.ListBox customArgumentsListBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox rmiActionGroup;
        private System.Windows.Forms.TextBox rmiNodeName;
        private System.Windows.Forms.CheckBox rmiNodeEnabled;
        private System.Windows.Forms.GroupBox rmActionGroup;
        private System.Windows.Forms.TextBox rmUrl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox rmNodeName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox rmiRegistrationAdditionalConfigurationGroupBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox rmiRegistrationJavaActionClassTextBox;
        private System.Windows.Forms.GroupBox resourceManagerRegistrationAdditionalConfigurationGroupBox;
        private System.Windows.Forms.TextBox resourceManagerRegistrationJavaActionClassTextBox;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox customAdditionalConfigurationGroupBox;
        private System.Windows.Forms.TextBox customJavaActionClassTextBox;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox enabledConnectionGroupBox;
        private System.Windows.Forms.Label maxCpuUsageLabel;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown maxCpuUsageNumericUpDown;
        private System.Windows.Forms.Label rmUsernameLabel;
        private System.Windows.Forms.Label rmPasswordLabel;
        private System.Windows.Forms.TextBox rmUsernameTextBox;
        private System.Windows.Forms.TextBox rmPasswordTextBox;
        private System.Windows.Forms.CheckBox rmAnonymousCheckBox;
        private System.Windows.Forms.GroupBox networkInterfaceListGroupBox;
        private System.Windows.Forms.Button refreshNetworkInterfacesButton;
        private System.Windows.Forms.ListBox networkInterfacesListBox;
        private System.Windows.Forms.Button useNetworkInterfaceButton;
        private System.Windows.Forms.GroupBox multiRuntimeGroupBox;
        private System.Windows.Forms.Label availableCPUsValue;
        private System.Windows.Forms.Label availableCPUsLabel;
        private System.Windows.Forms.NumericUpDown nbRuntimesNumericUpDown;
        private System.Windows.Forms.Label nbRuntimesLabel;
        private System.Windows.Forms.CheckBox useAllAvailableCPUsCheckBox;
        private System.Windows.Forms.GroupBox proActiveCommunicationProtocolGroupBox;
        private System.Windows.Forms.NumericUpDown portInitialValueNumericUpDown;
        private System.Windows.Forms.Label portInitialValue;
        private System.Windows.Forms.GroupBox onRuntimeExitGroupBox;
        private System.Windows.Forms.TextBox scriptLocationTextBox;
        private System.Windows.Forms.Button scriptLocationButton;
        private System.Windows.Forms.Label scriptLocationLabel;
        private System.Windows.Forms.OpenFileDialog scriptLocationFileDialog;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label protocolLabel;
        private System.Windows.Forms.ComboBox protocolComboBox;
    }


}