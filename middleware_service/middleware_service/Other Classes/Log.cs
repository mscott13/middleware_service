using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNet.SignalR;
using middleware_service.SignalR.EventObjects;

namespace middleware_service.Other_Classes
{
    public class Log
    {
        private string docPath = AppDomain.CurrentDomain.BaseDirectory + "resources";
        private string result = "";

        public string Save(string msg)
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

            var hub = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            hub.Clients.All.Event(new Logging(msg));
            return result;
        }

        public void WriteEnd()
        {
        
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Log.txt"), true))
            {
                outputFile.WriteLine("--------end\r\n");
            }
        }
    }
}
