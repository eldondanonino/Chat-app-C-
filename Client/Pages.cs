using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Shared;

namespace Client
{
    class Pages
    {

        public static int Firstpage(TcpClient comm)
        {
            int a = 0;
            bool b = false; //indicator of the success of creation
            while (a != 2) //while not logged in
            {
                Console.WriteLine("\nDo you want to create a user?");
                do
                {
                    Console.WriteLine("\n1 : Yes    2 : No, I already have my user \n");
                    a = Int32.Parse(Console.ReadLine());
                } while (a != 1 && a != 2);
                switch (a)
                {
                    case 1:
                        b = Connexion.RequestCreation(comm);
                        if (b)
                            Console.WriteLine("\nCreation successful! Try to login now!");
                        else
                            Console.WriteLine("\nThis username is already taken, try something else");
                        break;
                    case 2:
                        Connexion.Login(comm);
                        break;
                }
            }
            Console.WriteLine("\n---You are now in the main page---");
            return a;
        }

        public static int Mainpage(TcpClient comm)
        {
            int a;
            do
            {
                Console.WriteLine("\n1 : access topics\n2 : Logout\n");
                a = Int32.Parse(Console.ReadLine());
            } while (a != 1 && a != 2);
            switch (a)
            {
                case 1:
                    TopicClient.TopicNavigation(comm);
                    break;
                case 2:
                    Console.WriteLine("\nLogging out...");
                    Connexion.Logout("logout", comm);
                    //Console.WriteLine("You are now logged out!\n");
                    a = 0;
                    break;
            }
            return a;
        }

    }
}
