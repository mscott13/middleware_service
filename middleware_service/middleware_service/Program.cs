using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {

            new middleware_service()
            };
            ServiceBase.Run(ServicesToRun);
#else
            middleware_service service = new middleware_service();
            service.OnDebug();
#endif
        }
    }
}
