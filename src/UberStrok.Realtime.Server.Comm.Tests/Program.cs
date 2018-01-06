using System;
using System.Collections.Generic;

namespace UberStrok.Realtime.Server.Comm.Tests
{
    public static class Program
    {
        public static List<TestCommPeer> Peers { get; set; }

        public static void Main(string[] args)
        {
            Peers = new List<TestCommPeer>();
            for (int i = 0; i < 19; i++)
            {
                var peer = new TestCommPeer();
                if (!peer.Authenticate())
                    continue;

                peer.Connect("127.0.0.1:5055");
                Peers.Add(peer);
            }

            Console.ReadLine();
        }
    }
}
