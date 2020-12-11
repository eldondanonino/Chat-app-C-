using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;

namespace Client
{
    class Client
    {
        private string hostname;
        private int port;
        private string name;
        private TcpClient comm;
        public Client(string h, int p, string s)
        {
            hostname = h;
            port = p;
            name = s;
        }

       /// <summary>
       /// En gros apres un logout si j'enleve la partie connecter le client au serveur, les serialisations se desync jsp pk
       /// ah si ça marche parceque c'est un tout nouveau client donc il a pas pu se desync, a fix
       /// 
       /// TODO : comment deco un tcp client
       /// </summary>

        //main method for client manipulations
        public void Connect()
        {
            int a = 0;
            bool firstTime = true, b;
            while (true)
            {
                b = false;
                //while (firstTime) //checking if it is the first time the while true is run through
               // {
                    do
                    {
                        Console.WriteLine("1 to connect to server, 2 to leave");
                        a = Int32.Parse(Console.ReadLine());
                    } while (a != 1 && a != 2);
                    switch (a)
                    {
                        case 1:
                            start();
                            break;
                        case 2:
                            System.Environment.Exit(1);
                            break;
                    }
                    firstTime = false;
               // } 
                while (a != 2)
                {
                    Console.WriteLine("Do you want to create a user?");
                    do
                    {
                        Console.WriteLine("1 : Yes    2 : No, I already have my user ");
                        a = Int32.Parse(Console.ReadLine());
                    } while (a != 1 && a != 2);
                    switch (a)
                    {
                        case 1:
                            b = RequestCreation();
                            if (b) //if the creation is successful, consider it as a login
                            {
                                a = 2;
                                Console.WriteLine("Creation successful! Logging in...");
                            }
                            if (!b)
                                Console.WriteLine("This username is already taken, try something else");
                            break;
                        case 2:
                            Login();
                            break;
                    }
                }
                Console.WriteLine("\n---You are now in the main page---");
                do
                {
                    Console.WriteLine("Press 0 to logout");
                    a = Int32.Parse(Console.ReadLine());
                } while (a != 0);
                Console.WriteLine("Logging out...");
                Logout("logout");
               

            }
        }
        //Client connection to the server
        public void start()
        {
            comm = new TcpClient(hostname, port);
            Console.WriteLine("\nConnection established Client Side\n");
        }
        public bool RequestCreation()
        {
            BinaryFormatter bf = new BinaryFormatter();
            string un;
            bool b = false;
            bf.Serialize(comm.GetStream(), 1);
            Console.WriteLine("Creating a new user...");
            Console.WriteLine("Enter your username");

            do
            {
                un = Console.ReadLine();
                if (un.Length > 32)
                    Console.WriteLine("Your username is too long, 32 max characters");
            } while (un.Length > 32);
            string user = un;

            Console.WriteLine("Enter your password");
            do
            {
                un = Console.ReadLine();
                Console.WriteLine("Read password : " + un);
                if (un.Length > 32)
                    Console.WriteLine("Your password is too long, 32 max characters");
            } while (un.Length > 32);

            string pass = un;
            LoginRequest request = new LoginRequest(user, pass);
            //Console.WriteLine("request._pwd : " + request._pwd);
            Console.WriteLine("Sending the signup request");
            bf.Serialize(comm.GetStream(), request);
            //Thread.Sleep(150); //tiny delay just to be sure
            b = (bool)bf.Deserialize(comm.GetStream());
            //Console.WriteLine("Boolean status : " + b);
            return b;
        }

        //Logging in the client
        public void Login()
        {
            BinaryFormatter bf = new BinaryFormatter();
            string loginUn, loginPwd;
            int logged;
            bf.Serialize(comm.GetStream(), 2); //telling the server to initiate a login

            Console.WriteLine("\nPlease enter your username : ");
            loginUn = Console.ReadLine();
            Console.WriteLine("Please enter your password : ");
            loginPwd = Console.ReadLine();
            LoginRequest request = new LoginRequest(loginUn, loginPwd);
            bf.Serialize(comm.GetStream(), request);
            Thread.Sleep(500);
            logged = (int)bf.Deserialize(comm.GetStream());
            if (logged == 1)
            {
                Console.WriteLine("You are logged in!");
                return;
            }
            else
            {
                do
                {
                    
                    if (logged == 0)
                        Console.WriteLine("Wrong Username and/or password, try again");
                    if(logged == -1)
                        Console.WriteLine("You are already logged in! Check on other clients\n");

                    Console.WriteLine("Please enter your username : ");
                    loginUn = Console.ReadLine();
                    Console.WriteLine("Please enter your password : ");
                    loginPwd = Console.ReadLine();
                    request = new LoginRequest(loginUn, loginPwd);
                    bf.Serialize(comm.GetStream(), request);
                    Thread.Sleep(150);
                    logged = (int)bf.Deserialize(comm.GetStream());
                } while (logged != 1);
                Console.WriteLine("You are logged in!");
            }
        }

        //Logging out the client
        public void Logout(string username)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(comm.GetStream(), "logout");
            Console.WriteLine("You are now logged out of the server! \n");
        }

    }
}
