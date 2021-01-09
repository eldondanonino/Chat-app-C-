using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Shared;


namespace Server
{
    public class Server
    {
        private readonly int port;
        public Server(int port)
        {
            this.port = port;
        }


        //Server startup
        public void Start()
        {
            Console.WriteLine("Server starting up...\n");
            TcpListener l = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), port);
            l.Start();
            List<Username> bob = new List<Username>();

            Files.UsersDisplay(); //displaying the state the database was left in 
            Connexion.Logout(1, ""); //logging out every user for startup

            //Reset button for the whole "database" and various files , be careful
            /*
            Files.IDfilecreate();
            TopicServer.TopicIDfilecreate();
            Files.UsernamesInitialization();
            Files.PMfilecreate();
            TopicServer.TopicFileInitialization();
            */

            while (true)
            {
                TcpClient comm = l.AcceptTcpClient();
                Console.WriteLine("Connection established @" + comm + "\n");
                new Thread(new Receiver(comm).Master).Start();
            }
        }

        class Receiver
        {
            private TcpClient comm;
            public Receiver(TcpClient s)
            {
                comm = s;
            }

            //master method of the server receiver
            public void Master()
            {
                Console.WriteLine("---Master method reached---\n");

                BinaryFormatter bf = new BinaryFormatter();
                Message message = new Message("init", "init");
                int received, menu = 0;
                string s = "init";

                while (true)
                {
                    Console.WriteLine("Waiting for create or login\n");
                    received = (int)bf.Deserialize(comm.GetStream());

                    switch (received)
                    {
                        
                        case 1 : //Creating a new user

                            Connexion.ReceiveCreationRequest(comm);
                            break;

                        case 2 : //logging in an existing account, waiting for the rest
                            
                            Menu.MainMenu(comm, menu, received, message, s);

                            Console.WriteLine("Getting out of the master switch");
                            break;

                        default: 
                            Console.WriteLine("SOMETHING BROKE AAAAAAAAAAAAAA");
                            break;
                    }
                }
            }
        }


    }

}