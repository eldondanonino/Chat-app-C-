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
        public static void TopicUser(TcpClient comm)
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<Topic> myList = new List<Topic>();
            bf.Serialize(comm.GetStream(), 1); //Requesting the display of topics
            myList = (List<Topic>)bf.Deserialize(comm.GetStream());
            DisplayTopics(myList);


        }

        public static void DisplayTopics(List <Topic> l)
        {
            Console.WriteLine("\nDisplaying the current list of topics! \n");
            foreach(Topic t in l)
            {
                Console.WriteLine(t._name + "(" + t._messages.Count + " messages)");
            }
        }

    }
}
