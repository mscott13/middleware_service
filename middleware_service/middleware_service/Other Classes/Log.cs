using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using middleware_service.Database_Operations;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace middleware_service.Other_Classes
{
    public static class Log
    {
        private static string docPath = "C:\\Middleware Service";
        private static string result = "";
        private static Integration intlink;
        private static EventLog evt;
        private static string message = "";
        private static Socket handler;
        private static Socket server;
        private static bool run = true;
        private static int port = 11000;

        public static void Init(Integration integration, EventLog _evt)
        {
            intlink = integration;
            evt = _evt;
            Thread thread = new Thread(new ThreadStart(InitSocket));
            thread.Start();
        }

        public static void StopSocketConnection()
        {
            try
            {
                server.Close();
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                evt.WriteEntry(e.Message, EventLogEntryType.Information);
            }

            run = false;
        }

        public static string Save(string msg)
        {
            try
            {
                if (!Directory.Exists(docPath))
                {
                    Directory.CreateDirectory(docPath);
                }

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Log.txt"), true))
                {
                    result = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + msg;
                    outputFile.WriteLine(result);
                }

                intlink.Log(msg);
                evt.WriteEntry(msg, EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                message = e.Message;
                evt.WriteEntry(msg + " ~ ", EventLogEntryType.Information);
            }
            
            message = msg;
            return result;
        }

        public static void WriteEnd()
        {
            try
            {
                if (!Directory.Exists(docPath))
                {
                    Directory.CreateDirectory(docPath);
                }

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Log.txt"), true))
                {
                    Thread.Sleep(50);
                    outputFile.WriteLine("--------end\r\n");
                    message = "--------end\r\n";
                    result = "newline";
                }
                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                message = e.Message;
                evt.WriteEntry(e.Message + " ~ ", EventLogEntryType.Information);
            }
        }

        public static void InitSocket()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost"); ;
            IPAddress address = host.AddressList[0]; ;
            IPEndPoint localEndPoint = new IPEndPoint(address, port); ;

            while (run)
            {
                try
                {
                    server = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    server.Bind(localEndPoint);
                    server.Listen(10);

                    handler = server.Accept();

                    while (run)
                    {
                        Thread.Sleep(5);
                        byte[] bytes = null;
                        bytes = Encoding.ASCII.GetBytes(message);
                        handler.Send(bytes);
                        message = "";
                    }

                }
                catch (Exception e)
                {
                    evt.WriteEntry(e.Message, EventLogEntryType.Information);
                    server.Close();
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }

            try
            {
                server.Close();
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                evt.WriteEntry(e.Message, EventLogEntryType.Information);
            }
        }
    }
}
