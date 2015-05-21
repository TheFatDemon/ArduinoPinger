using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace ArduinoPinger
{
    class Program
    {
        static void Main(string[] args)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 240;
            PingReply reply = pingSender.Send(args[0], timeout, buffer, options);
            if(reply.Status == IPStatus.Success)
            {
                // TODO Write to Arduino
            }
        }
    }
}
