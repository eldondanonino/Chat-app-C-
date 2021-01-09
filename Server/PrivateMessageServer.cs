using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Shared;


namespace Server
{
    class PrivateMessageServer
    {
        //serialize the conversations of the user
        public static void SendUsersPM(LoginRequest switcher, TcpClient comm) 
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<PrivateMessage> wholeList = Files.PMlistGetter();
            List<PrivateMessage> sentlist = new List<PrivateMessage>();
            foreach(PrivateMessage pm in wholeList)
            {
                if(pm.un1 == switcher._un || pm.un2 == switcher._un) //if the user is one of the members of the room
                {
                    Console.WriteLine("Found a matching conversation between " + pm.un1 + " and " + pm.un2 + ", adding it to the list\n");
                    sentlist.Add(pm);
                }
            }
            bf.Serialize(comm.GetStream(), sentlist);
        }

        //serialize the conversation between user and correspondant
        public static void AccessPM(TcpClient comm, string correspondant, string user)
        {
            List<PrivateMessage> myList = Files.PMlistGetter();
            List<Message> empty = new List<Message>();
            BinaryFormatter bf = new BinaryFormatter();
            bool found = false;

            foreach(PrivateMessage message in myList)
            {
                //if both user and correspondant are part of the room
                if((message.un1.CompareTo(user) == 0 && message.un2.CompareTo(correspondant) == 0) || (message.un2.CompareTo(user) == 0 && message.un1.CompareTo(correspondant) == 0))
                {
                    Console.WriteLine("Correspondance found, sending the messages");
                    bf.Serialize(comm.GetStream(), message.pm); //serializing the list of messages between the two users
                    found = true;
                }
            }
            if(!found)
            {
                Console.WriteLine("Correspondance not found, sending empty stream");
                bf.Serialize(comm.GetStream(), empty); //serializing an empty list for the client to interpret as the conversation not existing
            }


        }

        //Creating a new chat room
        public static void NewPM(TcpClient comm, string user)
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<PrivateMessage> lpm = Files.PMlistGetter(); //getting the list of chat rooms
            List<Message> l = new List<Message>();
            Message firstmessage = new Message("server", "This is the start of your conversation! Send a message!");
            bool check = false;

            Console.WriteLine("waiting for the username");
            string username = (string)bf.Deserialize(comm.GetStream()); //receiving the user client wants  to send a pm to
            foreach(PrivateMessage pm in lpm)
            {
                //checking if the room already exists
                if(pm.un1.CompareTo(user) == 0 && pm.un2.CompareTo(username) == 0)
                {
                        Console.WriteLine("Already existing pm");
                        check = true; //found existing pms
                }
                if (pm.un2.CompareTo(user) == 0 && pm.un1.CompareTo(username) == 0)
                {
                        Console.WriteLine("Already existing pm");
                        check = true; //found existing pms
                }
            }
            if(!check) //if it doesnt exist, adding the first message
            {
                l.Add(firstmessage);
                PrivateMessage newpm = new PrivateMessage(user, username, l);
                lpm.Add(newpm);
                Console.WriteLine("Added the new pm to the list");
            }
            Files.PMlistSetter(lpm); //writting the file with the new room

        }

        //Writting a message in the chat room
        public static void WritePM(TcpClient comm, string user, string correspondant)
        {
            //Console.WriteLine("WRITE PM");
            BinaryFormatter bf = new BinaryFormatter();
            string message =(string) bf.Deserialize(comm.GetStream());
            Console.WriteLine("Message received in writepm : " + message);
            if (message.CompareTo("") != 0)
            {
                Message newmessage = new Message(user, message);
                List<PrivateMessage> pml = Files.PMlistGetter();
                foreach (PrivateMessage p in pml)
                {
                    if ((p.un1.CompareTo(user) == 0 && p.un2.CompareTo(correspondant) == 0) || (p.un2.CompareTo(user) == 0 && p.un1.CompareTo(correspondant) == 0))
                    {
                        p.pm.Add(newmessage); //adding the message received to the list of message where both usernames coincide
                        Console.WriteLine("added message to the list");
                    }
                }
                Console.WriteLine("Adding the new message to the file");
                Files.PMlistSetter(pml);
                Console.WriteLine("Sending to stream message : " + message);
                bf.Serialize(comm.GetStream(), message);
                
            }
            else
            {
                {
                    Console.WriteLine("Sending to stream message : " + message);
                    bf.Serialize(comm.GetStream(), message);
                }
            }
        }

    }

}
