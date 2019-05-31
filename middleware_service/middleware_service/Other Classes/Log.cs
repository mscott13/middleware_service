using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace middleware_service.Other_Classes
{
    public static class Log
    {
        private static string docPath = "C:\\Middleware Service";
        private static string result = "";

        public static void StartServer()
        {
            Save("Starting log server...");
            Thread thread = new Thread(new ThreadStart(startPipeServer));
            thread.Start();
        }

        public static string Save(string message)
        {
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Log.txt"), true))
            {
                result = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + message;
                outputFile.WriteLine(result);
            }
            return result;
        }

        public static void WriteEnd()
        {
            string docPath = "C:\\Middleware Service";
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Log.txt"), true))
            {
                outputFile.WriteLine("--------end\r\n");
                result = "newline";
            }
        }

        private static void startPipeServer()
        {
            bool restart = false;
            PipeSecurity ps = new PipeSecurity();
            PipeAccessRule psRule = new PipeAccessRule(@"Everyone", PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
            ps.AddAccessRule(psRule);
            var server = new NamedPipeServerStream("middleware-link", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1, 1, ps);

            server.WaitForConnection();

            StreamReader reader = new StreamReader(server);
            StreamWriter writer = new StreamWriter(server);
            Save("Log server started");
            WriteEnd();

            while (true)
            {
                if (restart)
                {
                    ps = new PipeSecurity();
                    psRule = new PipeAccessRule(@"Everyone", PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
                    ps.AddAccessRule(psRule);
                    server = new NamedPipeServerStream("middleware-link", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1, 1, ps);

                    server.WaitForConnection();
                    reader = new StreamReader(server);
                    writer = new StreamWriter(server);
                    restart = false;
                    Save("Log server restarted");
                }

                Thread.Sleep(1);
                try
                {
                    writer.WriteLine("");
                    writer.Flush();

                    if (result != "")
                    {
                        writer.WriteLine(result);
                        writer.Flush();
                        result = "";
                    }
                }
                catch (Exception e)
                {
                    Save(e.Message);
                    server.Disconnect();
                    server.Dispose();
                    restart = true;
                }
            }
        }
    }
}
