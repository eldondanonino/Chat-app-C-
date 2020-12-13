using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;



/// <summary>
/// 
/// Todo : Move all the Login and logout stuff to their own .cs
/// 
/// </summary>

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
            Connexion.Logout(1, ""); //logging out every user 

            //Reset button for the whole "database", be careful 
            /*
            Files.IDfilecreate();
            Files.UsernamesInitialization();
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
                LoginRequest switcher;
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

                            switcher = Connexion.ReceiveLogin(comm); 

                            //user is logged in, main menu
                            while (menu != -1)
                            {
                                Console.WriteLine("Waiting for menu command");
                                menu = (int)bf.Deserialize(comm.GetStream()) - 1;
                                Console.WriteLine("stream received : " + menu);

                                switch(menu)
                                {
                                    case -1 : //logout request
                                        Console.WriteLine("Logout request received");
                                        Connexion.Logout(2, switcher._un);
                                    break;
                                        
                                    case 0: //topic request
                                        Console.WriteLine("Topic request received");
                                        TopicServer.SendTopicList(comm);
                                        while (received != 0)
                                        {
                                            Console.WriteLine("Waiting for the topic to be picked");
                                            received = (int)bf.Deserialize(comm.GetStream());
                                            Console.WriteLine("Stream received : " + received);
                                            while (s.CompareTo("") != 0)
                                            {
                                                
                                                Console.WriteLine("Waiting for messages to the topic");
                                                s = (string)bf.Deserialize(comm.GetStream());
                                                Console.WriteLine("Stream received : " + s);
                                                message._un = switcher._un;
                                                message._con = s;
                                                Console.WriteLine("Sending the message' " + message._con + "' from user '" + message._un + "'");
                                                TopicServer.WriteMessageTopic(received, message);
                                                Console.WriteLine("Sending the updated Topic List");
                                                TopicServer.SendTopicList(comm);
                                            }
                                            s = "init"; //reseting s so that the user can get back in a chat room
                                            //not getting out of this loop if 0 is input, fix
                                        }
                                        break;

                                    case 1: //private message request
                                        Console.WriteLine("AAAAAAAAAAAAAAAAAA");
                                        break;

                                }

                            }
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
