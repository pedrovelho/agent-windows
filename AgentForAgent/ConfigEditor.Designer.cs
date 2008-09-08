namespace AgentForAgent
{
    partial class ConfigEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.jvmLocationButton = new System.Windows.Forms.Button();
            this.proactiveLocationButton = new System.Windows.Forms.Button();
            this.jvmParams = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.jvmDirectory = new System.Windows.Forms.TextBox();
            this.proactiveLocation = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.eventEditorGroup = new System.Windows.Forms.GroupBox();
            this.saveEventButton = new System.Windows.Forms.Button();
            this.secondsDuration = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.minutesDuration = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.hoursDuration = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.dayDuration = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.secondStart = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.minuteStart = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.hourStart = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.weekdayStart = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.newEventButton = new System.Windows.Forms.Button();
            this.deleteEventButton = new System.Windows.Forms.Button();
            this.eventsList = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.priorityBox = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.rmRadioButton = new System.Windows.Forms.RadioButton();
            this.rmiRadioButton = new System.Windows.Forms.RadioButton();
            this.p2pRadioButton = new System.Windows.Forms.RadioButton();
            this.p2pactionGroup = new System.Windows.Forms.GroupBox();
            this.p2pProtocol = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.saveHost = new System.Windows.Forms.Button();
            this.peerUrl = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.deleteHost = new System.Windows.Forms.Button();
            this.addHost = new System.Windows.Forms.Button();
            this.hostList = new System.Windows.Forms.ListBox();
            this.label14 = new System.Windows.Forms.Label();
            this.rmActionGroup = new System.Windows.Forms.GroupBox();
            this.rmUrl = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.rmiActionGroup = new System.Windows.Forms.GroupBox();
            this.rmiNodeName = new System.Windows.Forms.TextBox();
            this.rmiNodeEnabled = new System.Windows.Forms.CheckBox();
            this.saveConfig = new System.Windows.Forms.Button();
            this.closeConfig = new System.Windows.Forms.Button();
            this.proActiveLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.jvmLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.eventEditorGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondsDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutesDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hoursDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.secondStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minuteStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourStart)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.p2pactionGroup.SuspendLayout();
            this.rmActionGroup.SuspendLayout();
            this.rmiActionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.jvmLocationButton);
            this.groupBox1.Controls.Add(this.proactiveLocationButton);
            this.groupBox1.Controls.Add(this.jvmParams);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.jvmDirectory);
            this.groupBox1.Controls.Add(this.proactiveLocation);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(603, 175);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ProActive Configuration";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(127, 72);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(169, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Use system-wide JVM location";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // jvmLocationButton
            // 
            this.jvmLocationButton.Location = new System.Drawing.Point(502, 45);
            this.jvmLocationButton.Name = "jvmLocationButton";
            this.jvmLocationButton.Size = new System.Drawing.Size(75, 23);
            this.jvmLocationButton.TabIndex = 8;
            this.jvmLocationButton.Text = "Browse...";
            this.jvmLocationButton.UseVisualStyleBackColor = true;
            this.jvmLocationButton.Click += new System.EventHandler(this.jvmLocationButton_Click);
            // 
            // proactiveLocationButton
            // 
            this.proactiveLocationButton.Location = new System.Drawing.Point(501, 19);
            this.proactiveLocationButton.Name = "proactiveLocationButton";
            this.proactiveLocationButton.Size = new System.Drawing.Size(75, 23);
            this.proactiveLocationButton.TabIndex = 7;
            this.proactiveLocationButton.Text = "Browse...";
            this.proactiveLocationButton.UseVisualStyleBackColor = true;
            this.proactiveLocationButton.Click += new System.EventHandler(this.proactiveLocationButton_Click);
            // 
            // jvmParams
            // 
            this.jvmParams.Location = new System.Drawing.Point(127, 95);
            this.jvmParams.Multiline = true;
            this.jvmParams.Name = "jvmParams";
            this.jvmParams.Size = new System.Drawing.Size(368, 70);
            this.jvmParams.TabIndex = 6;
            this.jvmParams.TextChanged += new System.EventHandler(this.jvmParams_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "JVM Parameters:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
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
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "ProActive Location:";
            // 
            // jvmDirectory
            // 
            this.jvmDirectory.Location = new System.Drawing.Point(127, 45);
            this.jvmDirectory.Name = "jvmDirectory";
            this.jvmDirectory.Size = new System.Drawing.Size(368, 20);
            this.jvmDirectory.TabIndex = 1;
            this.jvmDirectory.TextChanged += new System.EventHandler(this.jvmDirectory_TextChanged);
            // 
            // proactiveLocation
            // 
            this.proactiveLocation.Location = new System.Drawing.Point(127, 19);
            this.proactiveLocation.Name = "proactiveLocation";
            this.proactiveLocation.Size = new System.Drawing.Size(368, 20);
            this.proactiveLocation.TabIndex = 0;
            this.proactiveLocation.TextChanged += new System.EventHandler(this.proactiveLocation_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(682, 293);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(674, 267);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ProActive Configuration";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.eventEditorGroup);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(674, 267);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Events";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // eventEditorGroup
            // 
            this.eventEditorGroup.Controls.Add(this.saveEventButton);
            this.eventEditorGroup.Controls.Add(this.secondsDuration);
            this.eventEditorGroup.Controls.Add(this.label12);
            this.eventEditorGroup.Controls.Add(this.minutesDuration);
            this.eventEditorGroup.Controls.Add(this.label11);
            this.eventEditorGroup.Controls.Add(this.hoursDuration);
            this.eventEditorGroup.Controls.Add(this.label10);
            this.eventEditorGroup.Controls.Add(this.dayDuration);
            this.eventEditorGroup.Controls.Add(this.label9);
            this.eventEditorGroup.Controls.Add(this.label8);
            this.eventEditorGroup.Controls.Add(this.secondStart);
            this.eventEditorGroup.Controls.Add(this.label7);
            this.eventEditorGroup.Controls.Add(this.minuteStart);
            this.eventEditorGroup.Controls.Add(this.label6);
            this.eventEditorGroup.Controls.Add(this.label5);
            this.eventEditorGroup.Controls.Add(this.hourStart);
            this.eventEditorGroup.Controls.Add(this.label4);
            this.eventEditorGroup.Controls.Add(this.weekdayStart);
            this.eventEditorGroup.Enabled = false;
            this.eventEditorGroup.Location = new System.Drawing.Point(259, 6);
            this.eventEditorGroup.Name = "eventEditorGroup";
            this.eventEditorGroup.Size = new System.Drawing.Size(398, 255);
            this.eventEditorGroup.TabIndex = 1;
            this.eventEditorGroup.TabStop = false;
            this.eventEditorGroup.Text = "Event Editor";
            // 
            // saveEventButton
            // 
            this.saveEventButton.Location = new System.Drawing.Point(317, 226);
            this.saveEventButton.Name = "saveEventButton";
            this.saveEventButton.Size = new System.Drawing.Size(75, 23);
            this.saveEventButton.TabIndex = 17;
            this.saveEventButton.Text = "Save Event";
            this.saveEventButton.UseVisualStyleBackColor = true;
            this.saveEventButton.Click += new System.EventHandler(this.saveEventButton_Click);
            // 
            // secondsDuration
            // 
            this.secondsDuration.Location = new System.Drawing.Point(350, 155);
            this.secondsDuration.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.secondsDuration.Name = "secondsDuration";
            this.secondsDuration.Size = new System.Drawing.Size(32, 20);
            this.secondsDuration.TabIndex = 16;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(292, 157);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Seconds:";
            // 
            // minutesDuration
            // 
            this.minutesDuration.Location = new System.Drawing.Point(246, 157);
            this.minutesDuration.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.minutesDuration.Name = "minutesDuration";
            this.minutesDuration.Size = new System.Drawing.Size(34, 20);
            this.minutesDuration.TabIndex = 14;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(191, 157);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Minutes:";
            // 
            // hoursDuration
            // 
            this.hoursDuration.Location = new System.Drawing.Point(146, 157);
            this.hoursDuration.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.hoursDuration.Name = "hoursDuration";
            this.hoursDuration.Size = new System.Drawing.Size(36, 20);
            this.hoursDuration.TabIndex = 12;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(96, 157);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Hours:";
            // 
            // dayDuration
            // 
            this.dayDuration.Location = new System.Drawing.Point(54, 157);
            this.dayDuration.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.dayDuration.Name = "dayDuration";
            this.dayDuration.Size = new System.Drawing.Size(33, 20);
            this.dayDuration.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 157);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Days:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Duration:";
            // 
            // secondStart
            // 
            this.secondStart.Location = new System.Drawing.Point(246, 83);
            this.secondStart.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.secondStart.Name = "secondStart";
            this.secondStart.Size = new System.Drawing.Size(34, 20);
            this.secondStart.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(188, 85);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Seconds:";
            // 
            // minuteStart
            // 
            this.minuteStart.Location = new System.Drawing.Point(146, 83);
            this.minuteStart.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.minuteStart.Name = "minuteStart";
            this.minuteStart.Size = new System.Drawing.Size(36, 20);
            this.minuteStart.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(93, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Minutes:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Hours:";
            // 
            // hourStart
            // 
            this.hourStart.Location = new System.Drawing.Point(54, 83);
            this.hourStart.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.hourStart.Name = "hourStart";
            this.hourStart.Size = new System.Drawing.Size(33, 20);
            this.hourStart.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Start time:";
            // 
            // weekdayStart
            // 
            this.weekdayStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.weekdayStart.FormattingEnabled = true;
            this.weekdayStart.Items.AddRange(new object[] {
            "monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday",
            "sunday"});
            this.weekdayStart.Location = new System.Drawing.Point(10, 46);
            this.weekdayStart.Name = "weekdayStart";
            this.weekdayStart.Size = new System.Drawing.Size(121, 21);
            this.weekdayStart.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.newEventButton);
            this.groupBox2.Controls.Add(this.deleteEventButton);
            this.groupBox2.Controls.Add(this.eventsList);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 255);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Events";
            // 
            // newEventButton
            // 
            this.newEventButton.Location = new System.Drawing.Point(6, 226);
            this.newEventButton.Name = "newEventButton";
            this.newEventButton.Size = new System.Drawing.Size(114, 23);
            this.newEventButton.TabIndex = 3;
            this.newEventButton.Text = "Create new event";
            this.newEventButton.UseVisualStyleBackColor = true;
            this.newEventButton.Click += new System.EventHandler(this.newEventButton_Click);
            // 
            // deleteEventButton
            // 
            this.deleteEventButton.Location = new System.Drawing.Point(166, 226);
            this.deleteEventButton.Name = "deleteEventButton";
            this.deleteEventButton.Size = new System.Drawing.Size(75, 23);
            this.deleteEventButton.TabIndex = 1;
            this.deleteEventButton.Text = "Delete";
            this.deleteEventButton.UseVisualStyleBackColor = true;
            this.deleteEventButton.Click += new System.EventHandler(this.deleteEventButton_Click);
            // 
            // eventsList
            // 
            this.eventsList.FormattingEnabled = true;
            this.eventsList.Location = new System.Drawing.Point(6, 19);
            this.eventsList.Name = "eventsList";
            this.eventsList.Size = new System.Drawing.Size(235, 199);
            this.eventsList.TabIndex = 0;
            this.eventsList.SelectedIndexChanged += new System.EventHandler(this.eventsList_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Controls.Add(this.p2pactionGroup);
            this.tabPage3.Controls.Add(this.rmActionGroup);
            this.tabPage3.Controls.Add(this.rmiActionGroup);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(674, 267);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Action";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.priorityBox);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Controls.Add(this.rmRadioButton);
            this.groupBox7.Controls.Add(this.rmiRadioButton);
            this.groupBox7.Controls.Add(this.p2pRadioButton);
            this.groupBox7.Location = new System.Drawing.Point(9, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(657, 51);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Choose action type:";
            // 
            // priorityBox
            // 
            this.priorityBox.FormattingEnabled = true;
            this.priorityBox.Items.AddRange(new object[] {
            "Idle",
            "Normal",
            "High",
            "Realtime"});
            this.priorityBox.Location = new System.Drawing.Point(544, 17);
            this.priorityBox.Name = "priorityBox";
            this.priorityBox.Size = new System.Drawing.Size(106, 21);
            this.priorityBox.TabIndex = 4;
            this.priorityBox.SelectedIndexChanged += new System.EventHandler(this.priorityBox_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(457, 20);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(81, 13);
            this.label16.TabIndex = 3;
            this.label16.Text = "Process priority:";
            // 
            // rmRadioButton
            // 
            this.rmRadioButton.AutoSize = true;
            this.rmRadioButton.Location = new System.Drawing.Point(275, 19);
            this.rmRadioButton.Name = "rmRadioButton";
            this.rmRadioButton.Size = new System.Drawing.Size(175, 17);
            this.rmRadioButton.TabIndex = 2;
            this.rmRadioButton.Text = "Resource Manager Registration";
            this.rmRadioButton.UseVisualStyleBackColor = true;
            this.rmRadioButton.CheckedChanged += new System.EventHandler(this.rmRadioButton_CheckedChanged);
            // 
            // rmiRadioButton
            // 
            this.rmiRadioButton.AutoSize = true;
            this.rmiRadioButton.Checked = true;
            this.rmiRadioButton.Location = new System.Drawing.Point(165, 19);
            this.rmiRadioButton.Name = "rmiRadioButton";
            this.rmiRadioButton.Size = new System.Drawing.Size(104, 17);
            this.rmiRadioButton.TabIndex = 1;
            this.rmiRadioButton.TabStop = true;
            this.rmiRadioButton.Text = "RMI Registration";
            this.rmiRadioButton.UseVisualStyleBackColor = true;
            this.rmiRadioButton.CheckedChanged += new System.EventHandler(this.rmiRadioButton_CheckedChanged);
            // 
            // p2pRadioButton
            // 
            this.p2pRadioButton.AutoSize = true;
            this.p2pRadioButton.Location = new System.Drawing.Point(7, 20);
            this.p2pRadioButton.Name = "p2pRadioButton";
            this.p2pRadioButton.Size = new System.Drawing.Size(152, 17);
            this.p2pRadioButton.TabIndex = 0;
            this.p2pRadioButton.Text = "Peer-To-Peer Collaboration";
            this.p2pRadioButton.UseVisualStyleBackColor = true;
            this.p2pRadioButton.CheckedChanged += new System.EventHandler(this.p2pRadioButton_CheckedChanged);
            // 
            // p2pactionGroup
            // 
            this.p2pactionGroup.Controls.Add(this.p2pProtocol);
            this.p2pactionGroup.Controls.Add(this.label17);
            this.p2pactionGroup.Controls.Add(this.saveHost);
            this.p2pactionGroup.Controls.Add(this.peerUrl);
            this.p2pactionGroup.Controls.Add(this.label15);
            this.p2pactionGroup.Controls.Add(this.deleteHost);
            this.p2pactionGroup.Controls.Add(this.addHost);
            this.p2pactionGroup.Controls.Add(this.hostList);
            this.p2pactionGroup.Controls.Add(this.label14);
            this.p2pactionGroup.Location = new System.Drawing.Point(9, 61);
            this.p2pactionGroup.Name = "p2pactionGroup";
            this.p2pactionGroup.Size = new System.Drawing.Size(657, 136);
            this.p2pactionGroup.TabIndex = 2;
            this.p2pactionGroup.TabStop = false;
            this.p2pactionGroup.Text = "Peer-To-Peer Collaboration";
            // 
            // p2pProtocol
            // 
            this.p2pProtocol.Location = new System.Drawing.Point(338, 33);
            this.p2pProtocol.Name = "p2pProtocol";
            this.p2pProtocol.Size = new System.Drawing.Size(313, 20);
            this.p2pProtocol.TabIndex = 8;
            this.p2pProtocol.TextChanged += new System.EventHandler(this.p2pProtocol_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(282, 36);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(49, 13);
            this.label17.TabIndex = 7;
            this.label17.Text = "Protocol:";
            // 
            // saveHost
            // 
            this.saveHost.Location = new System.Drawing.Point(576, 107);
            this.saveHost.Name = "saveHost";
            this.saveHost.Size = new System.Drawing.Size(75, 23);
            this.saveHost.TabIndex = 6;
            this.saveHost.Text = "Save Peer";
            this.saveHost.UseVisualStyleBackColor = true;
            this.saveHost.Click += new System.EventHandler(this.saveHost_Click);
            // 
            // peerUrl
            // 
            this.peerUrl.Location = new System.Drawing.Point(285, 82);
            this.peerUrl.Name = "peerUrl";
            this.peerUrl.Size = new System.Drawing.Size(366, 20);
            this.peerUrl.TabIndex = 5;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(282, 65);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "Peer URL:";
            // 
            // deleteHost
            // 
            this.deleteHost.Location = new System.Drawing.Point(90, 104);
            this.deleteHost.Name = "deleteHost";
            this.deleteHost.Size = new System.Drawing.Size(75, 23);
            this.deleteHost.TabIndex = 3;
            this.deleteHost.Text = "Delete";
            this.deleteHost.UseVisualStyleBackColor = true;
            this.deleteHost.Click += new System.EventHandler(this.deleteHost_Click);
            // 
            // addHost
            // 
            this.addHost.Location = new System.Drawing.Point(9, 104);
            this.addHost.Name = "addHost";
            this.addHost.Size = new System.Drawing.Size(75, 23);
            this.addHost.TabIndex = 2;
            this.addHost.Text = "Add";
            this.addHost.UseVisualStyleBackColor = true;
            this.addHost.Click += new System.EventHandler(this.addHost_Click);
            // 
            // hostList
            // 
            this.hostList.FormattingEnabled = true;
            this.hostList.Location = new System.Drawing.Point(9, 33);
            this.hostList.Name = "hostList";
            this.hostList.Size = new System.Drawing.Size(264, 69);
            this.hostList.TabIndex = 1;
            this.hostList.SelectedIndexChanged += new System.EventHandler(this.hostList_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(99, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "First Contact Peers:";
            // 
            // rmActionGroup
            // 
            this.rmActionGroup.Controls.Add(this.rmUrl);
            this.rmActionGroup.Controls.Add(this.label13);
            this.rmActionGroup.Enabled = false;
            this.rmActionGroup.Location = new System.Drawing.Point(294, 203);
            this.rmActionGroup.Name = "rmActionGroup";
            this.rmActionGroup.Size = new System.Drawing.Size(372, 53);
            this.rmActionGroup.TabIndex = 1;
            this.rmActionGroup.TabStop = false;
            this.rmActionGroup.Text = "Resource Manager Registration";
            // 
            // rmUrl
            // 
            this.rmUrl.Location = new System.Drawing.Point(139, 16);
            this.rmUrl.Name = "rmUrl";
            this.rmUrl.Size = new System.Drawing.Size(227, 20);
            this.rmUrl.TabIndex = 1;
            this.rmUrl.TextChanged += new System.EventHandler(this.rmUrl_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(126, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Resource Manager URL:";
            // 
            // rmiActionGroup
            // 
            this.rmiActionGroup.Controls.Add(this.rmiNodeName);
            this.rmiActionGroup.Controls.Add(this.rmiNodeEnabled);
            this.rmiActionGroup.Enabled = false;
            this.rmiActionGroup.Location = new System.Drawing.Point(9, 203);
            this.rmiActionGroup.Name = "rmiActionGroup";
            this.rmiActionGroup.Size = new System.Drawing.Size(279, 53);
            this.rmiActionGroup.TabIndex = 0;
            this.rmiActionGroup.TabStop = false;
            this.rmiActionGroup.Text = "RMI Registration";
            // 
            // rmiNodeName
            // 
            this.rmiNodeName.Enabled = false;
            this.rmiNodeName.Location = new System.Drawing.Point(96, 19);
            this.rmiNodeName.Name = "rmiNodeName";
            this.rmiNodeName.Size = new System.Drawing.Size(177, 20);
            this.rmiNodeName.TabIndex = 1;
            this.rmiNodeName.TextChanged += new System.EventHandler(this.rmiNodeName_TextChanged);
            // 
            // rmiNodeEnabled
            // 
            this.rmiNodeEnabled.AutoSize = true;
            this.rmiNodeEnabled.Location = new System.Drawing.Point(6, 21);
            this.rmiNodeEnabled.Name = "rmiNodeEnabled";
            this.rmiNodeEnabled.Size = new System.Drawing.Size(84, 17);
            this.rmiNodeEnabled.TabIndex = 0;
            this.rmiNodeEnabled.Text = "Node name:";
            this.rmiNodeEnabled.UseVisualStyleBackColor = true;
            this.rmiNodeEnabled.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // saveConfig
            // 
            this.saveConfig.Location = new System.Drawing.Point(534, 311);
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.Size = new System.Drawing.Size(75, 23);
            this.saveConfig.TabIndex = 2;
            this.saveConfig.Text = "Save";
            this.saveConfig.UseVisualStyleBackColor = true;
            this.saveConfig.Click += new System.EventHandler(this.saveConfig_Click);
            // 
            // closeConfig
            // 
            this.closeConfig.Location = new System.Drawing.Point(615, 311);
            this.closeConfig.Name = "closeConfig";
            this.closeConfig.Size = new System.Drawing.Size(75, 23);
            this.closeConfig.TabIndex = 3;
            this.closeConfig.Text = "Cancel";
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
            // ConfigEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 342);
            this.Controls.Add(this.closeConfig);
            this.Controls.Add(this.saveConfig);
            this.Controls.Add(this.tabControl1);
            this.Name = "ConfigEditor";
            this.Text = "ConfigEditor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.eventEditorGroup.ResumeLayout(false);
            this.eventEditorGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondsDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minutesDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hoursDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.secondStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minuteStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourStart)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.p2pactionGroup.ResumeLayout(false);
            this.p2pactionGroup.PerformLayout();
            this.rmActionGroup.ResumeLayout(false);
            this.rmActionGroup.PerformLayout();
            this.rmiActionGroup.ResumeLayout(false);
            this.rmiActionGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox jvmDirectory;
        private System.Windows.Forms.TextBox proactiveLocation;
        private System.Windows.Forms.TextBox jvmParams;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button jvmLocationButton;
        private System.Windows.Forms.Button proactiveLocationButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button saveConfig;
        private System.Windows.Forms.Button closeConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox eventsList;
        private System.Windows.Forms.GroupBox eventEditorGroup;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown hourStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox weekdayStart;
        private System.Windows.Forms.Button newEventButton;
        private System.Windows.Forms.Button deleteEventButton;
        private System.Windows.Forms.NumericUpDown secondStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown minuteStart;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown dayDuration;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown minutesDuration;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown hoursDuration;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown secondsDuration;
        private System.Windows.Forms.GroupBox rmiActionGroup;
        private System.Windows.Forms.TextBox rmiNodeName;
        private System.Windows.Forms.CheckBox rmiNodeEnabled;
        private System.Windows.Forms.GroupBox rmActionGroup;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox rmUrl;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton p2pRadioButton;
        private System.Windows.Forms.GroupBox p2pactionGroup;
        private System.Windows.Forms.TextBox peerUrl;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button deleteHost;
        private System.Windows.Forms.Button addHost;
        private System.Windows.Forms.ListBox hostList;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RadioButton rmRadioButton;
        private System.Windows.Forms.RadioButton rmiRadioButton;
        private System.Windows.Forms.FolderBrowserDialog proActiveLocationBrowser;
        private System.Windows.Forms.FolderBrowserDialog jvmLocationBrowser;
        private System.Windows.Forms.Button saveEventButton;
        private System.Windows.Forms.Button saveHost;
        private System.Windows.Forms.ComboBox priorityBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox p2pProtocol;
        private System.Windows.Forms.Label label17;
    }
}