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
using System.Collections;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AgentFirstSetup
{
    public partial class MainWindow : Form
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public extern static bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;

        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT35 = 1;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT50 = 3;

        private Configuration conf;
        private string configLocation;
        private string agentDir;
        private string path;

        public MainWindow(string path)
        {
            this.path = path;
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
            jvmDirectory.Text = Environment.GetEnvironmentVariable("JAVA_HOME");

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
            if (radioButton2.Checked && checkUser() != 0)
            {
                if (checkUser() == 1)
                {
                    MessageBox.Show("Wrong name/password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("This user is not an administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            try
            {
                ConfigurationParser.saveXml(this.configLocation, conf);

                //--launch install.bat
                Process process = new Process();
                process.StartInfo.FileName = path + "\\" + "install.bat";
                process.StartInfo.UseShellExecute = false;
                if (radioButton2.Checked)
                {
                    process.StartInfo.Arguments = user.Text + " " + password.Text + " " + domain.Text;
                }
                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error");
                }

                
            }
            catch (Exception)
            {
                MessageBox.Show("");
            }
            Close();
        }

        private int checkUser()
        {
            IntPtr UserToken = new IntPtr(0);
            bool loggedOn = false;
            try
            {
                //tente de logger l'utilisateur
                loggedOn = LogonUser(
                     user.Text,
                     domain.Text,
                     password.Text,
                      LOGON32_LOGON_INTERACTIVE,//LOGON32_LOGON_NETWORK,
                      LOGON32_PROVIDER_DEFAULT,
                      ref UserToken);
            }
            catch (Exception ex)
            {
                return 1;
                throw ex;
            }
            if (loggedOn)
            {
                //renvoi identité ASP_NET
                WindowsIdentity ident_here1 = WindowsIdentity.GetCurrent();
                WindowsIdentity SystemMonitorUser = new WindowsIdentity(UserToken);

                //Changement d'utilisateur ici
                WindowsImpersonationContext ImpersonatedUser = SystemMonitorUser.Impersonate();

                //ridentité nouvel User
                WindowsIdentity ident_here2 = WindowsIdentity.GetCurrent();
                WindowsIdentity identity = new WindowsIdentity(UserToken);
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                ImpersonatedUser.Undo();
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                    return 0;
                else
                    return 2;

                
            }
            return 1;
            
        }

        private void closeConfig_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The ProActiveAgent is not configured properly. Thus, it may not work. You can configure it later using AgentControl application.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                panelAccount.Enabled = true;
            }
            else
            {
                panelAccount.Enabled = false;
            }
        }



    }
}
