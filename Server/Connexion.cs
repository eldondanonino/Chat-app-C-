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
    class Connexion
    {

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

                    Console.WriteLine("---Setting all users as logged in---\n");
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
                    Console.WriteLine("Logging in user " + username + "\n");
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

                    Console.WriteLine("---Setting all users as logged out---\n");
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
                    Console.WriteLine("Logging out user " + username + "\n");
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
            Files.UsersDisplay();
        }

        //Checking if the submitted credentials are valid for a login
        public static int CheckCredentials(LoginRequest cred)
        {

            int checker = 0;
            BinaryFormatter bf = new BinaryFormatter();
            List<Username> users = Files.StartupUsernames();
            foreach (Username u in users)
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
            switch (checker)
            {
                case -1:
                    Console.WriteLine("User is already logged in\n");
                    break;
                case 0:
                    Console.WriteLine("Unable to find matching credentials\n");
                    break;
                case 1:
                    Login(2, cred._un);
                    break;

            }
            return checker;
        }

        //Waiting for the login request
        public static LoginRequest ReceiveLogin(TcpClient comm)
        {
            LoginRequest switcher = new LoginRequest("", "");
            BinaryFormatter bf = new BinaryFormatter();
            int check = 0;
            Console.WriteLine("---Waiting for the login request---");
            while (check == 0 || check == -1)
            {
                switcher = (LoginRequest)bf.Deserialize(comm.GetStream());
                Console.WriteLine("\nLogin attempt received server side, credentials : " + switcher._un + ", " + switcher._pwd);
                check = CheckCredentials(switcher);
                bf.Serialize(comm.GetStream(), check);
            }
            Files.UsersDisplay();
            return switcher;
        }

        //waiting for the creation request
        public static void ReceiveCreationRequest(TcpClient comm)
        {
            LoginRequest switcher = new LoginRequest("", "");
            string user, pwd;
            BinaryFormatter bf = new BinaryFormatter();
            Console.WriteLine("---Waiting for the creation request---\n");
            switcher = (LoginRequest)bf.Deserialize(comm.GetStream());

            user = switcher._un;
            pwd = switcher._pwd;
            Console.WriteLine("User Creation initiated server side! received : " + user + "\nPassword : " + pwd);
            Files.UserCreation(user, pwd, Files.StartupUsernames(), comm);
            //UsersDisplay();
        }


    }
}
