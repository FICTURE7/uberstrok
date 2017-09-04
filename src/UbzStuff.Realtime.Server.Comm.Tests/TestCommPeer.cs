using System;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;
using UbzStuff.Realtime.Client;
using UbzStuff.WebServices.Client;

namespace UbzStuff.Realtime.Server.Comm.Tests
{
    public class TestCommPeer : BaseCommPeer
    {
        public TestCommPeer()
        {
            _lobby = new TestLobbyRoom(this);
        }

        public TestLobbyRoom Lobby => _lobby;

        private string _authToken;
        private MemberView _member;
        private readonly TestLobbyRoom _lobby;

        public bool Authenticate()
        {
            // Create a new account on the web server.
            var rnd = new Random();
            var steamId = rnd.Next().ToString();
            var name = "Realtime.Comm.Tests-" + steamId;

            var client = new AuthenticationWebServiceClient("http://localhost/2.0");
            var loginResult = client.LoginSteam(steamId, string.Empty, string.Empty);
            if (loginResult.MemberAuthenticationResult != MemberAuthenticationResult.Ok)
            {
                Console.Error.WriteLine("Failed to login through the web server.");
                return false;
            }

            _authToken = loginResult.AuthToken;
            _member = loginResult.MemberView;

            // Complete the account, so we can set the user name.
            var completeResult = client.CompleteAccount(_member.PublicProfile.Cmid, name, ChannelType.Steam, "en_US", string.Empty);
            if (completeResult.Result != 1)
            {
                Console.Error.WriteLine("Failed to complete account through the web server.");
                return false;
            }

            Console.WriteLine("Login success!");
            return true;
        }

        protected override void OnLobbyEntered()
        {
            Console.WriteLine("[TestCommPeer] -> In lobby");
        }

        protected override void OnConnect(string endPoint)
        {
            Operations.SendAuthenticationRequest(_authToken, string.Empty);
        }

        protected override void OnError(string message)
        {
            // Space
        }
    }
}
