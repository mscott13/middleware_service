using System.ServiceProcess;
namespace middleware_service
{
    static class Program
    {
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
