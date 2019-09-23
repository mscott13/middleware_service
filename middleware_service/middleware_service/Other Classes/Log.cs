using System;
using System.IO;
using Microsoft.AspNet.SignalR;
using middleware_service.SignalR.EventObjects;

namespace middleware_service.Other_Classes
{
    public class Log
    {
        private string docPath = AppDomain.CurrentDomain.BaseDirectory + "resources\\";
        public void Save(string msg)
        {
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }

            using (StreamWriter file = new StreamWriter(docPath + Constants.LOG_FILE, true))
            {
                file.WriteLine((DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + msg));
            }

            var hub = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            hub.Clients.All.Event(new Logging(msg));
        }

        public void WriteEnd()
        {
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }

            using (StreamWriter file = new StreamWriter(docPath + Constants.LOG_FILE, true))
            {
                file.WriteLine("--------end\r\n");
            }
        }
    }
}
