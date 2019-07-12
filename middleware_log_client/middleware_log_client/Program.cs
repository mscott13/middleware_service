using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

namespace client_client
{
    class Program
    {
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);
        private static int port = 11000;

        static void Main(string[] args)
        {
            init();
        }


        public static void DisbleQuickEditMode()

        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(hStdin, mode);
        }


        public static void init()
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress address = host.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(address, port);

                Socket client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(localEndPoint);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                while (true)
                {
                    int bytesRec = client.Receive(bytes);
                    string val = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (Encoding.ASCII.GetString(bytes, 0, bytesRec) != null && Encoding.ASCII.GetString(bytes, 0, bytesRec) != "")
                    {
                        Console.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " ~ " + Encoding.ASCII.GetString(bytes, 0, bytesRec));
                        Thread.Sleep(1);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
    }
}
