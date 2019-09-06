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
        private static string docPath = AppDomain.CurrentDomain.BaseDirectory + "resources";
        private static string result = "";
        private static Integration intlink;
        private static EventLog evt;
        private static string message = "";
        public static void Init(Integration integration, EventLog _evt)
        {
            intlink = integration;
            evt = _evt;
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

            evt.WriteEntry(msg, EventLogEntryType.Information);
            intlink.Log(msg);
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
    }
}
