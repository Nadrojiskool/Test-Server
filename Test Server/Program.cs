using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Server
{
    public class Program
    {
        public static bool messageReceived = false;

        public struct UdpState
        {
            public IPEndPoint Endpoint;
            public UdpClient Client;
        }

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
                
                if (UserList.Count() > connectedUsers)
                {
                    Console.WriteLine($"User {Encoding.Default.GetString(data)} Cached!");
                    connectedUsers++;
                    udpServer.Send(new byte[] { 1 }, 1, remoteEP);
                }
                else { Console.WriteLine($"MERP!"); }

                UdpState state = new UdpState();
                state.Endpoint = remoteEP;
                state.Client = udpServer;
                
                DateTime dateTime = DateTime.UtcNow;

                byte[] informationToSend = new byte[50000];
                String[] informationToWriteBiome = new String[1000000];
                informationToWriteBiome = File.ReadAllLines("C:/Users/2/Desktop/test1biome.txt");

                for (int i = 0; i < 20; i++)
                {
                    for (int ii = 0; ii < 50000; ii++)
                    {
                        informationToSend[ii] = byte.Parse(informationToWriteBiome[i * ii]);
                    }

                    udpServer.Send(informationToSend, 50000, remoteEP);
                    Console.WriteLine($"Packet Sent..");
                    Console.WriteLine("Listening for Messages..");
                    udpServer.BeginReceive(new AsyncCallback(ReceiveCallback), state);

                    while (messageReceived != true)
                    {
                        Thread.Sleep(50);
                        if (DateTime.UtcNow.Millisecond > dateTime.Millisecond + 1000)
                        {
                            udpServer.Send(informationToSend, 50000, remoteEP);
                        }
                    }

                    messageReceived = false;
                }

                String[] informationToWriteMod = new String[1000000];
                informationToWriteMod = File.ReadAllLines("C:/Users/2/Desktop/test1mod.txt");

                for (int i = 0; i < 20; i++)
                {
                    for (int ii = 0; ii < 50000; ii++)
                    {
                        informationToSend[ii] = byte.Parse(informationToWriteMod[i * ii]);
                    }

                    udpServer.Send(informationToSend, 50000, remoteEP);
                    Console.WriteLine($"Packet Sent..");
                    Console.WriteLine("Listening for Messages..");
                    udpServer.BeginReceive(new AsyncCallback(ReceiveCallback), state);

                    while (messageReceived != true)
                    {
                        Thread.Sleep(50);
                        if (DateTime.UtcNow.Millisecond > dateTime.Millisecond + 1000)
                        {
                            udpServer.Send(informationToSend, 50000, remoteEP);
                        }
                    }

                    messageReceived = false;
                }



                /*for (int i = 0; i < 20; i++)
                {
                    packet.Add(udpServer.Receive(ref remoteEP));
                    Console.WriteLine($"Received {i + 1} Packet(s)!");
                    udpServer.Send(new byte[] { 1 }, 1, remoteEP);
                }

                Console.Write($"Total Packets Received: {packet.Count()}\n");*/
            }
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint endpoint = (IPEndPoint)((UdpState)(ar.AsyncState)).Endpoint;
            UdpClient client = (UdpClient)((UdpState)(ar.AsyncState)).Client;

            byte[] receiveBytes = client.EndReceive(ar, ref endpoint);

            Console.WriteLine("Received Response!");
            messageReceived = true;
        }
    }
}
