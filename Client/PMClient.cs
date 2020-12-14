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
            string correspondant, s;
            Console.WriteLine("Sending a pm request to server");
            bf.Serialize(comm.GetStream(), 2);
            myPrivateMessages = (List<PrivateMessage>)bf.Deserialize(comm.GetStream());
            do
            {
                Console.WriteLine("\n(Write new message to request a private chat with a user!\nSend a message to : ");
                correspondant = Console.ReadLine();
                bf.Serialize(comm.GetStream(), correspondant);
                messageList = (List<Message>)bf.Deserialize(comm.GetStream());
                Console.Clear();
                if(messageList == empty)
                {
                    Console.WriteLine("Sorry you dont have a conversation with this user, try requesting a private chat first!");
                }
                else
                {
                    do
                    {
                        Console.WriteLine("Private chat between you and " + correspondant);
                        foreach (Message m in messageList)
                            Console.WriteLine(m._un + " : " + m._con);
                        Console.Write("(press enter on an empty message to exit the conversation)\n>write a message : ");
                        s = Console.ReadLine();
                        bf.Serialize(comm.GetStream(), s);
                    } while (s.CompareTo("") != 0);
                }

            } while (correspondant.CompareTo("") != 0);

        }


    }
}
