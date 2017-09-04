using System;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;
using UbzStuff.Realtime.Client;
using UbzStuff.WebServices.Client;

namespace UbzStuff.Realtime.Server.Comm.Tests
{
    public class TestCommPeer : BaseCommPeer
    {
        private static readonly Random s_random = new Random();

        public TestCommPeer()
        {
            _lobby = new TestLobbyRoom(this);

            SteamId = s_random.Next().ToString();
            Name = "Realtime.Comm.Tests-" + SteamId;
        }

        public string SteamId { get; set; }
        public string Name { get; set; }

        public PublicProfileView Profile => _member.PublicProfile;
        public TestLobbyRoom Lobby => _lobby;

        private string _authToken;
        private MemberView _member;
        private readonly TestLobbyRoom _lobby;

        public bool Authenticate()
        {
            // Create a new account on the web server.

            var client = new AuthenticationWebServiceClient("http://localhost/2.0");
            var loginResult = client.LoginSteam(SteamId, string.Empty, string.Empty);
            if (loginResult.MemberAuthenticationResult != MemberAuthenticationResult.Ok)
            {
                Console.Error.WriteLine("[TestCommPeer] WS -> Failed to login through the web server.");
                return false;
            }

            _authToken = loginResult.AuthToken;
            _member = loginResult.MemberView;

            // Complete the account, so we can set the user name.
            var completeResult = client.CompleteAccount(Profile.Cmid, Name, ChannelType.Steam, "en_US", string.Empty);
            if (completeResult.Result != 1)
            {
                Console.Error.WriteLine("[TestCommPeer] WS -> Failed to complete account through the web server.");
                return false;
            }

            Console.WriteLine("[TestCommPeer] WS -> Login success!");
            return true;
        }

        protected override void OnLobbyEntered()
        {
            Console.WriteLine("[TestCommPeer] RT -> In lobby");
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
