using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;
using ConfigParser;
using ProActiveAgent;


//test emil
namespace AgentForAgent
{
    public partial class ConfigurationDialog : Form
    {
        private ServiceController sc = new ServiceController("ProActive Agent");
        private static string DEFAULT_CONF_LOCATION = "C:\\PAAgent-config.xml";
        private string agentLocation;

        private ServiceControllerStatus agentStatus;
        private string agentStatusString;

        public ConfigurationDialog()
        {
            InitializeComponent();
            UpdateStatus();
            ReadConfigLocation();
            ReadAgentLocation();
        }

        private void ReadConfigLocation()
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey("Software\\ProActiveAgent");
            if (confKey != null)
            {
                if (confKey.GetValue("ConfigLocation") != null)
                {
                    this.configLocation.Text = (string)confKey.GetValue("ConfigLocation");
                }
                else
                {
                    confKey.SetValue("ConfigLocation", DEFAULT_CONF_LOCATION);
                }
            }
            else
            {
                confKey = Registry.LocalMachine.CreateSubKey("Software\\ProActiveAgent");
                confKey.SetValue("ConfigLocation", DEFAULT_CONF_LOCATION);
            }
        }

        private void ReadAgentLocation()
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey("Software\\ProActiveAgent");
            if (confKey != null)
            {
                if (confKey.GetValue("AgentDirectory") != null)
                {
                    this.agentLocation = (string)confKey.GetValue("AgentDirectory");
                }
                else
                {
                    this.agentLocation = "";
                }
            }
        }

        private void UpdateConfigLocation()
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey("Software\\ProActiveAgent", true);
            if (confKey == null)
                return;
            confKey.SetValue("ConfigLocation", configLocation.Text);
        }

        private void UpdateStatus()
        {
            agentStatus = sc.Status;
            if (agentStatus == ServiceControllerStatus.StopPending)
            {
                agentStatusString = "stopping";
                this.troubleshoot.Enabled = false;
                this.startService.Enabled = false;
                this.stopService.Enabled = false;
                this.globalStop.Enabled = false;
                this.contextMenuStrip1.Items[1].Enabled = false;
            }
            else if (agentStatus == ServiceControllerStatus.Stopped)
            {
                agentStatusString = "stopped";
                this.troubleshoot.Enabled = false;
                this.startService.Enabled = true;
                this.stopService.Enabled = false;
                this.globalStop.Enabled = false;
                this.contextMenuStrip1.Items[1].Enabled = false;
            }
            else if (agentStatus == ServiceControllerStatus.StartPending)
            {
                agentStatusString = "starting";
                this.troubleshoot.Enabled = false;
                this.startService.Enabled = false;
                this.stopService.Enabled = false;
                this.globalStop.Enabled = false;
                this.contextMenuStrip1.Items[1].Enabled = false;
            }
            else if (agentStatus == ServiceControllerStatus.Running)
            {
                agentStatusString = "running";
                this.troubleshoot.Enabled = false;
                this.startService.Enabled = false;
                this.stopService.Enabled = true;
                this.globalStop.Enabled = true;
                this.contextMenuStrip1.Items[1].Enabled = true;
            }
            else
            {
                agentStatusString = "in unknown state.";
                this.troubleshoot.Enabled = true;
                this.startService.Enabled = false;
                this.stopService.Enabled = false;
                this.globalStop.Enabled = false;
            }

            this.statuslabel.Text = "The ProActive Agent is currently " + agentStatusString;
        }

        private void editConfig_Click(object sender, EventArgs e)
        {
            string filename = this.configLocation.Text;
            Process p = new Process();
            p.StartInfo.FileName = "notepad.exe";
            p.StartInfo.Arguments = filename;
            //p.StartInfo.UseShellExecute = false;
            p.Start();
        }

        private void troubleshoot_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "services.msc";
            p.StartInfo.Arguments = "";
            //p.StartInfo.UseShellExecute = false;
            p.Start();
        }

        private void startService_Click(object sender, EventArgs e)
        {
            startService.Enabled = false;
            stopService.Enabled = false;
            troubleshoot.Enabled = true;
            sc.Start();
        }

        private void stopService_Click(object sender, EventArgs e)
        {
            startService.Enabled = false;
            stopService.Enabled = false;
            troubleshoot.Enabled = true;
            sc.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sc.Refresh();
            UpdateStatus();
        }

        private void viewLogs_Click(object sender, EventArgs e)
        {
/*            Configuration conf = ConfigurationParser.parseXml(configLocation.Text);
            if (conf == null || conf.agentConfig == null)
            {
                MessageBox.Show("The configuration file is broken.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } */

             Process p = new Process();
            p.StartInfo.FileName = "notepad.exe";
            p.StartInfo.Arguments = this.agentLocation + "\\ProActiveAgent-log.txt";
            p.Start();
        }

        private void save_Click(object sender, EventArgs e)
        {
            UpdateConfigLocation();
        }

        private void browse_Click(object sender, EventArgs e)
        {
            browseConfig.FileName = configLocation.Text;
            browseConfig.ShowDialog();
            configLocation.Text = browseConfig.FileName;
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void scrSvrStart_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "D:\\tdobek\\Mes documents\\Screen Saver\\ScreenSaver\\bin\\Release\\ProActiveSSaver.exe";
            p.Start();
        }

        private void globalStop_Click(object sender, EventArgs e)
        {
            sc.ExecuteCommand((int)PAACommands.GlobalStop);
        }

        private void ConfigurationDialog_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void stopProActiveRuntimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sc.ExecuteCommand((int)PAACommands.GlobalStop);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigEditor window = new ConfigEditor(configLocation.Text);
            window.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }



    }
}
