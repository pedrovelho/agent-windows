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
            this.label1 = new System.Windows.Forms.Label();
            this.configLocation = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.editConfig = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.viewLogs = new System.Windows.Forms.Button();
            this.stopService = new System.Windows.Forms.Button();
            this.startService = new System.Windows.Forms.Button();
            this.statuslabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.browseConfig = new System.Windows.Forms.OpenFileDialog();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAdministrationPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Configuration file location:";
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
            this.browse.Location = new System.Drawing.Point(487, 19);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 2;
            this.browse.Text = "Load...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.configLocation);
            this.groupBox1.Controls.Add(this.editConfig);
            this.groupBox1.Controls.Add(this.browse);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 81);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(404, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "GUI Edit...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // editConfig
            // 
            this.editConfig.Location = new System.Drawing.Point(487, 48);
            this.editConfig.Name = "editConfig";
            this.editConfig.Size = new System.Drawing.Size(75, 23);
            this.editConfig.TabIndex = 2;
            this.editConfig.Text = "Text Edit...";
            this.editConfig.UseVisualStyleBackColor = true;
            this.editConfig.Click += new System.EventHandler(this.editConfig_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.viewLogs);
            this.groupBox2.Controls.Add(this.stopService);
            this.groupBox2.Controls.Add(this.startService);
            this.groupBox2.Controls.Add(this.statuslabel);
            this.groupBox2.Location = new System.Drawing.Point(12, 204);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(568, 81);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // viewLogs
            // 
            this.viewLogs.Location = new System.Drawing.Point(9, 48);
            this.viewLogs.Name = "viewLogs";
            this.viewLogs.Size = new System.Drawing.Size(89, 23);
            this.viewLogs.TabIndex = 4;
            this.viewLogs.Text = "View logs...";
            this.viewLogs.UseVisualStyleBackColor = true;
            this.viewLogs.Click += new System.EventHandler(this.viewLogs_Click);
            // 
            // stopService
            // 
            this.stopService.Location = new System.Drawing.Point(487, 48);
            this.stopService.Name = "stopService";
            this.stopService.Size = new System.Drawing.Size(75, 23);
            this.stopService.TabIndex = 2;
            this.stopService.Text = "Stop";
            this.stopService.UseVisualStyleBackColor = true;
            this.stopService.Click += new System.EventHandler(this.stopService_Click);
            // 
            // startService
            // 
            this.startService.Location = new System.Drawing.Point(487, 19);
            this.startService.Name = "startService";
            this.startService.Size = new System.Drawing.Size(75, 23);
            this.startService.TabIndex = 2;
            this.startService.Text = "Start";
            this.startService.UseVisualStyleBackColor = true;
            this.startService.Click += new System.EventHandler(this.startService_Click);
            // 
            // statuslabel
            // 
            this.statuslabel.AutoSize = true;
            this.statuslabel.Location = new System.Drawing.Point(6, 21);
            this.statuslabel.Name = "statuslabel";
            this.statuslabel.Size = new System.Drawing.Size(173, 13);
            this.statuslabel.TabIndex = 0;
            this.statuslabel.Text = "ProActiveAgent Service is currently";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(568, 102);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(430, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "ProActiveAgent v.0.9";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(444, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "(C) 2008 INRIA";
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
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "ProActiveAgent";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
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
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 289);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(595, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.IsLink = true;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(90, 17);
            this.toolStripStatusLabel1.Text = "Check for update";
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // ConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 311);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProActiveAgent Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigurationDialog_FormClosing);
            this.Resize += new System.EventHandler(this.ConfigurationDialog_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox configLocation;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button editConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button stopService;
        private System.Windows.Forms.Button startService;
        private System.Windows.Forms.Label statuslabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        //private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        //private System.Windows.Forms.Button troubleshoot;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button viewLogs;
        private System.Windows.Forms.OpenFileDialog browseConfig;
        //private System.Windows.Forms.Button scrSvrStart;
        //private System.Windows.Forms.Button globalStop;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAdministrationPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServiceToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        //private System.Windows.Forms.Button allowForbidRT;
    }
}

