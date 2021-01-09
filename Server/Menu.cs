using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Shared;

namespace Server
{
    //The brain for the project, it manipulates all the info and requests for the chat app
    class Menu
    {
        public static void MainMenu(TcpClient comm, int menu, int received, Message message, string s)
        {
            LoginRequest switcher;
            switcher = Connexion.ReceiveLogin(comm); //Our current user
            BinaryFormatter bf = new BinaryFormatter();

            while (menu != -1) //while not logged out
            {
                received = -10; //initializing the topic command to a non existant value for future use
                Console.WriteLine("Waiting for menu command");
                menu = (int)bf.Deserialize(comm.GetStream()) - 1;
                //Console.WriteLine("stream received : " + menu);
                
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
                                message._un = switcher._un; //name of the sender
                                message._con = s;           //content of the message
                                Console.WriteLine("Sending the message' " + message._con + "' from user '" + message._un + "'");
                                TopicServer.WriteMessageTopic(received, message);
                                Console.WriteLine("Sending the updated Topic List");
                                TopicServer.SendTopicList(comm); //send an event and thread listener (check the calculator)
                            }
                            if(received == -1) //Adding a new topic
                            {
                                string name = (string)bf.Deserialize(comm.GetStream()); //deserializing the name sent by the client
                                TopicServer.AddTopic(name, comm);
                            }

                            s = "init"; //reseting s so that the user can get back in a chat room
                        }
                        break;

                    case 1: //private message request
                        PrivateMessage empty = new PrivateMessage("", "", new List<Message>());
                        string msg = "a";
                        bool check = true;

                        Console.WriteLine("Private message request received, sending the users pms");
                        PrivateMessageServer.SendUsersPM(switcher, comm); //sending to the client the list of its chat rooms
                        while (s.CompareTo("") != 0) //While not exiting
                        {
                            //add display of all curent pms here
                            Console.WriteLine("Waiting for correspondant");
                            s = (string)bf.Deserialize(comm.GetStream());
                            Console.WriteLine("s received : " + s);
                            if (s.CompareTo("") != 0)
                            {
                                if (s.CompareTo("new message") == 0) //if "new message" is received, a PM request is initialized
                                {
                                    Console.WriteLine("Initiating new pm room");
                                    PrivateMessageServer.NewPM(comm, switcher._un);
                                    Console.WriteLine("Sending the updated pm list");
                                    //PrivateMessageServer.AccessPM(comm, s, switcher._un);
                                }
                                else //regular PM
                                {
                                    do
                                    {
                                        PrivateMessageServer.AccessPM(comm, s, switcher._un);
                                        Console.Write("reached check : ");
                                        check = (bool)bf.Deserialize(comm.GetStream()); //If false is received, the conversation doesnt exist
                                        Console.WriteLine(check);
                                        do
                                        {
                                            if (check) //if the room exists, initiate the message writing
                                            {
                                                Console.WriteLine("Reached writepm");
                                                PrivateMessageServer.WritePM(comm, switcher._un, s);
                                                msg = (string)bf.Deserialize(comm.GetStream());
                                                Console.WriteLine("Received msg : " + msg);
                                                PrivateMessageServer.AccessPM(comm, s, switcher._un);
                                            }
                                            else s = (string)bf.Deserialize(comm.GetStream()); //else getting the user client wants to chat to after getting denied
                                        } while (msg.CompareTo("") != 0);
                                    } while (msg != "" && s != ""); //While not fully exited
                                }
                            }
                            //else empty = (PrivateMessage)bf.Deserialize(comm.GetStream()); //catching an empty pm stream if the find fails

                        }
                        received = 0; //resetting variables for the master loop
                        s = "init";   //
                        break;

                }

            }
        }
    }
}
