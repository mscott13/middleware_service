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
using Microsoft.AspNet.SignalR;
using middleware_service.SignalR.EventObjects;

namespace middleware_service.Other_Classes
{
    public static class Log
    {
        private static string docPath = AppDomain.CurrentDomain.BaseDirectory + "resources";
        private static string result = "";
        private static Integration intlink;
        
        public static void Init(Integration integration)
        {
            intlink = integration;
        }

        public static string Save(string msg)
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
            //BroadcastEvent(new Logging(msg));
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
                    result = "newline";
                }
                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                intlink.Log(e.Message);
            }
        }

        private static void BroadcastEvent(object e)
        {
            IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            hub.Clients.All.Event(e);
        }
    }
}
