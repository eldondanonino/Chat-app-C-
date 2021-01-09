namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client Robert; //this is our client robert :)
            Robert = new Client("127.0.0.1", 1111);
            Robert.Connect();
        }
    }
}
