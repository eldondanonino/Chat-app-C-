using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;

/// <summary>
/// TODO : create and display a topic list 
/// each topic has its own .dat containing a log of messages
/// </summary>

namespace Client
{
    class Client
    {
        private readonly string hostname;
        private readonly int port;
        private TcpClient comm;
        public Client(string h, int p)
        {
            hostname = h;
            port = p;
        }
        public void start()
        {
            comm = new TcpClient(hostname, port);
            //Console.WriteLine("\nConnection established Client Side\n");
        }


        //main method for client manipulations
        public void Connect()
        {
            int a;
            while (true)
            {
                start();
                a = Pages.Firstpage(comm);

                while (a != 0)
                {
                    a = Pages.Mainpage(comm);
                }

            }
        }

        


    }
}
