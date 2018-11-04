using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


/*  Server Commands
 * 0: Establish Connection (Not In Use)
 * 1: Ready to Receive
 * 2: Receipt Confirmation
 * 3: 
 * 4: 
 * 5:
 * 6:
 * 7:
 * 8:
 * 9:
 * 10: Load Map
 * 11
 * 12
 */


namespace Test_Server
{
    public class Program
    {
        public static bool AcceptConnections = true;
        public static bool messageReceived = false;
        public static bool messageConfirmation = false;
        public static int connectedUsers = 0;
        public static List<User> UserList = new List<User>();
        public static UdpClient udpServer = new UdpClient(57000);

        static void Main(string[] args)
        {
            Listener(udpServer);

            while (AcceptConnections)
            {
                Task.Delay(1000);
            }
        }

        public static async Task Listener(UdpClient client)
        {
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();
            int delay = 100;
            while (AcceptConnections == true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 57000);
                Console.WriteLine("Listening for Information..");
                Data data = new Data(await Task.Run(() => client.Receive(ref remoteEP)), remoteEP);
                Console.WriteLine($"Received Information.. {data.Byte[0]} // {remoteEP}");
                if (data.Byte[0] == 2)
                {
                    messageConfirmation = true;
                }
                else
                {
                    if (data.Byte[0] == delay + 100)
                    {
                        messageReceived = true;
                        delay = data.Byte[0];
                        Console.WriteLine($"New Delay: {delay}");
                        data.Byte[0] += 100;
                        Speaker(new byte[] { 2 }, client, remoteEP);
                        Speaker(data.Byte, client, remoteEP);
                    }
                }
            }
        }

        public static async Task Speaker(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            if (packet[0] == 2)
            {
                while (!messageReceived)
                {
                    Console.WriteLine($"Sending Confirmation ({packet[0]})");
                    client.Send(packet, 1, endpoint);
                    await Task.Delay(1000);
                }
                messageReceived = false;
            }
            else
            {
                while (!messageConfirmation)
                {
                    Console.WriteLine($"Requesting Delay Increase to: {packet[0]}");
                    client.Send(packet, 1, endpoint);
                    await Task.Delay(packet[0]);
                }
                messageConfirmation = false;
            }
        }




        /*public static bool AcceptConnections = true;
        public static bool messageReceived = false;
        public static bool messageConfirmation = false;
        public static int connectedUsers = 0;
        public static List<User> UserList = new List<User>();
        public static UdpClient udpServer = new UdpClient(57000);

        static void Main(string[] args)
        {
            NewConnectionListener(udpServer);
            
            while (AcceptConnections)
            {
                Task.Delay(1000);
            }
        }

        public static async Task NewConnectionListener(UdpClient client)
        {
            while (AcceptConnections == true)
            {
                Data data = await DataListener(client);

                if (UserList.Count() == 0)
                {
                    InitializeClient(client, data.Endpoint, data);
                    //UserList[0].Endpoint = data.Endpoint;
                }

                if (data.Byte[0] == 10 && data.Byte[1] == 1)
                {
                    Console.WriteLine($"Start Send {data.Endpoint}");
                    DataSender(client, data.Endpoint);
                }
                else if (data.Byte[0] == 2)
                {
                    messageConfirmation = true;
                }
            }
        }

        public static async Task InitializeClient(UdpClient client, IPEndPoint endpoint, Data data)
        {
            UserList.Add(new User(Encoding.Default.GetString(data.Byte), data.Endpoint));
            Console.Write($"Received data from {data.Endpoint}!\n");
            Console.Write($"Adding Username & Endpoint to Cache!\n");
            if (UserList.Count() > connectedUsers)
            {
                Console.WriteLine($"User {Encoding.Default.GetString(data.Byte)} Cached!");
                connectedUsers++;
            }
            else { Console.WriteLine($"MERP!"); }
        }

        public static async Task<Data> DataListener(UdpClient client)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 57000);
            Console.WriteLine("Listening for Information..");
            Data data = new Data(await Task.Run(() => client.Receive(ref remoteEP)), remoteEP);
            Console.WriteLine("Received Information..");
            Console.WriteLine($"{data.Byte[0].ToString()} // {remoteEP}");
            return data;
        }

        public static async Task DataSender(UdpClient client, IPEndPoint endpoint)
        {
            byte[] informationToSend = new byte[50000];
            string[] informationToWriteBiome = new string[1000000];
            informationToWriteBiome = File.ReadAllLines("C:/Users/Hal/Desktop/test1biome.txt");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 20; i++)
            {
                for (int ii = 0; ii < 50000; ii++)
                {
                    informationToSend[ii] = byte.Parse(informationToWriteBiome[(i * 50000) + ii]);
                }

                client.Send(informationToSend, 50000, endpoint);
                Console.WriteLine($"Packet Sent..");
                Console.WriteLine("Listening for Messages..");

                while (!messageConfirmation)
                {
                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        client.Send(informationToSend, 50000, endpoint);
                        Console.WriteLine($"Packet Sent..");
                        stopwatch.Restart();
                    }
                    Thread.Sleep(50);
                }

                messageConfirmation = false;
            }
            
            string[] informationToWriteMod = new string[1000000];
            informationToWriteMod = File.ReadAllLines("C:/Users/Hal/Desktop/test1mod.txt");

            for (int i = 0; i < 20; i++)
            {
                for (int ii = 0; ii < 50000; ii++)
                {
                    informationToSend[ii] = byte.Parse(informationToWriteMod[(i * 50000) + ii]);
                }

                client.Send(informationToSend, 50000, endpoint);
                Console.WriteLine($"Packet Sent..");
                Console.WriteLine("Listening for Messages..");

                while (!messageConfirmation)
                {
                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        client.Send(informationToSend, 50000, endpoint);
                        Console.WriteLine($"Packet Sent..");
                    }
                    Thread.Sleep(50);
                }

                messageConfirmation = false;
            }
        }*/
    }
}
