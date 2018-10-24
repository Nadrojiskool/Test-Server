using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Test_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient udpServer = new UdpClient(57000);

            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 57000);
                var data = udpServer.Receive(ref remoteEP); // listen on port 11000
                Console.Write($"receive data from {remoteEP}\n");
                Console.Write($"{data.Count()}\n");
                udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
            }
        }
    }
}
