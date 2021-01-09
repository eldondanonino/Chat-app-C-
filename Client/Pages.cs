using System;
using System.Net.Sockets;

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
                Console.Clear();
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
            return a;
        }
        public static int Menu()
        {
            int a;
            do
            {
                Console.Clear();

                Console.WriteLine("____________\nMain Page\n____________\n\n1 : Access topics\n2 : Access Private Messages\n0 : Logout\n");
                a = Int32.Parse(Console.ReadLine());
            } while (a < 0 || a > 2);
            return a;
        }
        //The main page for Client, gives access to the functionnalities of the app
        public static int Mainpage(TcpClient comm)
        {
            int a = Menu();
            switch (a)
            {
                case 1:
                    TopicClient.TopicNavigation(comm); //Go to topics
                    break;
                case 2:
                    PMClient.PMNavigation(comm); //Go to private messages
                    break;
                case 0:
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
