using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace client_client
{
    class Program
    {
        static void Main(string[] args)
        {
            init();
        }

        public static void init()
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress address = host.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(address, 11000);

                Socket client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(localEndPoint);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                while (true)
                {
                    int bytesRec = client.Receive(bytes);
                    if (Encoding.ASCII.GetString(bytes, 0, bytesRec) != null || Encoding.ASCII.GetString(bytes, 0, bytesRec) != "")
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
