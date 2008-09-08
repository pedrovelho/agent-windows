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

        private bool isRuntimeStarted = false;
        private bool setVisibleCore = false;
        //private bool allowRuntime = true;

        private static string StopPending = "StopPending";
        private static string Stopped = "Stopped";
        private static string StartPending = "StartPending";
        private static string Running = "Running";
        private static string Unknown = "unknown";

        public ConfigurationDialog()
        {
            InitializeComponent();
            this.Hide();
            UpdateStatus();
            ReadConfigLocation();
            ReadAgentLocation();

            //Retrieve register value
            RegistryKey confKey = Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
            if (confKey != null)
            {
                if (confKey.GetValue("ProActive Agent Interface") != null)
                {
                    this.startToolStripMenuItem.CheckState = CheckState.Checked;
                }
            }
            confKey.Close();

            //--Read the register and update button
            /*confKey = Registry.LocalMachine.OpenSubKey("Software\\ProActiveAgent");
            if (confKey != null)
            {
                if (confKey.GetValue("AllowRuntime") != null)
                {
                    allowRuntime = (bool)TypeDescriptor.GetConverter(allowRuntime).ConvertFrom(confKey.GetValue("AllowRuntime"));
                }
                else
                {
                    allowRuntime = false;
                }
                if (allowRuntime)
                {
                    allowForbidRT.Text = "Forbid RT";
                }
                else
                {
                    allowForbidRT.Text = "Allow RT";
                }
            }*/
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
            confKey.Close();
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
            confKey.Close();
        }

        private void UpdateConfigLocation()
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey("Software\\ProActiveAgent", true);
            if (confKey == null)
                return;
            confKey.SetValue("ConfigLocation", configLocation.Text);
            confKey.Close();
        }

        private void UpdateStatus()
        {
            agentStatus = sc.Status;
            if (agentStatus == ServiceControllerStatus.StopPending)
            {
                agentStatusString = "stopping";
                //this.troubleshoot.Enabled = false;
                this.startService.Enabled = false;
                this.stopService.Enabled = false;
                this.startServiceToolStripMenuItem.Enabled = false;
                this.stopServiceToolStripMenuItem.Enabled = false;

                this.notifyIcon1.Text = StopPending;
                //this.globalStop.Enabled = false;
                //this.contextMenuStrip1.Items[1].Enabled = false;
                //this.allowForbidRT.Enabled = false;
            }
            else if (agentStatus == ServiceControllerStatus.Stopped)
            {
                agentStatusString = "stopped";
                //this.troubleshoot.Enabled = false;
                this.startService.Enabled = true;
                this.stopService.Enabled = false;
                this.startServiceToolStripMenuItem.Enabled = true;
                this.stopServiceToolStripMenuItem.Enabled = false;

                this.notifyIcon1.Text = Stopped;
                //this.globalStop.Enabled = false;
                //this.contextMenuStrip1.Items[1].Enabled = false;
                //this.allowForbidRT.Enabled = false;
            }
            else if (agentStatus == ServiceControllerStatus.StartPending)
            {
                agentStatusString = "starting";
                //this.troubleshoot.Enabled = false;
                this.startService.Enabled = false;
                this.stopService.Enabled = false;
                this.startServiceToolStripMenuItem.Enabled = false;
                this.stopServiceToolStripMenuItem.Enabled = false;

                this.notifyIcon1.Text = StartPending;
                //this.globalStop.Enabled = false;
                //this.contextMenuStrip1.Items[1].Enabled = false;
                //this.allowForbidRT.Enabled = false;
            }
            else if (agentStatus == ServiceControllerStatus.Running)
            {
                agentStatusString = "running";
                //this.troubleshoot.Enabled = false;
                this.startService.Enabled = false;
                this.stopService.Enabled = true;
                this.startServiceToolStripMenuItem.Enabled = false;
                this.stopServiceToolStripMenuItem.Enabled = true;

                this.notifyIcon1.Text = Running;
                //this.globalStop.Enabled = true;
                //this.contextMenuStrip1.Items[1].Enabled = true;
                //this.allowForbidRT.Enabled = true;
            }
            else
            {
                agentStatusString = "in unknown state.";
                //this.troubleshoot.Enabled = true;
                this.startService.Enabled = false;
                this.stopService.Enabled = false;
                this.startServiceToolStripMenuItem.Enabled = false;
                this.stopServiceToolStripMenuItem.Enabled = false;

                this.notifyIcon1.Text = Unknown;
                //this.globalStop.Enabled = false;
                //this.allowForbidRT.Enabled = false;
            }

            this.statuslabel.Text = "The ProActive Agent is currently " + agentStatusString;

            if (agentStatus != ServiceControllerStatus.Running)
            {
                //-- stopped icon
                notifyIcon1.Icon = (Icon)Resource1.icon_stop;
                Icon = (Icon)Resource1.icon_stop;
            }
            else
            {
                //--Read the register and update the trayIcon
                RegistryKey confKey = Registry.LocalMachine.OpenSubKey("Software\\ProActiveAgent");
                if (confKey != null)
                {
                    if (confKey.GetValue("IsRuntimeStarted") != null)
                    {
                        isRuntimeStarted = (bool)TypeDescriptor.GetConverter(isRuntimeStarted).ConvertFrom(confKey.GetValue("IsRuntimeStarted"));
                    }
                    else
                    {
                        isRuntimeStarted = false;
                    }
                }

                try
                {
                    if (isRuntimeStarted)
                    {
                        notifyIcon1.Icon = (Icon)Resource1.icon_active;
                        Icon = (Icon)Resource1.icon_active;
                    }
                    else
                    {
                        notifyIcon1.Icon = (Icon)Resource1.icon_passive;
                        Icon = (Icon)Resource1.icon_passive; ;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                confKey.Close();
            }
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
            try
            {
                Configuration conf = ConfigurationParser.parseXml(configLocation.Text, agentLocation);
                startService.Enabled = false;
                stopService.Enabled = false;
                //troubleshoot.Enabled = true;
                startServiceToolStripMenuItem.Enabled = false;
                stopServiceToolStripMenuItem.Enabled = false;
                sc.Start();
            }
            catch (IncorrectConfigurationException)
            {
                DialogResult res = MessageBox.Show("The configuration file is broken.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void stopService_Click(object sender, EventArgs e)
        {
            startService.Enabled = false;
            stopService.Enabled = false;
            //troubleshoot.Enabled = true;
            startServiceToolStripMenuItem.Enabled = false;
            stopServiceToolStripMenuItem.Enabled = false;
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

        private void browse_Click(object sender, EventArgs e)
        {
            browseConfig.FileName = configLocation.Text;
            browseConfig.ShowDialog();
            configLocation.Text = browseConfig.FileName;

            //validate the config file 
            try
            {
                bool valid = ConfigurationParser.validateXMLFile(configLocation.Text, agentLocation);
                if (!valid)
                {
                    MessageBox.Show("The selected file does not comply with the schema. The Agent will probably not work. You can edit the file in the text editor or the graphical one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (IncorrectConfigurationException)
            {
                MessageBox.Show("The selected file is not a correct xml file. The Agent will probably not work. You can edit the file in the text editor or the graphical one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }

        private void close_Click(object sender, EventArgs e)
        {
            setVisibleCore = false;
            Hide();
        }

        private void scrSvrStart_Click(object sender, EventArgs e)
        {
            Process p = new Process();

            p.StartInfo.FileName = Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\ProActiveSSaver.scr";
            try
            {
                p.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("The ProActive Screen Saver could not be started", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            setVisibleCore = true;
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setVisibleCore = true;
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void stopProActiveRuntimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sc.ExecuteCommand((int)PAACommands.GlobalStop);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*DialogResult res = MessageBox.Show("This operation doesn't change the state of ProActive service", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK)
            {
                this.Close();
                Application.ExitThread();
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Configuration conf = ConfigurationParser.parseXml(configLocation.Text, agentLocation);
                ConfigEditor window = new ConfigEditor(conf, configLocation.Text, agentLocation);
                window.Show();
            }
            catch (IncorrectConfigurationException)
            {
                //MessageBox.Show("The configuration file is broken.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult res = MessageBox.Show("The configuration file is broken.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //TODO : proposer de créer un fichier âr default
                /*if (res == DialogResult.Yes)
                {

                }*/
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void ConfigurationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            setVisibleCore = false;
            Hide();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
                ((ToolStripMenuItem)sender).CheckState = CheckState.Unchecked;
            else
                ((ToolStripMenuItem)sender).CheckState = CheckState.Checked;
        }

        private void startToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
            {
                //--Register the automatic launch
                RegistryKey confKey = Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (confKey != null)
                {
                    confKey.SetValue("ProActive Agent Interface", System.Environment.CurrentDirectory + "\\AgentForAgent.exe");
                }
                confKey.Close();
            }
            else
            {
                //--Remove the automatic launch key
                RegistryKey confKey = Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (confKey != null)
                {
                    if (confKey.GetValue("ProActive Agent Interface") != null)
                    {
                        confKey.DeleteValue("ProActive Agent Interface");
                    }
                }
                confKey.Close();
            }
        }
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(setVisibleCore);
        }

        /*private void allowForbidRT_Click(object sender, EventArgs e)
        {
            //change state
            allowRuntime = !allowRuntime;

            if (allowRuntime)
            {
                sc.ExecuteCommand((int)PAACommands.AllowRuntime);
                allowForbidRT.Text = "Forbid RT";
            }
            else
            {
                sc.ExecuteCommand((int)PAACommands.ForbidRuntime);
                allowForbidRT.Text = "Allow RT";
            }
        }*/

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            /*Form infoBox = new Form();

            infoBox.FormBorderStyle = FormBorderStyle.None;
            infoBox.ShowInTaskbar = false;
            infoBox.ControlBox = false;
            infoBox.StartPosition = FormStartPosition.Manual;
            infoBox.Width = 200;
            infoBox.Height = 100;
            infoBox.Top = Screen.PrimaryScreen.Bounds.Bottom - infoBox.Height - 35;
            infoBox.Left = Screen.PrimaryScreen.Bounds.Right - infoBox.Width - 10;
            infoBox.AllowTransparency = true;
            infoBox.Opacity = 0;

            RichTextBox textBox = new RichTextBox();
            textBox.Multiline = true;
            textBox.BackColor = Color.LightCyan;
            textBox.ForeColor = Color.DarkBlue;
            textBox.Font = new Font("Tahoma", 8, FontStyle.Bold | FontStyle.Underline);
            textBox.Text = "Message pour vous";
            textBox.Dock = DockStyle.Fill;
            textBox.ReadOnly = true;

            infoBox.Controls.Add(textBox);
            infoBox.Show();
            infoBox.Refresh();
            for (double opacity = 0.1; opacity < 0.80; opacity+=0.1)
            {
                infoBox.Opacity = opacity;
                Thread.Sleep(100);
            }*/
        }



        private void closeAdministrationPanelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("This operation doesn't change the state of ProActive service", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK)
            {
                this.notifyIcon1.Dispose();
                this.Close();
                Application.ExitThread();
            }
        }

        private void configLocation_TextChanged(object sender, EventArgs e)
        {
            UpdateConfigLocation();
        }
    }
}
