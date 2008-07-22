namespace AgentFirstSetup
{
    partial class MainWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.proactiveLocation = new System.Windows.Forms.TextBox();
            this.proactiveLocationButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.jvmLocationButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.jvmDirectory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.closeConfig = new System.Windows.Forms.Button();
            this.saveConfig = new System.Windows.Forms.Button();
            this.proActiveLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.jvmLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "ProActive Location:";
            // 
            // proactiveLocation
            // 
            this.proactiveLocation.Location = new System.Drawing.Point(115, 39);
            this.proactiveLocation.Name = "proactiveLocation";
            this.proactiveLocation.Size = new System.Drawing.Size(368, 20);
            this.proactiveLocation.TabIndex = 5;
            this.proactiveLocation.TextChanged += new System.EventHandler(this.proactiveLocation_TextChanged);
            // 
            // proactiveLocationButton
            // 
            this.proactiveLocationButton.Location = new System.Drawing.Point(489, 37);
            this.proactiveLocationButton.Name = "proactiveLocationButton";
            this.proactiveLocationButton.Size = new System.Drawing.Size(75, 23);
            this.proactiveLocationButton.TabIndex = 8;
            this.proactiveLocationButton.Text = "Browse...";
            this.proactiveLocationButton.UseVisualStyleBackColor = true;
            this.proactiveLocationButton.Click += new System.EventHandler(this.proactiveLocationButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(115, 91);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(169, 17);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "Use system-wide JVM location";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // jvmLocationButton
            // 
            this.jvmLocationButton.Location = new System.Drawing.Point(489, 63);
            this.jvmLocationButton.Name = "jvmLocationButton";
            this.jvmLocationButton.Size = new System.Drawing.Size(75, 23);
            this.jvmLocationButton.TabIndex = 12;
            this.jvmLocationButton.Text = "Browse...";
            this.jvmLocationButton.UseVisualStyleBackColor = true;
            this.jvmLocationButton.Click += new System.EventHandler(this.jvmLocationButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "JVM Directory:";
            // 
            // jvmDirectory
            // 
            this.jvmDirectory.Location = new System.Drawing.Point(115, 65);
            this.jvmDirectory.Name = "jvmDirectory";
            this.jvmDirectory.Size = new System.Drawing.Size(368, 20);
            this.jvmDirectory.TabIndex = 10;
            this.jvmDirectory.TextChanged += new System.EventHandler(this.jvmDirectory_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(379, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Please specify the following settings in order to be able to use ProActive Agent:" +
                "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(359, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "NOTE: You can change these settings later using AgentControl application";
            // 
            // closeConfig
            // 
            this.closeConfig.Location = new System.Drawing.Point(489, 118);
            this.closeConfig.Name = "closeConfig";
            this.closeConfig.Size = new System.Drawing.Size(75, 23);
            this.closeConfig.TabIndex = 17;
            this.closeConfig.Text = "Cancel";
            this.closeConfig.UseVisualStyleBackColor = true;
            this.closeConfig.Click += new System.EventHandler(this.closeConfig_Click);
            // 
            // saveConfig
            // 
            this.saveConfig.Location = new System.Drawing.Point(408, 118);
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.Size = new System.Drawing.Size(75, 23);
            this.saveConfig.TabIndex = 16;
            this.saveConfig.Text = "Save";
            this.saveConfig.UseVisualStyleBackColor = true;
            this.saveConfig.Click += new System.EventHandler(this.saveConfig_Click);
            // 
            // proActiveLocationBrowser
            // 
            this.proActiveLocationBrowser.Description = "Choose location for ProActive";
            // 
            // jvmLocationBrowser
            // 
            this.jvmLocationBrowser.Description = "Choose location for JVM";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 185);
            this.Controls.Add(this.closeConfig);
            this.Controls.Add(this.saveConfig);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.jvmLocationButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.jvmDirectory);
            this.Controls.Add(this.proactiveLocationButton);
            this.Controls.Add(this.proactiveLocation);
            this.Controls.Add(this.label1);
            this.Name = "MainWindow";
            this.Text = "ProActiveAgent Essential Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox proactiveLocation;
        private System.Windows.Forms.Button proactiveLocationButton;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button jvmLocationButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox jvmDirectory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button closeConfig;
        private System.Windows.Forms.Button saveConfig;
        private System.Windows.Forms.FolderBrowserDialog proActiveLocationBrowser;
        private System.Windows.Forms.FolderBrowserDialog jvmLocationBrowser;

    }
}

