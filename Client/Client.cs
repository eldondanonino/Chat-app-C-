using System.Net.Sockets;

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
        }


        //main method for client manipulations
        public void Connect()
        {
            int a;
            while (true)
            {
                start();
                a = Pages.Firstpage(comm); //manages the login / signup

                while (a != 0)
                {
                    a = Pages.Mainpage(comm); //manages the actual chat app
                }

            }
        }

        


    }
}
