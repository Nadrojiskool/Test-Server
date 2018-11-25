using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Test_Server
{
    public class User
    {
        public string Username;
        public IPEndPoint Endpoint;
        public List<Job> JobList = new List<Job>();

        public User(string username, IPEndPoint endpoint)
        {
            Username = username;
            Endpoint = endpoint;
        }
    }
}
