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
        public byte ID;
        public byte Type;
        public IPEndPoint Employee;
        public IPEndPoint Employer;
        public Stopwatch ElapsedTime;
        public bool IsActive;
        public bool IsCompleted;

        public Job(byte id, byte type, IPEndPoint ep, IPEndPoint EP)
        {
            ID = id;
            Type = type;
            Employee = ep;
            Employer = EP;
            ElapsedTime.Start();
        }
    }
}
