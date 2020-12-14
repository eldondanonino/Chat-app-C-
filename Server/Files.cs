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
    class Files
    {
        //Method getting the username list 
        public static List<Username> StartupUsernames()
        {
            Console.WriteLine("---Getting the username list---\n");
            List<Username> usernames = new List<Username>();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead("./Files/usernames.dat");

            usernames = (List<Username>)bf.Deserialize(fs);
            /* //Checking the integrity of the data received
            foreach(Username u in usernames)
            {
                Console.WriteLine(u._un);
            }*/
            fs.Close();

            return usernames;
        }

        //Method displaying the users and their attributes
        public static void UsersDisplay()
        {
            Console.WriteLine("---Displaying the full user list---\n");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead("./Files/usernames.dat");
            List<Username> usernames = new List<Username>();
            usernames = (List<Username>)bf.Deserialize(fs);
            foreach (Username u in usernames)
            {
                Console.WriteLine(u._un + ", Password : " + u._pwd + ", ID : " + u._id + ", Connection status : " + u._log);
            }
            Console.WriteLine("\n");
            fs.Close();
        }

        //Initializing the username "database" with two new users 
        public static void UsernamesInitialization()
        {
            Console.WriteLine("---Initilaizing the first two users---\n");
            FileStream fs = new FileStream("./Files/usernames.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();

            List<Username> init = new List<Username>();
            init.Add(new Username(0, "Dandan", "sudo", false));
            init.Add(new Username(1, "King Gdd", "monke", false));
            bf.Serialize(fs, init);
            fs.Close();
        }

        //Initializing the ID file to 2 (since our two initial users take 2 spots)
        public static void IDfilecreate()
        {
            FileStream fs = new FileStream("./Files/ID.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            int id = 2;
            bf.Serialize(fs, id);
            fs.Close();
        }

        //Creating a unique ID for a user
        public static int IDcreate()
        {
            FileStream fs = File.OpenRead("./Files/ID.dat");
            BinaryFormatter bf = new BinaryFormatter();
            int id = (int)bf.Deserialize(fs);
            id++;
            fs.Close();
            fs = File.Create("./Files/ID.dat");
            bf.Serialize(fs, id);
            fs.Close();

            return id;
        }

        //Adding a new user and his ID to the list of usernames
        public static void UserCreation(string username, string password, List<Username> usernameList, TcpClient comm)
        {
            bool b = true; // true = does not exist in the database yet 

            FileStream fs = File.OpenRead("./Files/usernames.dat");
            List<Username> list = new List<Username>();
            BinaryFormatter bf = new BinaryFormatter();
            list = (List<Username>)bf.Deserialize(fs);
            Console.WriteLine("Searching for existing username : " + username + "\n");
            foreach (Username u in list)
            {
                //Console.WriteLine("current username : " + u._un);
                if (u._un == username)
                {
                    Console.WriteLine("Same username foud! Initiating refusal of creation\n");
                    b = false;
                }
            }
            bf.Serialize(comm.GetStream(), b);
            fs.Close();
            fs = new FileStream("./Files/usernames.dat", FileMode.Open, FileAccess.Write);


            if (b)
            {
                int tempid = IDcreate();
                Username tempUN = new Username(tempid, username, password, false);

                usernameList.Add(tempUN);
                bf.Serialize(fs, usernameList);

                Console.WriteLine("\nCongratulations, New user " + tempUN._un + "\nPassword : " + password + "\ncreated with ID : " + tempid);
            }
            else
            {
                Console.WriteLine("\nThis user already exists, aborting creation\n");
            }
            fs.Close();
        }

        //Initializing the PM file 
        public static void PMfilecreate()
        {
            FileStream fs = new FileStream("./Files/PM.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            List<Message> messages = new List<Message>();
            Message m = new Message("Dandan", "This is the first private message");
            messages.Add(m);
            PrivateMessage pm = new PrivateMessage("Dandan", "King Gdd", messages);
            List<PrivateMessage> myList = new List<PrivateMessage>();
            myList.Add(pm);
            bf.Serialize(fs, myList);
            fs.Close();
        }


        public static List<PrivateMessage> PMlistGetter()
        {
            Console.WriteLine("---Getting the PM list---\n");
            List<PrivateMessage> myList = new List<PrivateMessage>();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead("./Files/PM.dat");

            myList = (List<PrivateMessage>)bf.Deserialize(fs);
            fs.Close();

            return myList;
        }

        public static void PMcreate(string un1, string un2, TcpClient comm )
        {
            BinaryFormatter bf = new BinaryFormatter();
            List<Message> pm = new List<Message>();
            PrivateMessage newPM = new PrivateMessage(un1, un2, pm);
            List<PrivateMessage> myList = PMlistGetter();
            myList.Add(newPM);
            FileStream fs = new FileStream("./Files/PM.dat", FileMode.Create, FileAccess.Write);
            bf.Serialize(fs, myList);
            fs.Close();
        }

        public static void PMsender(TcpClient comm)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(comm.GetStream(), PMlistGetter());
        }


    }
}
