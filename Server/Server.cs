using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;



/// <summary>
/// 
/// Todo : Duplicata check on sign up
/// 
/// </summary>

namespace Server
{
    public class Server
    {
        private int port;
        public Server(int port)
        {
            this.port = port;
        }


        //Server startup
        public void start()
        {
            Console.WriteLine("Server starting up...\n");
            TcpListener l = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), port);
            l.Start();
            List<Username> bob = new List<Username>();

            UsersDisplay();
            bob = StartupUsernames();
            Logout(1, ""); //logging out every user on server start

            //Reset button for the whole "database", be careful 
            /*
            IDfilecreate();
            UsernamesInitialization();
            */
            UsersDisplay();
            

            while (true)
            {
                TcpClient comm = l.AcceptTcpClient();
                Console.WriteLine("Connection established @" + comm + "\n");
                new Thread(new Receiver(comm).Master).Start();
            }
        }


        //Method getting the username list 
        public static List<Username> StartupUsernames()
        {
            Console.WriteLine("\n---Getting the username list---\n");
            List<Username> usernames = new List<Username>();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead("./Files/usernames.dat");

            usernames = (List<Username>)bf.Deserialize(fs);
            /*
            foreach(Username u in usernames)
            {
                Console.WriteLine(u._un);
            }*/
            fs.Close();
            
            return usernames;
        }

        //Login method (1 for all, 2 for specific)
        public static void Login(int i, string username)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead("./Files/usernames.dat");

            List<Username> list = new List<Username>();
            switch (i)
            {
                //login all users
                case 1:

                    Console.WriteLine("\n---Setting all users as logged in---\n");
                    list = (List<Username>)bf.Deserialize(fs);
                    fs.Close();
                    FileStream fs2 = new FileStream("./Files/usernames.dat", FileMode.Create, FileAccess.Write);
                    foreach (Username u in list)
                    {
                        u._log = true;
                    }
                    bf.Serialize(fs2, list);
                    fs2.Close();
                    break;
                //login a single user
                case 2:
                    Console.WriteLine("\nLogging in user " + username +"\n");
                    list = (List<Username>)bf.Deserialize(fs);
                    fs.Close();
                    fs2 = new FileStream("./Files/usernames.dat", FileMode.Create, FileAccess.Write);
                    foreach (Username u in list)
                    {
                        //Console.WriteLine("current username : " + u._un);
                        if (u._un == username)
                        {
                            u._log = true;
                            Console.WriteLine(username + " is now logged in \n");
                        }
                    }
                    bf.Serialize(fs2, list);
                    fs2.Close();
                    break;
            }

        }

        //Logout method (same)
        public static void Logout(int i, string username)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead("./Files/usernames.dat");
            List<Username> list = new List<Username>();
            switch (i)
            {
                //logout all users
                case 1: 

                    Console.WriteLine("\n---Setting all users as logged out---\n");
                    list = (List<Username>)bf.Deserialize(fs);
                    fs.Close();
                    FileStream fs2 = new FileStream("./Files/usernames.dat", FileMode.Create, FileAccess.Write);
                    foreach (Username u in list)
                    {
                        u._log = false;
                    }
                    bf.Serialize(fs2, list);
                    fs2.Close();
                    break;
                //logout a single user
                case 2:
                    Console.WriteLine("\nLogging out user " + username + "\n");
                    list = (List<Username>)bf.Deserialize(fs);
                    fs.Close();
                    fs2 = new FileStream("./Files/usernames.dat", FileMode.Create, FileAccess.Write);
                    foreach (Username u in list)
                    {
                        if (u._un == username)
                        {
                            u._log = false;
                            Console.WriteLine(username + " is now logged out\n");
                        }
                    }
                    bf.Serialize(fs2, list);
                    fs2.Close();
                    break;
            }

        }

        //Method displaying the users and their attributes
        public static void UsersDisplay()
        {
            Console.WriteLine("\n---Displaying the full user list---\n");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.OpenRead("./Files/usernames.dat");
            List<Username> usernames = new List<Username>();
            usernames = (List<Username>)bf.Deserialize(fs);
            foreach (Username u in usernames)
            {
                Console.WriteLine(u._un + ", Password : " + u._pwd + ", ID : " + u._id + ", Connection status : " + u._log);
            }
            fs.Close();
        }

        //Initializing the username "database" with two new users 
        public static void UsernamesInitialization()
        {
            Console.WriteLine("\n---Initilaizing the first two users---\n");
            FileStream fs = new FileStream("./Files/usernames.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();

            List<Username> init = new List<Username>();
            init.Add(new Username(0,"Dandan", "sudo", false));
            init.Add(new Username(1, "King Gdd", "monke", false));
            bf.Serialize(fs, init);
            fs.Close();
        }


        //Adding a new user and his ID to the list of usernames
        public static void UserCreation(string username, string password, List<Username> usernameList, TcpClient comm)
        {
            bool b = true; // true = does not exist in the database yet 
            
            FileStream fs = File.OpenRead("./Files/usernames.dat");
            List<Username> list = new List<Username>();
            BinaryFormatter bf = new BinaryFormatter();
            list = (List<Username>)bf.Deserialize(fs);
            Console.WriteLine("\nSearching for existing username : " + username + "\n");
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

        //Checking if the submitted credentials are valid for a login
        public static int CheckCredentials(LoginRequest cred)
        {
            
            int checker = 0;
            BinaryFormatter bf = new BinaryFormatter();
            List<Username> users = StartupUsernames();
            foreach(Username u in users)
            {
                if (checker == 0)
                {
                    if (cred._un == u._un)
                    {
                        Console.WriteLine("Username match found");
                        if (cred._pwd == u._pwd)
                        {
                            Console.WriteLine("Password match foud too");
                            if (u._log != true)
                            {
                                checker = 1;
                                u._log = true;
                            }
                            else
                                checker = -1;
                            
                        }
                    }
                }
            }
            switch(checker)
            {
                case -1 : Console.WriteLine("User is already logged in\n");
                    break;
                case 0 : Console.WriteLine("Unable to find matching credentials\n");
                    break;
                case 1 : Login(2, cred._un);
                    break;

            }
            return checker;
        }



        class Receiver
        {
            private TcpClient comm;

            public Receiver(TcpClient s)
            {
                comm = s;
            }

            //Extracting the message from the tcp client in order to use the master switch
            private static string StreamReader(TcpClient comm)
            {
                BinaryFormatter bf = new BinaryFormatter();

                string switcher = (string)bf.Deserialize(comm.GetStream());
                return switcher;
            }

            public void Master()
            {
                Console.WriteLine("\n---Master method reached---\n");
                BinaryFormatter bf = new BinaryFormatter();
                LoginRequest switcher = new LoginRequest("","");
                int received = 0;
                int check = 0;
                string user, pwd;
                string logout = "";
                while (true)
                {
                    Console.WriteLine("Waiting for create or login\n");
                    received = (int)bf.Deserialize(comm.GetStream());
                    switch (received)
                    {
                        //Creating a new user
                        case 1 :
                            Console.WriteLine("---Waiting for the creation request---\n");
                            switcher = (LoginRequest)bf.Deserialize(comm.GetStream());
                                
                            user = switcher._un;
                            pwd = switcher._pwd;
                            Console.WriteLine("\nUser Creation initiated server side! received : " + user + "\nPassword : " + pwd);
                            UserCreation(user, pwd, StartupUsernames(), comm);
                            //UsersDisplay();
                            break;

                        //logging in an existing account
                        case 2 :
                            Console.WriteLine("---Waiting for the login request---\n");
                            while (check == 0 || check == -1)
                            {
                                switcher = (LoginRequest)bf.Deserialize(comm.GetStream());
                                Console.WriteLine("\nLogin attempt received server side, credentials : " + switcher._un + ", " + switcher._pwd);
                                check = CheckCredentials(switcher);
                                bf.Serialize(comm.GetStream(), check);
                            }
                            Console.WriteLine("User is logged in");
                            UsersDisplay();
                            while (logout.CompareTo("logout") != 0)
                            {
                                Console.WriteLine("Waiting for logout");
                                logout = (string)bf.Deserialize(comm.GetStream());
                            }
                            //Console.WriteLine("Logging out user AAAAAAAAAAAAAAAAA " + switcher._un);
                            Logout(2, switcher._un);
                            Console.WriteLine("User logged out, stream content : " + logout);
                            //UsersDisplay();
                            break;

                        default: 
                            Console.WriteLine("SOMETHING BROKE AAAAAAAAAAAAAA");
                            break;
                    }
                }
            }
        }
    }
}
