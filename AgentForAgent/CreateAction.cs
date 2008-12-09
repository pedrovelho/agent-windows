using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AgentForAgent
{
    public partial class CreateAction : Form
    {
        ConfigEditor hock;

        public CreateAction(ConfigEditor hock)
        {
            InitializeComponent();
            this.hock = hock;
        }
     
        private void radioRMI_CheckedChanged(object sender, EventArgs e)
        {
            if (radioRMI.Checked == true)
            {
                hock.typeofNewAction = ConfigParser.AdvertAction.DESCRIPTION;
                Dispose();
            }
        }

        private void radioRM_CheckedChanged(object sender, EventArgs e)
        {
            if (radioRM.Checked == true)
            {
                hock.typeofNewAction = ConfigParser.RMAction.DESCRIPTION;
                Dispose();
            }
        }

        private void radioP2P_CheckedChanged(object sender, EventArgs e)
        {
            if (radioP2P.Checked == true)
            {
                hock.typeofNewAction = ConfigParser.P2PAction.DESCRIPTION;
                Dispose();
            }
        }
    }
}
