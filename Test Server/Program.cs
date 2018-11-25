﻿using System;
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
    public static class Program
    {
        public static bool AcceptConnections = true;
        public static bool messageReceived = false;
        public static bool messageConfirmation = false;
        public static int connectedUsers = 0;
        public static List<User> UserList = new List<User>();
        public static UdpClient udpServer = new UdpClient(57000);
        public static IPEndPoint ServerEP = new IPEndPoint(IPAddress.Parse("24.20.157.144"), 57000);
        public static byte delay = 9;
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index + 1));

        static void Main(string[] args)
        {
            Listener(udpServer).Wait();
        }

        public static async Task GarbageCollector(string descriptor)
        {
            Console.WriteLine($"Blorg! {descriptor}");
            GC.Collect();
        }

        public static async Task Listener(UdpClient client)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 57000);
            Stopwatch elapsed = new Stopwatch();
            byte[] packet;
            elapsed.Start();
            while (AcceptConnections == true)
            {
                DataProcessor(packet = await Task.Run(() => client.Receive(ref endpoint)), endpoint);
            }
        }

        public static async Task DataProcessor(byte[] packet, IPEndPoint endpoint)
        {
            Console.WriteLine($"Processing {packet[0]}/{packet[1]} : {endpoint}");
            foreach (var (user, index) in UserList.WithIndex()) {
                if (user.Endpoint.Equals(endpoint)) {
                    if (packet[0] == 2) {
                        foreach (Job Job in user.JobList) {
                            if (Job.ID == packet[1]) {
                                Job.IsCompleted = true; }}}
                    else if (packet[0] == 5) {
                        if (user.JobList.Count > 0) {
                            foreach (Job Job in user.JobList) {
                                if (Job.ID == packet[1]) {
                                    return; }
                                else {
                                    CreateJob(user, packet, endpoint); }}}
                        else {
                            CreateJob(user, packet, endpoint); }}

                    return; }}

            Console.WriteLine($"Adding Bob: {endpoint}");
            UserList.Add(new User("Bob", endpoint));
        }

        public static async Task CreateJob(User user, byte[] packet, IPEndPoint endpoint)
        {
            Console.WriteLine($"Creating New Job Type {packet[0]} ID {packet[1]}");
            Job job = new Job(packet[1], packet[0], endpoint, ServerEP);
            user.JobList.Add(job);
            JobManager(job);
        }

        public static async Task JobManager(Job job)
        {
            while (!job.IsCompleted)
            {
                /*if (job.ElapsedTime.ElapsedMilliseconds > 5000) {  }*/
                if (job.Type == 2) {
                    Console.WriteLine($"Job of Type 2"); }
                else if (job.Type == 5) {
                    #region Packet Sizes
                    /////////////////////////////////////
                    //      Break Into 25 Packets      //
                    /////////////////////////////////////
                    //39940//39945//39950//39955//39960// 0000 / 0000 / 0005 / 0015 / 0030 / 
                    //39965//39970//39975//39980//39985// 0050 / 0075 / 0105 / 0140 / 0180 /
                    //39990//39995//40000//40005//40010// 0225 / 0275 / 0330 / 0390 / 0455 /
                    //40015//40020//40025//40030//40035// 0525 / 0600 / 0680 / 0765 / 0855 /
                    //40040//40045//40050//40055//40060// 0950 / 1050 / 1155 / 1265 / 1380 /
                    /////////////////////////////////////
                    #endregion
                    Console.WriteLine($"Job of Type 5");
                    string[] informationToReadBiome = new string[1000000]; 
                    informationToReadBiome = File.ReadAllLines("C:/Users/Hal/Desktop/test1biome.txt");
                    int count = 0;
                    for (int i = 0; i < 25; i++) {
                        byte[] iSend = new byte[39940 + (i * 5) + 2];
                        iSend[0] = job.Type;
                        iSend[1] = job.ID;
                        for (int ii = 0; ii < (39940 + (i * 5)); ii++) {
                            iSend[2 + ii] = byte.Parse(informationToReadBiome[count]);
                            count++; }

                        Speaker(iSend, udpServer, job.Employee);
                        await Task.Delay(25);
                    }

                    string[] informationToReadLand = new string[1000000];
                    informationToReadLand = File.ReadAllLines("C:/Users/Hal/Desktop/test1mod.txt");
                    count = 0;
                    for (int i = 0; i < 25; i++)
                    {
                        byte[] iSend = new byte[39940 + (i * 5) + 2];
                        iSend[0] = 6;
                        iSend[1] = job.ID;
                        for (int ii = 0; ii < (39940 + (i * 5)); ii++)
                        {
                            iSend[2 + ii] = byte.Parse(informationToReadLand[count]);
                            count++;
                        }

                        Speaker(iSend, udpServer, job.Employee);
                        await Task.Delay(25);
                    }

                    GarbageCollector($"Job Type({job.Type}) ID({job.ID}) for {Environment.NewLine}   // Employer ({job.Employer}) / Employee ({job.Employee}) //");
                    await Task.Delay(3000); }

                Console.WriteLine($"{job.IsCompleted}");
            }
            
            return;
        }

        public static async Task Speaker(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            if (packet[0] == 2) {
                Speaker2(packet, client, endpoint); }
            else if (packet[0] == 5 || packet[0] == 6) {
                //Console.WriteLine($"Sending Information Type {packet[0]} ID {packet[1]} Length {packet.Length}");
                client.Send(packet, packet.Length, endpoint); }
        }

        public static async Task Speaker2(byte[] packet, UdpClient client, IPEndPoint endpoint)
        {
            while (true)
            {
                Console.WriteLine($"Sending Confirmation ({packet[0]})");
                client.Send(packet, 2, endpoint);
                await Task.Delay(1000);
            }
        }



        /*

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

        */
    }
}
