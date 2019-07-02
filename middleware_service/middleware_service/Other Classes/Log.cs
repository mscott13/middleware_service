using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using middleware_service.Database_Operations;

namespace middleware_service.Other_Classes
{
    public static class Log
    {
        private static string docPath = "C:\\Middleware Service";
        private static string result = "";
        private static Integration intlink;

        public static void Init(Integration integration)
        {
            intlink = integration;
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

            intlink.Log(message);
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
    }
}
