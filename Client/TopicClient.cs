using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Shared;

namespace Client
{
    class TopicClient
    {
        //Topic navigation method
        public static void TopicNavigation(TcpClient comm)
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<Topic> myList = new List<Topic>();
            int a=-1;

            bf.Serialize(comm.GetStream(), 1); //Requesting the display of topics
            myList = (List<Topic>)bf.Deserialize(comm.GetStream());

            while(a != 0) //while the user doesnt want to get back to the main page
            {
                do
                {
                    DisplayTopics(myList);
                    Console.WriteLine("\nWhich topic do you wish to access?");
                    a = Int32.Parse(Console.ReadLine());
                } while (a > myList.Count || a < -1);
                //Console.WriteLine("TopicNavigation() sending : " + a);
                bf.Serialize(comm.GetStream(), a);


                if (a != 0 && a != -1)
                {
                    Console.WriteLine("Displaying the topic " + a);
                   myList = DisplaySpecificTopic(myList, a, comm);
                }
                if (a == 0)
                    return;
                if (a == -1)
                {
                    myList = CreateNewTopic(comm);
                }
                }

        }
        
        public static List<Topic> CreateNewTopic(TcpClient comm)
        {
            Console.Clear();
            BinaryFormatter bf = new BinaryFormatter();
            List<Topic> myList;
            Console.WriteLine("\nWhat is the title of your new topic?");
            string s = Console.ReadLine();
            bf.Serialize(comm.GetStream(), s);
            myList = (List<Topic>)bf.Deserialize(comm.GetStream());
            Console.WriteLine("Topic created!");
            return myList;
        }

        public static string WriteMessageTopic(List<Topic> myList, int topicId, TcpClient comm)
        {
            string s; 
            BinaryFormatter bf = new BinaryFormatter();
            s = Console.ReadLine();
            bf.Serialize(comm.GetStream(), s);
            return s;
        }

        public static List<Topic> DisplaySpecificTopic(List<Topic> l, int topicId, TcpClient comm)
        {
            Console.Clear();
            string s = "hello";
            BinaryFormatter bf = new BinaryFormatter();

            while (s.CompareTo("") != 0)
            {
                //Console.WriteLine("specific display loop accessed");
                foreach(Topic t in l)
                {
                    if (t._id.Equals(topicId))
                    {
                        DisplayTopicMessages(t);
                        s = WriteMessageTopic(l, topicId, comm);
                        //Console.WriteLine("message received : " + s);
                    }
                }
                l = (List<Topic>)bf.Deserialize(comm.GetStream()); ;
            }
            return l;
        }

        public static void DisplayTopicMessages(Topic t)
        {
            Console.Clear();
            Console.WriteLine("\n---Topic : " + t._name + "---\n____________\nMessages :\n\n ");
            foreach(Message m in t._messages)
            {
                Console.WriteLine(m._un + " : " + m._con);
            }
            Console.Write("\n>Press [enter] without typing a message to get back to the topic list\n____________\n\n>Write a message :");
           
        }
        public static void DisplayTopics(List <Topic> l)
        {
            Console.Clear();
            Console.WriteLine("____________\nDisplaying the current list of topics! \n____________\n");
            foreach(Topic t in l)
            {
                Console.WriteLine(t._id + " : " + t._name + "(" + t._messages.Count + " messages)");
            }
            Console.WriteLine("\n0 : Back to Main Page\n-1 : Create a new topic");
        }

    }
}
