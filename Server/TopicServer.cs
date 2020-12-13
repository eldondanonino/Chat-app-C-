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

            
            Topic newTopic = new Topic(1, "Coding", new List<Message>());
            Topic newTopic2 = new Topic(2, "Food", new List<Message>());
            Topic newTopic3 = new Topic(3, "Music", new List<Message>());

            newTopic._messages.Add(new Message("Dandan", "Man this programming project sure is fascinating (and bug free)"));
            newTopic2._messages.Add(new Message("King Gdd", "I love eating, what's your fav food /ck/ ?"));
            newTopic3._messages.Add(new Message("Dandan", "I like playing the pipeau"));

            mylist.Add(newTopic);
            mylist.Add(newTopic2);
            mylist.Add(newTopic3);


            bf.Serialize(fs, mylist);
            fs.Close();
        }

        //returns the list of topics 
        public static List<Topic> TopicGetter()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("./Files/Topics.dat", FileMode.Open, FileAccess.Read);
            List<Topic> myList = new List<Topic>();
            myList = (List<Topic>)bf.Deserialize(fs);
            fs.Close();
            return myList;
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

        //writing the new message in Topic.dat
        public static void WriteMessageTopic(int topicID, Message message)
        {
            //TopicDisplay();
            Console.WriteLine("Initializing the writing on file\nMessage received : " + message._un + " : " + message._con);
            if(message._con.CompareTo("") == 0)
            {
                Console.WriteLine("Empty message received, canceling...");
                return;
            }
            BinaryFormatter bf = new BinaryFormatter();
            List<Topic> topicList = new List<Topic>();
            List<Message> messageList = new List<Message>();

            topicList = TopicGetter();

            foreach(Topic t in topicList)
            {
                if(t._id == topicID)
                {
                    Console.WriteLine("Adding message : " + message._con + " to topic nb. " + t._id);
                    t._messages.Add(message); //adding the message to the message list from the corresponding topic
                }
            }
            
            FileStream fs = new FileStream("./Files/Topics.dat", FileMode.Create, FileAccess.Write);
            bf.Serialize(fs, topicList);
            fs.Close();
            //TopicDisplay();
        }

        public static void TopicDisplay()
        {
            Console.WriteLine("Displaying all the topics and messages");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("./Files/Topics.dat", FileMode.Open, FileAccess.Read);
            List<Topic> myList = new List<Topic>();
            myList = (List<Topic>)bf.Deserialize(fs);
            foreach(Topic t in myList)
            {
                Console.WriteLine("Topic nb. " + t._id + "\nDisplaying the messages\n");
                foreach(Message m in t._messages)
                {
                    Console.WriteLine(m._un + " : " + m._con);
                }
            }
            fs.Close();
        }
    }
}
