using System.ServiceProcess;
namespace middleware_service
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
            new middleware_service()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
