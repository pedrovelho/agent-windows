namespace AgentForAgent
{
    partial class ConfigurationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialog));
            this.configurationFileLocationLabel = new System.Windows.Forms.Label();
            this.configLocation = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.guiEditButton = new System.Windows.Forms.Button();
            this.textEditConfig = new System.Windows.Forms.Button();
            this.infoGroupBox = new System.Windows.Forms.GroupBox();
            this.spawnedRuntimesValue = new System.Windows.Forms.Label();
            this.agentStatusValue = new System.Windows.Forms.Label();
            this.spawnedRuntimesLabel = new System.Windows.Forms.Label();
            this.agentStatusLabel = new System.Windows.Forms.Label();
            this.viewLogsWithNotepadButton = new System.Windows.Forms.Button();
            this.stopService = new System.Windows.Forms.Button();
            this.startService = new System.Windows.Forms.Button();
            this.proActiveLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.browseConfig = new System.Windows.Forms.OpenFileDialog();
            this.agentStatusNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAdministrationPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.proActiveInriaLinkLabel = new System.Windows.Forms.LinkLabel();
            this.controlsGroupBox = new System.Windows.Forms.GroupBox();
            this.logsGroupBox = new System.Windows.Forms.GroupBox();
            this.viewLogsWithExplorerButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.infoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.proActiveLogoPictureBox)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.controlsGroupBox.SuspendLayout();
            this.logsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // configurationFileLocationLabel
            // 
            this.configurationFileLocationLabel.AutoSize = true;
            this.configurationFileLocationLabel.Location = new System.Drawing.Point(6, 21);
            this.configurationFileLocationLabel.Name = "configurationFileLocationLabel";
            this.configurationFileLocationLabel.Size = new System.Drawing.Size(128, 13);
            this.configurationFileLocationLabel.TabIndex = 0;
            this.configurationFileLocationLabel.Text = "Configuration file location:";
            // 
            // configLocation
            // 
            this.configLocation.Location = new System.Drawing.Point(140, 18);
            this.configLocation.Name = "configLocation";
            this.configLocation.ReadOnly = true;
            this.configLocation.Size = new System.Drawing.Size(339, 20);
            this.configLocation.TabIndex = 1;
            this.configLocation.Text = "C:\\PAAgent-config.xml";
            this.configLocation.TextChanged += new System.EventHandler(this.configLocation_TextChanged);
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(487, 16);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 2;
            this.browse.Text = "Load...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.guiEditButton);
            this.groupBox1.Controls.Add(this.configLocation);
            this.groupBox1.Controls.Add(this.textEditConfig);
            this.groupBox1.Controls.Add(this.browse);
            this.groupBox1.Controls.Add(this.configurationFileLocationLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 81);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // guiEditButton
            // 
            this.guiEditButton.Location = new System.Drawing.Point(404, 48);
            this.guiEditButton.Name = "guiEditButton";
            this.guiEditButton.Size = new System.Drawing.Size(75, 23);
            this.guiEditButton.TabIndex = 3;
            this.guiEditButton.Text = "GUI Edit";
            this.guiEditButton.UseVisualStyleBackColor = true;
            this.guiEditButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // textEditConfig
            // 
            this.textEditConfig.Location = new System.Drawing.Point(487, 48);
            this.textEditConfig.Name = "textEditConfig";
            this.textEditConfig.Size = new System.Drawing.Size(75, 23);
            this.textEditConfig.TabIndex = 2;
            this.textEditConfig.Text = "Text Edit";
            this.textEditConfig.UseVisualStyleBackColor = true;
            this.textEditConfig.Click += new System.EventHandler(this.editConfig_Click);
            // 
            // infoGroupBox
            // 
            this.infoGroupBox.Controls.Add(this.spawnedRuntimesValue);
            this.infoGroupBox.Controls.Add(this.agentStatusValue);
            this.infoGroupBox.Controls.Add(this.spawnedRuntimesLabel);
            this.infoGroupBox.Controls.Add(this.agentStatusLabel);
            this.infoGroupBox.Location = new System.Drawing.Point(12, 204);
            this.infoGroupBox.Name = "infoGroupBox";
            this.infoGroupBox.Size = new System.Drawing.Size(186, 81);
            this.infoGroupBox.TabIndex = 3;
            this.infoGroupBox.TabStop = false;
            this.infoGroupBox.Text = "Info";
            // 
            // spawnedRuntimesValue
            // 
            this.spawnedRuntimesValue.AutoSize = true;
            this.spawnedRuntimesValue.Location = new System.Drawing.Point(119, 53);
            this.spawnedRuntimesValue.Name = "spawnedRuntimesValue";
            this.spawnedRuntimesValue.Size = new System.Drawing.Size(13, 13);
            this.spawnedRuntimesValue.TabIndex = 3;
            this.spawnedRuntimesValue.Text = "0";
            // 
            // agentStatusValue
            // 
            this.agentStatusValue.AutoSize = true;
            this.agentStatusValue.Location = new System.Drawing.Point(52, 24);
            this.agentStatusValue.Name = "agentStatusValue";
            this.agentStatusValue.Size = new System.Drawing.Size(53, 13);
            this.agentStatusValue.TabIndex = 2;
            this.agentStatusValue.Text = "Unknown";
            // 
            // spawnedRuntimesLabel
            // 
            this.spawnedRuntimesLabel.AutoSize = true;
            this.spawnedRuntimesLabel.Location = new System.Drawing.Point(6, 53);
            this.spawnedRuntimesLabel.Name = "spawnedRuntimesLabel";
            this.spawnedRuntimesLabel.Size = new System.Drawing.Size(102, 13);
            this.spawnedRuntimesLabel.TabIndex = 1;
            this.spawnedRuntimesLabel.Text = "Spawned Runtimes:";
            // 
            // agentStatusLabel
            // 
            this.agentStatusLabel.AutoSize = true;
            this.agentStatusLabel.Location = new System.Drawing.Point(6, 24);
            this.agentStatusLabel.Name = "agentStatusLabel";
            this.agentStatusLabel.Size = new System.Drawing.Size(40, 13);
            this.agentStatusLabel.TabIndex = 0;
            this.agentStatusLabel.Text = "Status:";
            // 
            // viewLogsWithNotepadButton
            // 
            this.viewLogsWithNotepadButton.Location = new System.Drawing.Point(61, 19);
            this.viewLogsWithNotepadButton.Name = "viewLogsWithNotepadButton";
            this.viewLogsWithNotepadButton.Size = new System.Drawing.Size(118, 23);
            this.viewLogsWithNotepadButton.TabIndex = 4;
            this.viewLogsWithNotepadButton.Text = "View with Notepad";
            this.viewLogsWithNotepadButton.UseVisualStyleBackColor = true;
            this.viewLogsWithNotepadButton.Click += new System.EventHandler(this.viewLogs_Click);
            // 
            // stopService
            // 
            this.stopService.Location = new System.Drawing.Point(104, 48);
            this.stopService.Name = "stopService";
            this.stopService.Size = new System.Drawing.Size(75, 23);
            this.stopService.TabIndex = 2;
            this.stopService.Text = "Stop";
            this.stopService.UseVisualStyleBackColor = true;
            this.stopService.Click += new System.EventHandler(this.stopService_Click);
            // 
            // startService
            // 
            this.startService.Location = new System.Drawing.Point(104, 19);
            this.startService.Name = "startService";
            this.startService.Size = new System.Drawing.Size(75, 23);
            this.startService.TabIndex = 2;
            this.startService.Text = "Start";
            this.startService.UseVisualStyleBackColor = true;
            this.startService.Click += new System.EventHandler(this.startService_Click);
            // 
            // proActiveLogoPictureBox
            // 
            this.proActiveLogoPictureBox.BackColor = System.Drawing.Color.White;
            this.proActiveLogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("proActiveLogoPictureBox.Image")));
            this.proActiveLogoPictureBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("proActiveLogoPictureBox.InitialImage")));
            this.proActiveLogoPictureBox.Location = new System.Drawing.Point(12, 12);
            this.proActiveLogoPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.proActiveLogoPictureBox.Name = "proActiveLogoPictureBox";
            this.proActiveLogoPictureBox.Size = new System.Drawing.Size(568, 102);
            this.proActiveLogoPictureBox.TabIndex = 4;
            this.proActiveLogoPictureBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(430, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "ProActiveAgent v1.0.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(430, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "(C) 2008-2009 INRIA";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 800;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // browseConfig
            // 
            this.browseConfig.FileName = "openFileDialog1";
            this.browseConfig.Title = "Choose configuration file";
            // 
            // agentStatusNotifyIcon
            // 
            this.agentStatusNotifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.agentStatusNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("agentStatusNotifyIcon.Icon")));
            this.agentStatusNotifyIcon.Text = "ProActiveAgent";
            this.agentStatusNotifyIcon.Visible = true;
            this.agentStatusNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startServiceToolStripMenuItem,
            this.stopServiceToolStripMenuItem,
            this.startToolStripMenuItem,
            this.closeAdministrationPanelToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(212, 92);
            // 
            // startServiceToolStripMenuItem
            // 
            this.startServiceToolStripMenuItem.Name = "startServiceToolStripMenuItem";
            this.startServiceToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.startServiceToolStripMenuItem.Text = "Start Service";
            this.startServiceToolStripMenuItem.Click += new System.EventHandler(this.startService_Click);
            // 
            // stopServiceToolStripMenuItem
            // 
            this.stopServiceToolStripMenuItem.Name = "stopServiceToolStripMenuItem";
            this.stopServiceToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.stopServiceToolStripMenuItem.Text = "Stop Service";
            this.stopServiceToolStripMenuItem.Click += new System.EventHandler(this.stopService_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.startToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.startToolStripMenuItem.Text = "Automatic launch";
            this.startToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.startToolStripMenuItem_CheckStateChanged);
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // closeAdministrationPanelToolStripMenuItem
            // 
            this.closeAdministrationPanelToolStripMenuItem.Name = "closeAdministrationPanelToolStripMenuItem";
            this.closeAdministrationPanelToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.closeAdministrationPanelToolStripMenuItem.Text = "Close Administration Panel";
            this.closeAdministrationPanelToolStripMenuItem.Click += new System.EventHandler(this.closeAdministrationPanelToolStripMenuItem_Click_1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 289);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(595, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // proActiveInriaLinkLabel
            // 
            this.proActiveInriaLinkLabel.AutoSize = true;
            this.proActiveInriaLinkLabel.BackColor = System.Drawing.Color.White;
            this.proActiveInriaLinkLabel.DisabledLinkColor = System.Drawing.Color.White;
            this.proActiveInriaLinkLabel.Location = new System.Drawing.Point(430, 74);
            this.proActiveInriaLinkLabel.Name = "proActiveInriaLinkLabel";
            this.proActiveInriaLinkLabel.Size = new System.Drawing.Size(118, 13);
            this.proActiveInriaLinkLabel.TabIndex = 9;
            this.proActiveInriaLinkLabel.TabStop = true;
            this.proActiveInriaLinkLabel.Text = "http://proactive.inria.fr/";
            this.proActiveInriaLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.proActiveInriaLinkLabel_LinkClicked);
            // 
            // controlsGroupBox
            // 
            this.controlsGroupBox.Controls.Add(this.startService);
            this.controlsGroupBox.Controls.Add(this.stopService);
            this.controlsGroupBox.Location = new System.Drawing.Point(395, 204);
            this.controlsGroupBox.Name = "controlsGroupBox";
            this.controlsGroupBox.Size = new System.Drawing.Size(185, 81);
            this.controlsGroupBox.TabIndex = 10;
            this.controlsGroupBox.TabStop = false;
            this.controlsGroupBox.Text = "Controls";
            // 
            // logsGroupBox
            // 
            this.logsGroupBox.Controls.Add(this.viewLogsWithExplorerButton);
            this.logsGroupBox.Controls.Add(this.viewLogsWithNotepadButton);
            this.logsGroupBox.Location = new System.Drawing.Point(204, 204);
            this.logsGroupBox.Name = "logsGroupBox";
            this.logsGroupBox.Size = new System.Drawing.Size(185, 81);
            this.logsGroupBox.TabIndex = 11;
            this.logsGroupBox.TabStop = false;
            this.logsGroupBox.Text = "Logs";
            // 
            // viewLogsWithExplorerButton
            // 
            this.viewLogsWithExplorerButton.Location = new System.Drawing.Point(61, 48);
            this.viewLogsWithExplorerButton.Name = "viewLogsWithExplorerButton";
            this.viewLogsWithExplorerButton.Size = new System.Drawing.Size(118, 23);
            this.viewLogsWithExplorerButton.TabIndex = 5;
            this.viewLogsWithExplorerButton.Text = "View with Explorer";
            this.viewLogsWithExplorerButton.UseVisualStyleBackColor = true;
            this.viewLogsWithExplorerButton.Click += new System.EventHandler(this.viewLogsWithIExplorerButton_Click);
            // 
            // ConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 311);
            this.Controls.Add(this.proActiveInriaLinkLabel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.controlsGroupBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.logsGroupBox);
            this.Controls.Add(this.proActiveLogoPictureBox);
            this.Controls.Add(this.infoGroupBox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProActive Agent Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigurationDialog_FormClosing);
            this.Resize += new System.EventHandler(this.ConfigurationDialog_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.infoGroupBox.ResumeLayout(false);
            this.infoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.proActiveLogoPictureBox)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.controlsGroupBox.ResumeLayout(false);
            this.logsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label configurationFileLocationLabel;
        private System.Windows.Forms.TextBox configLocation;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button textEditConfig;
        private System.Windows.Forms.GroupBox infoGroupBox;
        private System.Windows.Forms.Button stopService;
        private System.Windows.Forms.Button startService;
        private System.Windows.Forms.Label agentStatusLabel;
        private System.Windows.Forms.PictureBox proActiveLogoPictureBox;
        //private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        //private System.Windows.Forms.Button troubleshoot;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button viewLogsWithNotepadButton;
        private System.Windows.Forms.OpenFileDialog browseConfig;
        //private System.Windows.Forms.Button scrSvrStart;
        //private System.Windows.Forms.Button globalStop;
        private System.Windows.Forms.NotifyIcon agentStatusNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button guiEditButton;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAdministrationPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServiceToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.LinkLabel proActiveInriaLinkLabel;
        private System.Windows.Forms.GroupBox controlsGroupBox;
        private System.Windows.Forms.GroupBox logsGroupBox;
        private System.Windows.Forms.Label spawnedRuntimesLabel;
        private System.Windows.Forms.Button viewLogsWithExplorerButton;
        private System.Windows.Forms.Label agentStatusValue;
        private System.Windows.Forms.Label spawnedRuntimesValue;
        //private System.Windows.Forms.Button allowForbidRT;
    }
}

