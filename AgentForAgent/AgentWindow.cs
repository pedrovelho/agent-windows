/*
 * ################################################################
 *
 * ProActive: The Java(TM) library for Parallel, Distributed,
 *            Concurrent computing with Security and Mobility
 *
 * Copyright (C) 1997-2010 INRIA/University of 
 *                                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 *
 * If needed, contact us to obtain a release under GPL Version 2 
 * or a different license than the GPL.
 *
 *  Initial developer(s):               The ProActive Team
 *                        http://proactive.inria.fr/team_members.htm
 *  Contributor(s): ActiveEon Team - http://www.activeeon.com
 *
 * ################################################################
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using ConfigParser;
using Microsoft.Win32;
using ProActiveAgent;

namespace AgentForAgent
{
    public partial class ConfigurationDialog : Form
    {

        public const string AGENT_AUTO_RUN_SUBKEY = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

        private ConfigurationEditor window;
        private readonly string agentLocation;
        private readonly ServiceController sc;
        private Thread pipeClientThread;


        public ConfigurationDialog(string agentLocation, string configLocation, ServiceController sc)
        {
            // Init all visuals components
            InitializeComponent();

            this.agentLocation = agentLocation;
            this.configLocation.Text = configLocation;
            this.sc = sc;

            // Update the status
            this.updateStatus();

            // Start pipe client            
            this.startPipeClientThread();
            
            // Find registry value for auto start agent
            try
            {
                RegistryKey confKey = Registry.LocalMachine.CreateSubKey(AGENT_AUTO_RUN_SUBKEY);
                if (confKey != null)
                {
                    if (confKey.GetValue(Constants.PROACTIVE_AGENT_SERVICE_NAME) != null)
                    {
                        this.startToolStripMenuItem.CheckState = CheckState.Checked;
                    }
                    confKey.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot create the following subkey " + AGENT_AUTO_RUN_SUBKEY + ". " + e.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void receiveFromPipe()
        {
            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", Constants.PIPE_NAME, PipeDirection.In))
            {
                try
                {
                    // The connect function will indefinately wait for the pipe to become available
                    // If that is not acceptable specify a maximum waiting time (in ms)
                    pipeStream.Connect();

                    using (StreamReader sr = new StreamReader(pipeStream))
                    {
                        string temp;
                        // We read from the pipe to the end
                        while ((temp = sr.ReadLine()) != null)
                        {                            
                            this.Invoke((MethodInvoker)delegate
                            {
                                if (this.spawnedRuntimesValue.Text != temp)
                                {
                                    this.spawnedRuntimesValue.Text = "" + temp; // runs on UI thread
                                    int runningExecutorsCount = Convert.ToInt32(temp);
                                    agentStatusNotifyIcon.Icon = Icon = (runningExecutorsCount > 0 ? (Icon)Resource1.icon_active : (Icon)Resource1.icon_passive);
                                }
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    // Pipe connection lost, maybe later we can handle this and report it to the user
                    // anyway if the service is restarted manually, the gui will need to be restarted 
                }
                this.Invoke((MethodInvoker)delegate
                {
                    this.spawnedRuntimesValue.Text = "";
                });
            }            
        }

        private void startPipeClientThread()
        {
            // There must be no already running pipe client thread
            // and the service must be running otherwise the pipe.Connect()
            // can hang on at 100% CPU
            if (this.pipeClientThread == null && this.sc.Status == ServiceControllerStatus.Running) {                        
                this.pipeClientThread = new Thread(receiveFromPipe);
                this.pipeClientThread.IsBackground = true;
                pipeClientThread.Start();
            }
        }

        private void updateStatus()
        {
            // Refresh the service status
            this.sc.Refresh();
            this.agentStatusValue.Text = this.agentStatusNotifyIcon.Text = Enum.GetName(typeof(ServiceControllerStatus), this.sc.Status);
            switch (this.sc.Status)
            {
                case ServiceControllerStatus.StopPending:            
                    this.startService.Enabled = false;
                    this.stopService.Enabled = false;
                    this.startServiceToolStripMenuItem.Enabled = false;
                    this.stopServiceToolStripMenuItem.Enabled = false;                    
                    break;
                case ServiceControllerStatus.Stopped:                    
                    this.startService.Enabled = true;
                    this.stopService.Enabled = false;
                    this.startServiceToolStripMenuItem.Enabled = true;
                    this.stopServiceToolStripMenuItem.Enabled = false;
                    
                    // Set the pipe to null
                    this.pipeClientThread = null;
                    break;
                case ServiceControllerStatus.StartPending:                    
                    this.startService.Enabled = false;
                    this.stopService.Enabled = false;
                    this.startServiceToolStripMenuItem.Enabled = false;
                    this.stopServiceToolStripMenuItem.Enabled = false;                    
                    break;
                case ServiceControllerStatus.Running:                    
                    this.startService.Enabled = false;
                    this.stopService.Enabled = true;
                    this.startServiceToolStripMenuItem.Enabled = false;
                    this.stopServiceToolStripMenuItem.Enabled = true;                    
                    break;
                default:                    
                    this.startService.Enabled = false;
                    this.stopService.Enabled = false;
                    this.startServiceToolStripMenuItem.Enabled = false;
                    this.stopServiceToolStripMenuItem.Enabled = false;
                    break;
            }

            // If the service is not running a red icon will appear
            if (sc.Status != ServiceControllerStatus.Running)
            {
                
                agentStatusNotifyIcon.Icon = Icon = (Icon)Resource1.icon_stop;
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

        /// <summary>
        /// Starts the service.
        /// </summary>
        private void startService_Click(object sender, EventArgs e)
        {
            // 1 - Parse the xml config file            
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

            // 2 - Start the service
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

            // 3 - Disable gui components to avoid user errors
            startService.Enabled = false;
            stopService.Enabled = false;
            startServiceToolStripMenuItem.Enabled = false;
            stopServiceToolStripMenuItem.Enabled = false;

            // 4 - Wait for the service to be running (ie the pipe server to be available)
            this.sc.WaitForStatus(ServiceControllerStatus.Running);

            // 5 - Start a new thread that will receive data from the service through the pipe           
            this.startPipeClientThread();

        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        private void stopService_Click(object sender, EventArgs e)
        {
            // 1 - Refresh the service status
            this.sc.Refresh();

            // 2 - Stop the service through the service controller
            try
            {
                if (this.sc.CanStop && this.sc.Status != ServiceControllerStatus.Stopped)
                {
                    this.sc.Stop();
                }
            }
            catch (Exception ex)
            {
                // Cannot continue show error message box and exit from this method
                MessageBox.Show("Cannot stop the service. " + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 3 - Disable all controls
            startService.Enabled = false;
            stopService.Enabled = false;
            startServiceToolStripMenuItem.Enabled = false;
            stopServiceToolStripMenuItem.Enabled = false;

            // 4 - The pipe client thread will terminate and the ref is no longer needed
            this.pipeClientThread = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.sc.Refresh();
                this.updateStatus();
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

        private void browse_Click(object sender, EventArgs args)
        {
            browseConfig.FileName = configLocation.Text;
            browseConfig.Filter = "Xml File|*.xml";
            browseConfig.ShowDialog();
            configLocation.Text = browseConfig.FileName;

            // Try to validate against current version schema
            try
            {
                ConfigurationParser.validateXMLFile(configLocation.Text, agentLocation);
            }
            catch (Exception e)
            {
                MessageBox.Show("The selected file is not a valid xml file du to " + e.Message + " The Agent will probably not work. You can edit the file in the text editor or the graphical one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
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
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                AgentType conf = ConfigurationParser.parseXml(configLocation.Text, agentLocation);
                window = new ConfigurationEditor(conf, configLocation.Text, agentLocation, this);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                DialogResult res = MessageBox.Show("The configuration file is broken. " + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // Add Register the automatic launch
            RegistryKey confKey = Registry.LocalMachine.CreateSubKey(AGENT_AUTO_RUN_SUBKEY);
            if (confKey != null)
            {
                if (((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
                {
                    confKey.SetValue(Constants.PROACTIVE_AGENT_SERVICE_NAME, System.Environment.CommandLine);
                }
                else
                {
                    confKey.DeleteValue(Constants.PROACTIVE_AGENT_SERVICE_NAME);
                }

                confKey.Close();
            }
        }

        private void closeAdministrationPanelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("This operation doesn't change the state of ProActive service", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK)
            {
                this.agentStatusNotifyIcon.Dispose();
                this.Close();
                Application.ExitThread();
            }
        }

        private void configLocation_TextChanged(object sender, EventArgs e)
        {
            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY, true);
            if (confKey != null)
            {
                confKey.SetValue(Constants.PROACTIVE_AGENT_CONFIG_LOCATION_REG_VALUE_NAME, configLocation.Text);
                confKey.Close();
            }
        }

        private void proActiveInriaLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.proActiveInriaLinkLabel.Text);
        }

        private void viewLogsWithIExplorerButton_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "iexplore.exe";
            p.StartInfo.Arguments = this.agentLocation + "\\ProActiveAgent-log.txt";
            p.Start();
        }

        private void documentationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Constants.DOC_LINK);
        }
    }
}
