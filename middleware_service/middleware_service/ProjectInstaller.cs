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
            
            processInstaller.Account = ServiceAccount.NetworkService;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = true;
            serviceInstaller.Description = "Syncronize databases between ASMS and SAGE";
            serviceInstaller.ServiceName = "middleware_service";
            serviceInstaller.DisplayName = "Middleware Service";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
