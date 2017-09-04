using System;

namespace UbzStuff.Realtime.Server.Comm.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var commPeer = new TestCommPeer();
            if (!commPeer.Authenticate())
                return;

            commPeer.Connect("127.0.0.1:5055");

            Console.ReadLine();
        }
    }
}
