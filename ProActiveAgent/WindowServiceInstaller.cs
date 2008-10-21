﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;
using System.Collections;
using System.Collections.Specialized;

namespace ProActiveAgent
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        /// <summary>

        /// Public Constructor for WindowsServiceInstaller.

        /// - Put all of your Initialization code here.

        /// </summary>
        /// 
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;


        public WindowsServiceInstaller()
        {
            Console.WriteLine("Welcome");
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            //# Service Account Information

            //serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            try
            {
                serviceProcessInstaller.Password = null;
                serviceProcessInstaller.Username = null;

                //# Service Information

                serviceInstaller.DisplayName = "ProActive Agent";
                serviceInstaller.StartType = ServiceStartMode.Automatic;
                serviceInstaller.Description = "Background computations on desktop machines";

                //# This must be identical to the WindowsService.ServiceBase name

                //# set in the constructor of WindowsService.cs

                serviceInstaller.ServiceName = "ProActive Agent";

                this.Installers.Add(serviceProcessInstaller);
                this.Installers.Add(serviceInstaller);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {

            Console.WriteLine("Install Start...");

            // Check for Custom User
            /*if (this.Context.Parameters.ContainsKey("MyUserName") == true)
            {
                Console.WriteLine("******* Using CUSTOMISED values ....");
                this.serviceProcessInstaller.Account =
                System.ServiceProcess.ServiceAccount.User;
                this.serviceProcessInstaller.Password =
                this.Context.Parameters["MyPassword"];
                string user = this.Context.Parameters["MyUserName"];
                string domain = this.Context.Parameters["MyDomainName"];

                // Do we use Principal Name @ Authority or Authority\Principal ( Domain\User )
                string userName = string.Empty;
                if (domain.IndexOf(".") > -1 && domain.Length > 1)
                {
                userName = string.Concat(user, "@", domain);
                }
                else
                {
                userName = string.Concat(domain, "\\", user);
                }
                // Update the Correct user Name
                this.serviceProcessInstaller.Username = userName;
            }
            else
            {


                Console.WriteLine("******* Using HARDCODED values ....");
            
                Console.WriteLine("User=");
                this.serviceProcessInstaller.Username = ".\\proactive";

                Console.WriteLine("Password=");
                this.serviceProcessInstaller.Password = "proactive";
            }*/
            Console.WriteLine("******* Using HARDCODED values ....");

            if (!this.Context.Parameters["user"].Equals(""))
            {
                //--Define domain+user
                this.serviceProcessInstaller.Username = String.Concat(this.Context.Parameters["domain"], "\\");
                this.serviceProcessInstaller.Username = String.Concat(this.serviceProcessInstaller.Username, this.Context.Parameters["user"]);

                //-Define password
                this.serviceProcessInstaller.Password = this.Context.Parameters["pass"];

                Console.WriteLine("******* Username: {0}, Account: {1}, Password: {2}",
                this.serviceProcessInstaller.Username,
                this.serviceProcessInstaller.Account,
                this.serviceProcessInstaller.Password);
            }

            // Call the Base Class to finish Installation
            base.Install(stateSaver);
            Console.WriteLine("Install End...");
            
        }
    }
}
