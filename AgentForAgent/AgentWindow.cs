using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;
using ConfigParser;
using Microsoft.Win32;
using ProActiveAgent;

namespace AgentForAgent
{
    public partial class ConfigurationDialog : Form
    {
        // The configuration location registery key name
        public const string CONFIG_LOCATION_KEY = "ConfigLocation";
        public const string PROACTIVE_AGENT_SUB_KEY = "Software\\ProActiveAgent";

        // The following constants are used by the gui
        public const string STOP_PENDING = "StopPending";
        public const string STOPPED = "Stopped";
        public const string START_PENDING = "StartPending";
        public const string RUNNING = "Running";
        public const string UNKNOWN = "unknown";
        // The default location of the ProActive Agent configuration file
        public const string DEFAULT_CONFIG_LOCATION = "C:\\PAAgent-config.xml";

        private ServiceController sc = new ServiceController(Constants.PROACTIVE_AGENT_SERVICE_NAME);
        private ServiceControllerStatus agentStatus;

        private string agentLocation;
        private string agentStatusString;

        private bool isRuntimeStarted = false;
        private bool setVisibleCore = false;
        //private bool allowRuntime = true;

        private ConfigurationEditor window;

        public ConfigurationDialog()
        {
            // Init all visuals
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
        }

        private void ReadConfigLocation()
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(PROACTIVE_AGENT_SUB_KEY);
            if (confKey == null)
            {
                confKey = Registry.LocalMachine.CreateSubKey(PROACTIVE_AGENT_SUB_KEY);
                confKey.SetValue(CONFIG_LOCATION_KEY, DEFAULT_CONFIG_LOCATION);
            }
            else
            {
                string configLocation = (string)confKey.GetValue(CONFIG_LOCATION_KEY);
                if (configLocation != null)
                {
                    this.configLocation.Text = configLocation;
                }
                else
                {
                    confKey.SetValue(CONFIG_LOCATION_KEY, DEFAULT_CONFIG_LOCATION);
                }
            }
            confKey.Close();
        }

        private void ReadAgentLocation()
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(PROACTIVE_AGENT_SUB_KEY);
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
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(PROACTIVE_AGENT_SUB_KEY, true);
            if (confKey == null)
                return;
            confKey.SetValue(CONFIG_LOCATION_KEY, configLocation.Text);
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
                this.notifyIcon1.Text = STOP_PENDING;
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

                this.notifyIcon1.Text = STOPPED;
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

                this.notifyIcon1.Text = START_PENDING;
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

                this.notifyIcon1.Text = RUNNING;
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

                this.notifyIcon1.Text = UNKNOWN;
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
            // 1) Parse the xml config file            
            try
            {
                ConfigurationParser.parseXml(configLocation.Text, agentLocation);
            }
            catch (Exception ex) // since the parseXml method does not declare any exceptions like IncorrectConfigurationException 
            {
                // Cannot continue show error message box and exit from this method
                MessageBox.Show("The configuration file is broken. " + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2) Start the service
            try
            {
                sc.Start();
            }
            catch (Exception ex)
            {
                // Cannot continue show error message box and exit from this method
                MessageBox.Show("Could not start the service. " + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3) Disable gui components to avoid user errors
            startService.Enabled = false;
            stopService.Enabled = false;
            startServiceToolStripMenuItem.Enabled = false;
            stopServiceToolStripMenuItem.Enabled = false;
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
            try
            {
                sc.Refresh();
                UpdateStatus();
            }
            catch (Exception)
            {
                // In case of exception just exit silently
                // This can happend when the service is terminated
                this.Close();
                Application.ExitThread();
            }
        }

        private void viewLogs_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "notepad.exe";
            p.StartInfo.Arguments = this.agentLocation + "\\ProActiveAgent-log.txt";
            p.Start();
        }

        private void browse_Click(object sender, EventArgs e)
        {
            browseConfig.FileName = configLocation.Text;
            browseConfig.Filter = "Xml File|*.xml";
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
            catch (Exception)
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Configuration conf = ConfigurationParser.parseXml(configLocation.Text, agentLocation);
                window = new ConfigurationEditor(conf, configLocation.Text, agentLocation, this);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("The configuration file is broken.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult res = MessageBox.Show("The configuration file is broken. " + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //TODO : proposer de créer un fichier âr default
                /*if (res == DialogResult.Yes)
                {

                }*/
            }
        }

        public void setConfigLocation(string title)
        {
            configLocation.Text = title;
        }

        private void ConfigurationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                    {
                        //Handle application exit call
                        setVisibleCore = false;
                        Hide();
                        e.Cancel = true;
                        break;
                    }
                case CloseReason.FormOwnerClosing:
                    {
                        //Handle Form owner close
                        break;
                    }
                case CloseReason.MdiFormClosing:
                    {
                        //Handle MDI parent closing
                        break;
                    }
                case CloseReason.None:
                    {
                        //Handle unknown reason
                        break;
                    }
                case CloseReason.TaskManagerClosing:
                    {
                        //Handle taskmanager close
                        this.kill();
                        break;
                    }
                case CloseReason.UserClosing:
                    {
                        // Handle User close
                        setVisibleCore = false;
                        Hide();
                        e.Cancel = true;
                        break;
                    }
                case CloseReason.WindowsShutDown:
                    {
                        // Handle system shutdown
                        this.kill();
                        break;
                    }
            }
        }

        private void kill()
        {
            // if the service is running stop it then exit
            try
            {
                if (this.sc.CanStop && this.sc.Status != ServiceControllerStatus.Stopped)
                {
                    this.sc.Stop();
                }
            }
            catch (Exception) { }
            finally
            {
                this.Close();
                Application.ExitThread();
            }
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
                //MessageBox.Show("==>" + System.Environment.CurrentDirectory + "   " + System.Environment.CommandLine);
                if (confKey != null)
                {
                    confKey.SetValue("ProActive Agent Interface", System.Environment.CommandLine);
                    //                    confKey.SetValue("ProActive Agent Interface", System.Environment.CurrentDirectory + "\\AgentForAgent.exe");
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

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            new VersionChecker().ShowDialog();
        }

        private void proActiveInriaLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // If the value looks like a URL, navigate to it.
            // Otherwise, display it in a message box.
            System.Diagnostics.Process.Start(this.proActiveInriaLinkLabel.Text);            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
