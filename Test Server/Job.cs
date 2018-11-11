using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Test_Server
{
    public class Job
    {
        public List<byte[]> ByteList = new List<byte[]>();
        public byte ID { get; set; }
        public byte Type { get; set; }
        public IPEndPoint Employee { get; set; }
        public IPEndPoint Employer { get; set; }
        public Stopwatch ElapsedTime;
        public bool IsActive = false;
        public bool IsCompleted = false;

        public Job(byte id, byte type, IPEndPoint ep, IPEndPoint EP)
        {
            ID = id;
            Type = type;
            Employee = ep;
            Employer = EP;
        }
    }
}
