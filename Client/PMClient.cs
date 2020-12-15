using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;
namespace Client
{
    class PMClient
    {
        public static void PMNavigation(TcpClient comm)
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<PrivateMessage> myPrivateMessages = new List<PrivateMessage>();
            List<Message> messageList = new List<Message>();
            List<Message> empty = new List<Message>();
            string correspondant, s = "init";
            
            //Console.WriteLine("Sending a pm request to server");
            bf.Serialize(comm.GetStream(), 2);
            myPrivateMessages = (List<PrivateMessage>)bf.Deserialize(comm.GetStream());
            do
            {   
                Console.Clear();
                Console.WriteLine("Your private messages : ");
                foreach(PrivateMessage pm in myPrivateMessages)
                {
                    Console.WriteLine(pm.un1 + " / " + pm.un2 + " ( " + (pm.pm.Count-1) + " messages)");
                }
                Console.WriteLine("\n(Write new message to request a private chat with a user!)\nSend a message to : ");
                correspondant = Console.ReadLine();
                if (correspondant.CompareTo("") != 0)
                {
                    bf.Serialize(comm.GetStream(), correspondant);
                    //Console.WriteLine("correspondant : " + correspondant);
                    if (correspondant.CompareTo("new message") == 0)
                    {
                        Console.WriteLine("With whom do you want to initiate a private chat?");
                        correspondant = Console.ReadLine();
                        bf.Serialize(comm.GetStream(), correspondant);
                    }
                    else
                    {
                        messageList = (List<Message>)bf.Deserialize(comm.GetStream());
                        Console.Clear();

                        if (messageList.Count == 0)
                        {
                            Console.WriteLine("Sorry you dont have a conversation with this user, try requesting a private chat first!");
                            bf.Serialize(comm.GetStream(), false);
                        }
                        else
                        {
                            bf.Serialize(comm.GetStream(), true);
                            do
                            {
                                Console.Clear();
                                Console.WriteLine("Private chat between you and " + correspondant + "\n____________\n");
                                foreach (Message m in messageList)
                                    Console.WriteLine(m._un + " : " + m._con);
                                Console.Write("(press enter on an empty message to exit the conversation)\n___________\n\n>write a message : ");
                                s = Console.ReadLine();
                                Console.WriteLine("Sending message " + s + " to server");
                                bf.Serialize(comm.GetStream(), s);
                                string msg = (string)bf.Deserialize(comm.GetStream());
                                bf.Serialize(comm.GetStream(), msg);
                                messageList =(List<Message>)bf.Deserialize(comm.GetStream());
                                //bf.Serialize(comm.GetStream(), s);
;                                //messageList = (List<Message>)bf.Deserialize(comm.GetStream());
                            } while (s.CompareTo("") != 0);
                            //Console.WriteLine("AAAAAAAAA " + correspondant);
                            //s = (string)bf.Deserialize(comm.GetStream());
                            
                        }
                        //Console.WriteLine("AAAAAAAAA " + correspondant);
                       
                    }
                    
                }
                else bf.Serialize(comm.GetStream(), correspondant);
            } while (correspondant.CompareTo("") != 0);
            Console.WriteLine("Empty correspondant, going back to main menu");
        }


    }
}
