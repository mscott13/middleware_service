using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace middleware_service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public ProjectInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();
            
            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = true;
            serviceInstaller.Description = "Syncronize databases between ASMS and SAGE";
            serviceInstaller.ServiceName = "middleware_service";
            serviceInstaller.DisplayName = "Middleware Service";
            AfterInstall += middleware_service_AfterInstall;
            AfterUninstall += ProjectInstaller_AfterUninstall; ;

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }

        private void ProjectInstaller_AfterUninstall(object sender, InstallEventArgs e)
        {
            //remove left over files
        }

        private void middleware_service_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
            {
                sc.Start();
            }
        }

        private void installer_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
