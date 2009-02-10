using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using ConfigParser;
using Microsoft.Win32;
using ProActiveAgent;

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

            this.configLocation = "";
            this.agentDir = "";

            RegistryKey confKey = Registry.LocalMachine.OpenSubKey(Constants.PROACTIVE_AGENT_REG_SUBKEY);
            if (confKey == null)
            {
                MessageBox.Show("The " + Constants.PROACTIVE_AGENT_SERVICE_NAME + " is not installed properly." + " Missing " + Constants.PROACTIVE_AGENT_REG_SUBKEY + " reg key. Please restart the installation process.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            this.agentDir = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_INSTALL_REG_VALUE_NAME);
            this.configLocation = (string)confKey.GetValue(Constants.PROACTIVE_AGENT_CONFIG_REG_VALUE_NAME);

            if (this.agentDir == null || this.configLocation == null)
            {
                MessageBox.Show("The " + Constants.PROACTIVE_AGENT_SERVICE_NAME + " is not installed properly. Please restart the installation process.", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            try
            {
                this.conf = ConfigurationParser.parseXml(configLocation, agentDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The " + Constants.PROACTIVE_AGENT_SERVICE_NAME + " is not installed properly. Please restart the installation process.\n" + ex.ToString(), "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            save();
        }

        private void save()
        {
            string accountDomain = null;
            string accountUser = null;
            string accountPassword = null;

            // All radio buttons cannot be checked at the same time
            // If the radioButton1 is checked ie the LocalSystem account will be used

            // Check the radioButton2 ie a specific user
            if (radioButton2.Checked)
            {
                accountDomain = domain.Text;
                accountUser = user.Text;
                accountPassword = password.Text;

                if (accountDomain == null || accountDomain.Equals(""))
                {
                    MessageBox.Show("Please specify a Domain.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (accountUser == null || accountUser.Equals("") || accountPassword == null || accountPassword.Equals(""))
                {
                    MessageBox.Show("Please specify a User/Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!checkUser(accountDomain, accountUser, accountPassword))
                {
                    // No need to print any message, the checkUser function already does it !
                    return;
                }
            }

            try
            {
                // Save the config
                ConfigurationParser.saveXml(this.configLocation, this.conf);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not save the configuration.");
            }

            try
            {
                // Install new version
                SrvInstaller.Install(path + "\\" + Constants.PROACTIVE_AGENT_EXECUTABLE_NAME, Constants.PROACTIVE_AGENT_SERVICE_NAME, Constants.PROACTIVE_AGENT_SERVICE_NAME, accountDomain, accountUser, accountPassword);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not install the " + Constants.PROACTIVE_AGENT_SERVICE_NAME + " service.");
            }
            Close();
        }

        // If the function fails, the return value is false        
        private bool checkUser(string domain, string username, string password)
        {
            IntPtr UserToken = new IntPtr(0);
            bool loggedOn = false;
            try
            {
                //tente de logger l'utilisateur
                loggedOn = LogonUser(
                     username,
                     domain,
                     password,
                      LOGON32_LOGON_INTERACTIVE,//LOGON32_LOGON_NETWORK,
                      LOGON32_PROVIDER_DEFAULT,
                      ref UserToken);

                if (!loggedOn)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong User/Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //renvoi identité ASP_NET <--- ???????????
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
            {
                // Only a valid account with valid rights can be used
                return true;
            }
            else
            {
                MessageBox.Show("This user is not an administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                save();
            }
        }
    }
}
