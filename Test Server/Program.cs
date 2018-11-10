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
 * 2: Job Complete Confirmation
 * 3: 
 * 4: 
 * 5: Request Load Map
 * 6:
 * 7:
 * 8:
 * 9:
 * 10: 
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
        public static IPEndPoint ServerEP = new IPEndPoint(IPAddress.Parse("24.20.157.144"), 57000);
        public static byte delay = 9;

        static void Main(string[] args)
        {
            UserList.Add(new User("Home", ServerEP));
            Listener(udpServer).Wait();
        }

        public static async Task Listener(UdpClient client)
        {
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();
            while (AcceptConnections == true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 57000);
                Console.WriteLine("Listening for Information..");
                byte[] bytes = await Task.Run(() => client.Receive(ref remoteEP));
                Data data = new Data(bytes, remoteEP);
                Console.WriteLine($"Received Information.. {data.Byte[0]} : {remoteEP}");
                DataProcessor(data.Byte, client, remoteEP);
            }
        }

        public static async Task DataProcessor(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            Console.WriteLine($"Processing {packet[0]} : {endpoint}");
            foreach (User user in UserList)
            {
                if (user.Endpoint.Equals(endpoint))
                {
                    Console.WriteLine($"Running Task #{packet[1]} for {endpoint}");
                    if (packet[0] == 2)
                    {
                        foreach (Job job in user.JobList)
                        {
                            if (job.ID == packet[1])
                            {
                                job.IsCompleted = true;
                                return;
                            }
                        }
                    }
                    else if (packet[0] == 5)
                    {
                        foreach (Job Job in user.JobList)
                        {
                            if (Job.ID.Equals(packet[1]))
                            {

                                return;
                            }
                        }
                        Console.WriteLine($"Creating New Job Type {packet[0]} ID {packet[1]}");
                        Job job = new Job(packet[1], packet[0], endpoint, ServerEP);
                        user.JobList.Add(job);
                        JobManager(job);
                    }
                    else
                    {
                        /*if (packet[0] == delay + 2)
                        {
                            delay = packet[0];
                            Console.WriteLine($"New Delay: {delay}");
                            Speaker(new byte[] { (byte)(delay + 1) }, client, endpoint);
                            Speaker(new byte[] { 2, (byte)(delay + 1) }, client, endpoint);
                        }*/
                    }
                    return;
                }
                else
                {
                    Console.WriteLine($"No Match for User");
                }
            }

            Console.WriteLine($"Adding Bob: {endpoint}");
            UserList.Add(new User("Bob", endpoint));
        }

        public static async Task JobManager(Job job)
        {
            while (!job.IsCompleted)
            {

                /*if (job.ElapsedTime.ElapsedMilliseconds > 5000)
                {

                }*/
                if (job.Type == 2)
                {
                    Console.WriteLine($"Job of Type 2");
                }
                else if (job.Type == 5)
                {
                    // Break Into 20 Packets //
                    //49950//49955//49960//49965//49970//49975//49980//49985//49990//49995
                    //50005//50010//50015//50020//50025//50030//50035//50040//50045//50050
                    string[] informationToReadBiome = new string[1000000];
                    informationToReadBiome = File.ReadAllLines("C:/Users/Hal/Desktop/test1biome.txt");

                    for (int i = 0; i < 20; i++)
                    {
                        for (int ii = 0; ii < (49950 + (i * 5)); ii++)
                        {
                            byte[] iSend = new byte[49950 + (i * 5) + 2];
                            iSend[0] = job.Type;
                            iSend[1] = job.ID;
                            iSend[ii] = byte.Parse(informationToReadBiome[(i * (49950 + (i * 5))) + ii]);
                            Speaker(iSend, udpServer, job.Employee);
                        }
                    }
                    await Task.Delay(5000);
                }
            }
        }

        public static async Task Speaker(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            if (packet[0] == 2)
            {
                Speaker2(packet, client, endpoint);
            }
            else if (packet[0] == 5)
            {
                Console.WriteLine($"Sending Information Type {packet[0]} ID {packet[1]} Length {packet.Length}");
                client.Send(packet, packet.Length, endpoint);
            }
            else
            {
                //SpeakerElse(packet, client, endpoint);
            }
        }

        public static async Task Speaker2(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            while (packet[1] > delay)
            {
                Console.WriteLine($"Sending Confirmation ({packet[0]})");
                client.Send(packet, 2, endpoint);
                await Task.Delay(1000);
            }
        }

        public static async Task SpeakerElse(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            while (packet[0] > delay)
            {
                Console.WriteLine($"Requesting Delay Increase to: {packet[0]}");
                client.Send(packet, 1, endpoint);
                await Task.Delay(500);
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
