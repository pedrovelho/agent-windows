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
            this.proActiveLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.jvmLocationBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panelAccount = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.domain = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.user = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.continueButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelAccount.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "ProActive or";
            // 
            // proactiveLocation
            // 
            this.proactiveLocation.Location = new System.Drawing.Point(169, 25);
            this.proactiveLocation.Name = "proactiveLocation";
            this.proactiveLocation.Size = new System.Drawing.Size(309, 20);
            this.proactiveLocation.TabIndex = 5;
            this.proactiveLocation.TextChanged += new System.EventHandler(this.proactiveLocation_TextChanged);
            // 
            // proactiveLocationButton
            // 
            this.proactiveLocationButton.Location = new System.Drawing.Point(484, 23);
            this.proactiveLocationButton.Name = "proactiveLocationButton";
            this.proactiveLocationButton.Size = new System.Drawing.Size(84, 23);
            this.proactiveLocationButton.TabIndex = 8;
            this.proactiveLocationButton.Text = "Browse...";
            this.proactiveLocationButton.UseVisualStyleBackColor = true;
            this.proactiveLocationButton.Click += new System.EventHandler(this.proactiveLocationButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(89, 91);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(169, 17);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "Use system-wide JVM location";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // jvmLocationButton
            // 
            this.jvmLocationButton.Location = new System.Drawing.Point(484, 63);
            this.jvmLocationButton.Name = "jvmLocationButton";
            this.jvmLocationButton.Size = new System.Drawing.Size(84, 23);
            this.jvmLocationButton.TabIndex = 12;
            this.jvmLocationButton.Text = "Browse...";
            this.jvmLocationButton.UseVisualStyleBackColor = true;
            this.jvmLocationButton.Click += new System.EventHandler(this.jvmLocationButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "JVM Directory:";
            // 
            // jvmDirectory
            // 
            this.jvmDirectory.Location = new System.Drawing.Point(88, 65);
            this.jvmDirectory.Name = "jvmDirectory";
            this.jvmDirectory.Size = new System.Drawing.Size(390, 20);
            this.jvmDirectory.TabIndex = 10;
            this.jvmDirectory.TextChanged += new System.EventHandler(this.jvmDirectory_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(203, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(376, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Note that these settings can be changed through the ProActive Agent Control.";
            // 
            // proActiveLocationBrowser
            // 
            this.proActiveLocationBrowser.Description = "Choose location for ProActive";
            // 
            // jvmLocationBrowser
            // 
            this.jvmLocationBrowser.Description = "Choose location for JVM";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.jvmLocationButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.jvmDirectory);
            this.groupBox1.Controls.Add(this.proactiveLocationButton);
            this.groupBox1.Controls.Add(this.proactiveLocation);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(576, 114);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelAccount);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(3, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(576, 123);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Service Account";
            // 
            // panelAccount
            // 
            this.panelAccount.Controls.Add(this.label5);
            this.panelAccount.Controls.Add(this.label7);
            this.panelAccount.Controls.Add(this.domain);
            this.panelAccount.Controls.Add(this.label6);
            this.panelAccount.Controls.Add(this.user);
            this.panelAccount.Controls.Add(this.password);
            this.panelAccount.Enabled = false;
            this.panelAccount.Location = new System.Drawing.Point(25, 71);
            this.panelAccount.Name = "panelAccount";
            this.panelAccount.Size = new System.Drawing.Size(534, 33);
            this.panelAccount.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Domain";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(371, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Password";
            // 
            // domain
            // 
            this.domain.Location = new System.Drawing.Point(64, 7);
            this.domain.Name = "domain";
            this.domain.Size = new System.Drawing.Size(100, 20);
            this.domain.TabIndex = 1;
            this.domain.Text = ".";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(201, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "User";
            // 
            // user
            // 
            this.user.Location = new System.Drawing.Point(236, 7);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(100, 20);
            this.user.TabIndex = 1;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(430, 7);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(100, 20);
            this.password.TabIndex = 1;
            this.password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.password_KeyDown);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(34, 48);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(134, 17);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.Text = "Install as User Account";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AccessibleName = "";
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(34, 25);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(172, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Install as LocalSystem Account";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(496, 274);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 17;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "ProActive Scheduling Directory:";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 306);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProActiveAgent Essential Configuration";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelAccount.ResumeLayout(false);
            this.panelAccount.PerformLayout();
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
        private System.Windows.Forms.FolderBrowserDialog proActiveLocationBrowser;
        private System.Windows.Forms.FolderBrowserDialog jvmLocationBrowser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox user;
        private System.Windows.Forms.TextBox domain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelAccount;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Label label4;

    }
}

