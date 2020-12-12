using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server bob = new Server(1111);
            bob.Start();
        }
    }
}
