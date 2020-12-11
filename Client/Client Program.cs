using System;
using System.IO;

//TODO : login !
//       logout
//       

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client Robert;
            Robert = new Client("127.0.0.1", 1111, "test");
            Robert.Connect();
        }
    }
}
