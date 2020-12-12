using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Server
{
    class TopicServer
    {
        public static void HelloWorld()
        {
            Console.WriteLine("Hello world");
        }

        //adding a message to the topic's board
        public static void TopicFileInitialization()
        {
            Console.WriteLine("---Creating the initial topics---\n");
            FileStream fs = new FileStream("./Files/Topics.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            List<Topic> mylist = new List<Topic>();
            List<Message> l = new List<Message>();

            Message test = new Message("DanDan", "This is the first message of the topic, join me!");
            l.Add(test);
            Topic newTopic = new Topic(1, "Coding", l);
            Topic newTopic2 = new Topic(2, "Food", l);
            Topic newTopic3 = new Topic(3, "Music", l);

            mylist.Add(newTopic);
            mylist.Add(newTopic2);
            mylist.Add(newTopic3);


            bf.Serialize(fs, mylist);
            fs.Close();
        }

        //sending the topic list to display
        public static void SendTopicList(TcpClient comm)
        {
            Console.WriteLine("Sending the topic list\n");
            FileStream fs = new FileStream("./Files/Topics.dat", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            List<Topic> topicList = new List<Topic>();
            topicList = (List<Topic>)bf.Deserialize(fs);
            fs.Close();
            bf.Serialize(comm.GetStream(), topicList);

        }
    }
}
