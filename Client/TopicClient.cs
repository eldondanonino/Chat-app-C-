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
            int a;

            bf.Serialize(comm.GetStream(), 1); //Requesting the display of topics
            myList = (List<Topic>)bf.Deserialize(comm.GetStream());

            DisplayTopics(myList);

            do
            {
                Console.WriteLine("\nWhich topic do you wish to access? (0 to return to the main page)");
                a = Int32.Parse(Console.ReadLine());
            } while (a > myList.Count || a < 0);
            if (a != 0)
                DisplaySpecificTopic(myList, a);
            else
                return;

        }
        
        public static void DisplaySpecificTopic(List<Topic> l, int a)
        {
            foreach(Topic t in l)
            {
                if(t._id.Equals(a))
                {
                    DisplayTopicMessages(t);
                }
            }
        }

        public static void DisplayTopicMessages(Topic t)
        {
            Console.WriteLine("Welcome to " + t._name + " !\n____________\nMessages :\n____________\n\n ");
            foreach(Message m in t._messages)
            {
                Console.WriteLine(m._un + " : " + m._con);
            }
            Console.Write("____________\nWrite a message :");
            string s = Console.ReadLine();
        }
        public static void DisplayTopics(List <Topic> l)
        {
            Console.WriteLine("\nDisplaying the current list of topics! \n");
            foreach(Topic t in l)
            {
                Console.WriteLine(t._id + " : " + t._name + "(" + t._messages.Count + " messages)");
            }
        }

    }
}
