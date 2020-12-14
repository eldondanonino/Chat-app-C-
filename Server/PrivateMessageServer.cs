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
    class PrivateMessageServer
    {
        public static void SendUsersPM(LoginRequest switcher, TcpClient comm)
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<PrivateMessage> wholeList = Files.PMlistGetter();
            List<PrivateMessage> sentlist = new List<PrivateMessage>();
            foreach(PrivateMessage pm in wholeList)
            {
                if(pm.un1 == switcher._un || pm.un2 == switcher._un)
                {
                    Console.WriteLine("Found a matching conversation between " + pm.un1 + " and " + pm.un2 + ", adding it to the list");
                    sentlist.Add(pm);
                }
            }
            bf.Serialize(comm.GetStream(), sentlist);
        }

        public static void AccessPM(TcpClient comm, string correspondant, string user)
        {
            List<PrivateMessage> myList = Files.PMlistGetter();
            BinaryFormatter bf = new BinaryFormatter();
            bool found = false;

            foreach(PrivateMessage message in myList)
            {
                if((message.un1.CompareTo(user) == 0 && message.un2.CompareTo(correspondant) == 0) || (message.un2.CompareTo(user) == 0 && message.un1.CompareTo(correspondant) == 0))
                {
                    Console.WriteLine("Correspondance found, sending the messages");
                    bf.Serialize(comm.GetStream(), message.pm);
                    found = true;
                }
            }
            if(!found)
            {
                bf.Serialize(comm.GetStream(), myList);
            }


        }

        public static void WritePM(TcpClient comm)
        { 
            BinaryFormatter bf = new BinaryFormatter();
            string message =(string) bf.Deserialize(comm.GetStream());
            Console.WriteLine("Message received : " + message); //todo : add this message to the list of message from this conv
        }

    }

}
