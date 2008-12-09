namespace AgentForAgent
{
    partial class CreateAction
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
            this.label1 = new System.Windows.Forms.Label();
            this.radioRMI = new System.Windows.Forms.RadioButton();
            this.radioRM = new System.Windows.Forms.RadioButton();
            this.radioP2P = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please select an action type:";
            // 
            // radioRMI
            // 
            this.radioRMI.AutoSize = true;
            this.radioRMI.Location = new System.Drawing.Point(15, 34);
            this.radioRMI.Name = "radioRMI";
            this.radioRMI.Size = new System.Drawing.Size(104, 17);
            this.radioRMI.TabIndex = 1;
            this.radioRMI.Text = "RMI Registration";
            this.radioRMI.UseVisualStyleBackColor = true;
            this.radioRMI.CheckedChanged += new System.EventHandler(this.radioRMI_CheckedChanged);
            // 
            // radioRM
            // 
            this.radioRM.AutoSize = true;
            this.radioRM.Location = new System.Drawing.Point(125, 34);
            this.radioRM.Name = "radioRM";
            this.radioRM.Size = new System.Drawing.Size(175, 17);
            this.radioRM.TabIndex = 1;
            this.radioRM.Text = "Resource Manager Registration";
            this.radioRM.UseVisualStyleBackColor = true;
            this.radioRM.CheckedChanged += new System.EventHandler(this.radioRM_CheckedChanged);
            // 
            // radioP2P
            // 
            this.radioP2P.AutoSize = true;
            this.radioP2P.Location = new System.Drawing.Point(306, 34);
            this.radioP2P.Name = "radioP2P";
            this.radioP2P.Size = new System.Drawing.Size(88, 17);
            this.radioP2P.TabIndex = 1;
            this.radioP2P.Text = "Peer-To-Peer";
            this.radioP2P.UseVisualStyleBackColor = true;
            this.radioP2P.CheckedChanged += new System.EventHandler(this.radioP2P_CheckedChanged);
            // 
            // CreateAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 67);
            this.Controls.Add(this.radioP2P);
            this.Controls.Add(this.radioRM);
            this.Controls.Add(this.radioRMI);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateAction";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CreateAction";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioRMI;
        private System.Windows.Forms.RadioButton radioRM;
        private System.Windows.Forms.RadioButton radioP2P;
    }
}