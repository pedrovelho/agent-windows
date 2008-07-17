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
            this.save = new System.Windows.Forms.Button();
            this.editConfig = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.viewLogs = new System.Windows.Forms.Button();
            this.troubleshoot = new System.Windows.Forms.Button();
            this.stopService = new System.Windows.Forms.Button();
            this.startService = new System.Windows.Forms.Button();
            this.statuslabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.close = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.browseConfig = new System.Windows.Forms.OpenFileDialog();
            this.scrSvrStart = new System.Windows.Forms.Button();
            this.globalStop = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopProActiveRuntimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
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
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(487, 19);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 2;
            this.browse.Text = "Browse...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.configLocation);
            this.groupBox1.Controls.Add(this.save);
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
            this.button1.Location = new System.Drawing.Point(323, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "GUI Edit...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(487, 48);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 2;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // editConfig
            // 
            this.editConfig.Location = new System.Drawing.Point(404, 48);
            this.editConfig.Name = "editConfig";
            this.editConfig.Size = new System.Drawing.Size(75, 23);
            this.editConfig.TabIndex = 2;
            this.editConfig.Text = "Edit...";
            this.editConfig.UseVisualStyleBackColor = true;
            this.editConfig.Click += new System.EventHandler(this.editConfig_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.viewLogs);
            this.groupBox2.Controls.Add(this.troubleshoot);
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
            // troubleshoot
            // 
            this.troubleshoot.Location = new System.Drawing.Point(387, 48);
            this.troubleshoot.Name = "troubleshoot";
            this.troubleshoot.Size = new System.Drawing.Size(92, 23);
            this.troubleshoot.TabIndex = 3;
            this.troubleshoot.Text = "Troubleshoot...";
            this.troubleshoot.UseVisualStyleBackColor = true;
            this.troubleshoot.Click += new System.EventHandler(this.troubleshoot_Click);
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
            // close
            // 
            this.close.Location = new System.Drawing.Point(499, 304);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 5;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(430, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "ProActiveAgent v.0.99";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(444, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "(C) 2008 INRIA";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // browseConfig
            // 
            this.browseConfig.FileName = "openFileDialog1";
            this.browseConfig.Title = "Choose configuration file";
            // 
            // scrSvrStart
            // 
            this.scrSvrStart.Location = new System.Drawing.Point(21, 304);
            this.scrSvrStart.Name = "scrSvrStart";
            this.scrSvrStart.Size = new System.Drawing.Size(110, 23);
            this.scrSvrStart.TabIndex = 7;
            this.scrSvrStart.Text = "Start ScreenSaver";
            this.scrSvrStart.UseVisualStyleBackColor = true;
            this.scrSvrStart.Click += new System.EventHandler(this.scrSvrStart_Click);
            // 
            // globalStop
            // 
            this.globalStop.Location = new System.Drawing.Point(137, 304);
            this.globalStop.Name = "globalStop";
            this.globalStop.Size = new System.Drawing.Size(149, 23);
            this.globalStop.TabIndex = 8;
            this.globalStop.Text = "Stop ProActive Runtime!";
            this.globalStop.UseVisualStyleBackColor = true;
            this.globalStop.Click += new System.EventHandler(this.globalStop_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "ProActiveAgent";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem,
            this.stopProActiveRuntimeToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(224, 70);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // stopProActiveRuntimeToolStripMenuItem
            // 
            this.stopProActiveRuntimeToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.stopProActiveRuntimeToolStripMenuItem.Name = "stopProActiveRuntimeToolStripMenuItem";
            this.stopProActiveRuntimeToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.stopProActiveRuntimeToolStripMenuItem.Text = "Stop ProActive Runtime!";
            this.stopProActiveRuntimeToolStripMenuItem.Click += new System.EventHandler(this.stopProActiveRuntimeToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // ConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 341);
            this.Controls.Add(this.globalStop);
            this.Controls.Add(this.scrSvrStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.close);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ConfigurationDialog";
            this.Text = "ProActiveAgent Configuration";
            this.Resize += new System.EventHandler(this.ConfigurationDialog_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox configLocation;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button editConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button stopService;
        private System.Windows.Forms.Button startService;
        private System.Windows.Forms.Label statuslabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button troubleshoot;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button viewLogs;
        private System.Windows.Forms.OpenFileDialog browseConfig;
        private System.Windows.Forms.Button scrSvrStart;
        private System.Windows.Forms.Button globalStop;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopProActiveRuntimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}

