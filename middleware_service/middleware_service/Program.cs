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
            new MiddlewareService()
            };
            ServiceBase.Run(ServicesToRun);
#else
            MiddlewareService service = new MiddlewareService();
            service.OnDebug();
#endif
        }
    }
}
