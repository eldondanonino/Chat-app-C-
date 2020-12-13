using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;
namespace Client
{
    class Connexion
    {

        //Client connection to the server

        //Request to the server a creation of user
        public static bool RequestCreation(TcpClient comm)
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
                //Console.WriteLine("Read password : " + un);
                if (un.Length > 32)
                    Console.WriteLine("Your password is too long, 32 max characters");
            } while (un.Length > 32);

            string pass = un;
            LoginRequest request = new LoginRequest(user, pass);
            //Console.WriteLine("request._pwd : " + request._pwd);
            //Console.WriteLine("\nSending the signup request");
            bf.Serialize(comm.GetStream(), request);
            //Thread.Sleep(150); //tiny delay just to be sure
            b = (bool)bf.Deserialize(comm.GetStream());
            //Console.WriteLine("Boolean status : " + b);
            return b;
        }

        //Logging in the client
        public static void Login(TcpClient comm)
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
                        Console.WriteLine("\nWrong Username and/or password, try again\n");
                    if (logged == -1)
                        Console.WriteLine("\nYou are already logged in! Check on other clients\n");

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
        public static void Logout(string username, TcpClient comm)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(comm.GetStream(), 0);
            Console.WriteLine("You are now logged out of the server! \n");
            //comm.Close(); //How to close the tcp client without nuking the server???

        }


    }
}
