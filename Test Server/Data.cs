﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test_Server
{
    public class Data
    {
        public byte[] Bytes;
        public IPEndPoint Endpoint;

        public Data(byte[] b, IPEndPoint ep)
        {
            Bytes = b;
            Endpoint = ep;
        }
    }
}
