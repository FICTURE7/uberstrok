using System;
using UbzStuff.Realtime.Client;

namespace UbzStuff.Realtime.Server.Comm.Tests
{
    public class TestCommPeer : BaseCommPeer
    {
        public TestCommPeer()
        {
            _lobby = new TestLobbyRoom(this);
        }

        public TestLobbyRoom Lobby => _lobby;
        private readonly TestLobbyRoom _lobby;

        protected override void OnLobbyEntered()
        {
            Console.WriteLine("We in the lobby boi!");
        }

        protected override void OnConnect(string endPoint)
        {
            Operations.SendAuthenticationRequest("sup", "anothershit");
        }

        protected override void OnError(string message)
        {
            // Space
        }
    }
}
