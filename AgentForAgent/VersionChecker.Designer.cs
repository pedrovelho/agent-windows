namespace AgentForAgent
{
    partial class VersionChecker
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelCurrentVersion = new System.Windows.Forms.Label();
            this.currentVersion = new System.Windows.Forms.Label();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.textBoxFeatures = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.resultLabel = new System.Windows.Forms.Label();
            this.latestVersion = new System.Windows.Forms.Label();
            this.labelFeatures = new System.Windows.Forms.Label();
            this.labelLatestVersion = new System.Windows.Forms.Label();
            this.panelWait = new System.Windows.Forms.Panel();
            this.labelPleaseWait = new System.Windows.Forms.Label();
            this.panelInfo.SuspendLayout();
            this.panelWait.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelCurrentVersion
            // 
            this.labelCurrentVersion.AutoSize = true;
            this.labelCurrentVersion.Location = new System.Drawing.Point(12, 9);
            this.labelCurrentVersion.Name = "labelCurrentVersion";
            this.labelCurrentVersion.Size = new System.Drawing.Size(105, 13);
            this.labelCurrentVersion.TabIndex = 0;
            this.labelCurrentVersion.Text = "Your current version:";
            // 
            // currentVersion
            // 
            this.currentVersion.AutoSize = true;
            this.currentVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentVersion.Location = new System.Drawing.Point(123, 9);
            this.currentVersion.Name = "currentVersion";
            this.currentVersion.Size = new System.Drawing.Size(0, 13);
            this.currentVersion.TabIndex = 0;
            // 
            // panelInfo
            // 
            this.panelInfo.Controls.Add(this.textBoxFeatures);
            this.panelInfo.Controls.Add(this.button1);
            this.panelInfo.Controls.Add(this.resultLabel);
            this.panelInfo.Controls.Add(this.latestVersion);
            this.panelInfo.Controls.Add(this.labelFeatures);
            this.panelInfo.Controls.Add(this.labelLatestVersion);
            this.panelInfo.Location = new System.Drawing.Point(8, 26);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(389, 173);
            this.panelInfo.TabIndex = 1;
            // 
            // textBoxFeatures
            // 
            this.textBoxFeatures.AcceptsReturn = true;
            this.textBoxFeatures.Location = new System.Drawing.Point(114, 54);
            this.textBoxFeatures.Multiline = true;
            this.textBoxFeatures.Name = "textBoxFeatures";
            this.textBoxFeatures.Size = new System.Drawing.Size(264, 84);
            this.textBoxFeatures.TabIndex = 7;
            this.textBoxFeatures.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(109, 147);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Download the latest version";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultLabel.ForeColor = System.Drawing.Color.Red;
            this.resultLabel.Location = new System.Drawing.Point(110, 39);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(0, 15);
            this.resultLabel.TabIndex = 5;
            // 
            // latestVersion
            // 
            this.latestVersion.AutoSize = true;
            this.latestVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.latestVersion.Location = new System.Drawing.Point(114, 1);
            this.latestVersion.Name = "latestVersion";
            this.latestVersion.Size = new System.Drawing.Size(0, 13);
            this.latestVersion.TabIndex = 3;
            // 
            // labelFeatures
            // 
            this.labelFeatures.AutoSize = true;
            this.labelFeatures.Location = new System.Drawing.Point(3, 55);
            this.labelFeatures.Name = "labelFeatures";
            this.labelFeatures.Size = new System.Drawing.Size(51, 13);
            this.labelFeatures.TabIndex = 4;
            this.labelFeatures.Text = "Features:";
            // 
            // labelLatestVersion
            // 
            this.labelLatestVersion.AutoSize = true;
            this.labelLatestVersion.Location = new System.Drawing.Point(3, 1);
            this.labelLatestVersion.Name = "labelLatestVersion";
            this.labelLatestVersion.Size = new System.Drawing.Size(76, 13);
            this.labelLatestVersion.TabIndex = 4;
            this.labelLatestVersion.Text = "Latest version:";
            // 
            // panelWait
            // 
            this.panelWait.Controls.Add(this.labelPleaseWait);
            this.panelWait.Location = new System.Drawing.Point(8, 25);
            this.panelWait.Name = "panelWait";
            this.panelWait.Size = new System.Drawing.Size(389, 174);
            this.panelWait.TabIndex = 7;
            // 
            // labelPleaseWait
            // 
            this.labelPleaseWait.AutoSize = true;
            this.labelPleaseWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPleaseWait.Location = new System.Drawing.Point(141, 65);
            this.labelPleaseWait.Name = "labelPleaseWait";
            this.labelPleaseWait.Size = new System.Drawing.Size(100, 16);
            this.labelPleaseWait.TabIndex = 0;
            this.labelPleaseWait.Text = "Please wait...";
            // 
            // VersionChecker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 205);
            this.Controls.Add(this.panelWait);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.currentVersion);
            this.Controls.Add(this.labelCurrentVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VersionChecker";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VersionChecker";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VersionChecker_FormClosed);
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.panelWait.ResumeLayout(false);
            this.panelWait.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCurrentVersion;
        private System.Windows.Forms.Label currentVersion;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.Label latestVersion;
        private System.Windows.Forms.Label labelLatestVersion;
        private System.Windows.Forms.Panel panelWait;
        private System.Windows.Forms.Label labelPleaseWait;
        private System.Windows.Forms.Label labelFeatures;
        private System.Windows.Forms.TextBox textBoxFeatures;
    }
}