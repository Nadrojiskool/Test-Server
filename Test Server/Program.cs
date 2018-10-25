﻿using System;
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
                }
                else { Console.WriteLine($"MERP!"); }

                InitiateCallback(udpServer, remoteEP);
                while (!messageReceived)
                {
                    Thread.Sleep(50);
                }
                DateTime dateTime = DateTime.UtcNow;

                byte[] informationToSend = new byte[50000];
                string[] informationToWriteBiome = new string[1000000];
                informationToWriteBiome = File.ReadAllLines("C:/Users/Hal/Desktop/test1biome.txt");

                for (int i = 0; i < 20; i++)
                {
                    for (int ii = 0; ii < 50000; ii++)
                    {
                        informationToSend[ii] = byte.Parse(informationToWriteBiome[(i * 50000) + ii]);
                    }

                    udpServer.Send(informationToSend, 50000, remoteEP);
                    Console.WriteLine($"Packet Sent..");
                    Console.WriteLine("Listening for Messages..");
                    InitiateCallback(udpServer, remoteEP);

                    while (messageReceived != true)
                    {
                        if (DateTime.UtcNow.Millisecond > dateTime.Millisecond + 1000)
                        {
                            udpServer.Send(informationToSend, 50000, remoteEP);
                            Console.WriteLine($"Packet Sent..");
                        }
                        Thread.Sleep(50);
                    }

                    messageReceived = false;
                }

                dateTime = DateTime.UtcNow;
                string[] informationToWriteMod = new string[1000000];
                informationToWriteMod = File.ReadAllLines("C:/Users/Hal/Desktop/test1mod.txt");

                for (int i = 0; i < 20; i++)
                {
                    for (int ii = 0; ii < 50000; ii++)
                    {
                        informationToSend[ii] = byte.Parse(informationToWriteMod[(i * 50000) + ii]);
                    }

                    udpServer.Send(informationToSend, 50000, remoteEP);
                    Console.WriteLine($"Packet Sent..");
                    Console.WriteLine("Listening for Messages..");
                    InitiateCallback(udpServer, remoteEP);

                    while (messageReceived != true)
                    {
                        if (DateTime.UtcNow.Millisecond > dateTime.Millisecond + 1000)
                        {
                            udpServer.Send(informationToSend, 50000, remoteEP);
                            Console.WriteLine($"Packet Sent..");
                        }
                        Thread.Sleep(50);
                    }

                    messageReceived = false;
                }
            }
        }

        public static async Task InitiateCallback(UdpClient client, IPEndPoint ep)
        {
            await Task.Run(() => ReceiveCallback(client, ep));
        }

        public static void ReceiveCallback(UdpClient client, IPEndPoint ep)
        {
            client.Receive(ref ep);
            Console.WriteLine("Received Response!");
            messageReceived = true;
        }
    }
}
