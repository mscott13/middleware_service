using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace client_client
{
    class Program
    {
        static string filename = "Log.txt";
        static string line = "";

        static void Main(string[] args)
        {

            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "resources";
                string path_mon_desc = "Monitoring path: " + path;
                Console.WriteLine(path_mon_desc);
                Console.WriteLine("target: " + filename);

                for (int i = 0; i < path_mon_desc.Length; i++)
                {
                    Console.Write("=");
                }
                Console.WriteLine("\n");

                FileSystemWatcher fileWatch = new FileSystemWatcher();
                fileWatch.Path = path;
                fileWatch.Changed += FileWatch_Changed;
                fileWatch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                fileWatch.Filter = "*.txt";
                fileWatch.EnableRaisingEvents = true;

                while (true) { }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void FileWatch_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "resources";
                string result = File.ReadLines(path + @"\Log.txt").Last();

                if (line != "result")
                {
                    Console.WriteLine(result);
                    line = result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
