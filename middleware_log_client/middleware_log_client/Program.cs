using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Threading;

namespace middleware_log_client
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceController sc = new ServiceController("middleware_service");
            Console.WriteLine("Middleware Log Client");
            Console.WriteLine("---------------------");

            if (sc.Status != ServiceControllerStatus.Running)
            {
                Console.WriteLine("Middleware Service is not running. Please start the service to continue...");
                Console.WriteLine("Service status: "+sc.Status.ToString());
                while (true)
                {
                    sc = new ServiceController("middleware_service");
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        break;
                    }
                }
            }

            Console.Clear();
            Console.WriteLine("Waiting for middleware server...");
            Thread.Sleep(1500);

            var client = new NamedPipeClientStream("middleware-link");
            client.Connect();

            StreamReader reader = new StreamReader(client);
            StreamWriter writer = new StreamWriter(client);

            try
            {
                Console.WriteLine("\nConnected");
                Console.WriteLine("Listening for log messages...\n");
                while (true)
                {
                    string result = reader.ReadLine();
                    if (result != null && result != "")
                    {
                        if (result == "newline")
                        {
                            Console.WriteLine("");
                        }
                        else
                        {
                            Console.WriteLine(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nMiddlware Log Server not started. Make sure the middleware_service is running before starting this client.");
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
            }
        }
    }
}
