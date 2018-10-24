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
            List<User> UserList = new List<User>();
            int connectedUsers = 0;

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 57000);
                var data = udpServer.Receive(ref remoteEP); // listen on port 57000
                List<byte[]> packet = new List<byte[]>();
                Console.Write($"receive data from {remoteEP}\n");
                Console.Write($"Adding Username & Endpoint to Cache\n");
                UserList.Add(new User(Encoding.Default.GetString(data), remoteEP));
                if (UserList.Count() > connectedUsers) { Console.WriteLine($"User {Encoding.Default.GetString(data)} Cached!"); connectedUsers++; udpServer.Send(new byte[] { 1 }, 1, remoteEP); }
                else { Console.WriteLine($"MERP!"); }
                for (int i = 0; i < 20; i++)
                {
                    packet.Add(udpServer.Receive(ref remoteEP));
                    Console.WriteLine($"Received {i + 1} Packet(s)!");
                    udpServer.Send(new byte[] { 1 }, 1, remoteEP);
                }
                Console.Write($"Total Packets Received: {packet.Count()}\n");
            }
        }
    }
}
