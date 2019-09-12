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
        private static StreamWriter outputFile = null;
        public static void Open(string filename)
        {
            outputFile = new StreamWriter(Path.Combine(docPath, filename), true);
        }

        public static void Close()
        {
            if (outputFile != null)
            {
                outputFile.Close();
                outputFile.Dispose();
            }
        }
        
        public static void Save(string msg)
        {
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }
            
            outputFile.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + msg);
            outputFile.Flush();

            BroadcastEvent(new Logging(msg));
        }

        public static void WriteEnd()
        {
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }

            outputFile.WriteLine("--------end\r\n");
            outputFile.Flush();
        }

        private static void BroadcastEvent(object e)
        {
            IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            hub.Clients.All.Event(e);
        }
    }
}
