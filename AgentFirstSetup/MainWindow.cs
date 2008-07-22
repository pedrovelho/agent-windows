using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConfigParser;
using Microsoft.Win32;

namespace AgentFirstSetup
{
    public partial class MainWindow : Form
    {
        private Configuration conf;
        private string configLocation;
        private string agentDir;

        public MainWindow()
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey("Software\\ProActiveAgent");
            this.configLocation = "";
            this.agentDir = "";
            if (confKey != null)
            {
                if (confKey.GetValue("ConfigLocation") != null)
                {
                    configLocation = (string)confKey.GetValue("ConfigLocation");
                }
                else
                {
                    MessageBox.Show("The ProActiveAgent is not installed properly. Please restart the installation process.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                if (confKey.GetValue("AgentDirectory") != null)
                {
                    agentDir = (string)confKey.GetValue("AgentDirectory");
                }
                else
                {
                    MessageBox.Show("The ProActiveAgent is not installed properly. Please restart the installation process.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

            }
            else
            {
                MessageBox.Show("The ProActiveAgent is not installed properly. Please restart the installation process.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            try
            {
                this.conf = ConfigurationParser.parseXml(configLocation, agentDir);
            }
            catch (IncorrectConfigurationException)
            {
                MessageBox.Show("The ProActiveAgent is not installed properly. Please restart the installation process.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            InitializeComponent();
            
        }

        private void proactiveLocationButton_Click(object sender, EventArgs e)
        {
            proActiveLocationBrowser.SelectedPath = proactiveLocation.Text;
            proActiveLocationBrowser.ShowDialog();
            proactiveLocation.Text = proActiveLocationBrowser.SelectedPath;
        }

        private void jvmLocationButton_Click(object sender, EventArgs e)
        {
            jvmLocationBrowser.ShowDialog();
            jvmDirectory.Text = jvmLocationBrowser.SelectedPath;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                jvmDirectory.Enabled = false;
                jvmLocationButton.Enabled = false;
                conf.agentConfig.javaHome = "";
            }
            else
            {
                jvmDirectory.Enabled = true;
                jvmLocationButton.Enabled = true;
            }
        }

        private void proactiveLocation_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.proactiveLocation = proactiveLocation.Text;
        }

        private void jvmDirectory_TextChanged(object sender, EventArgs e)
        {
            conf.agentConfig.javaHome = jvmDirectory.Text;
        }

        private void saveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigurationParser.saveXml(this.configLocation, conf);
            }
            catch (Exception)
            {
                MessageBox.Show("");
            }
            Close();
        }

        private void closeConfig_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The ProActiveAgent is not configured properly. Thus, it may not work. You can configure it later using AgentControl application.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
        }



    }
}
