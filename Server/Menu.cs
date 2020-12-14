using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;

namespace Server
{
    class Menu
    {
        public static void MainMenu(TcpClient comm, int menu, int received, Message message, string s)
        {
            LoginRequest switcher;
            switcher = Connexion.ReceiveLogin(comm);
            BinaryFormatter bf = new BinaryFormatter();

            while (menu != -1)
            {
                received = -10;
                Console.WriteLine("Waiting for menu command");
                menu = (int)bf.Deserialize(comm.GetStream()) - 1;
                Console.WriteLine("stream received : " + menu);
                
                switch (menu)
                {
                    case -1: //logout request
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
                            while (s.CompareTo("") != 0 && received != 0 && received != -1) //empty message to exit the topic, 0 to exit the topic list, -1 to create a topic
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
                            if(received == -1)
                            {
                                string name = (string)bf.Deserialize(comm.GetStream());
                                TopicServer.AddTopic(name, comm);
                            }

                            s = "init"; //reseting s so that the user can get back in a chat room
                                        //not getting out of this loop if 0 is input, fix
                        }
                        break;

                    case 1: //private message request
                        PrivateMessage empty = new PrivateMessage("", "", new List<Message>());
                        string msg = "a";
                        Console.WriteLine("Private message request received, sending the users pms");
                        PrivateMessageServer.SendUsersPM(switcher, comm);
                        while (s.CompareTo("") != 0)
                        {
                            //add display of all curent pms here
                            Console.WriteLine("Waiting for correspondant");
                            s = (string)bf.Deserialize(comm.GetStream());
                            
                            if (s.CompareTo("") != 0)
                            {
                                do
                                {
                                    PrivateMessageServer.AccessPM(comm, s, switcher._un);
                                    Console.WriteLine("Reached writepm");
                                    PrivateMessageServer.WritePM(comm, switcher._un, s);
                                    msg = (string)bf.Deserialize(comm.GetStream());
                                    //PrivateMessageServer.AccessPM(comm, s, switcher._un);

                                } while (msg != "");

                                s = (string)bf.Deserialize(comm.GetStream());
                            }
                            //else empty = (PrivateMessage)bf.Deserialize(comm.GetStream()); //catching an empty pm stream if the find fails

                        }
                        received = 0;
                        s = "init";
                        break;

                }

            }
        }
    }
}
