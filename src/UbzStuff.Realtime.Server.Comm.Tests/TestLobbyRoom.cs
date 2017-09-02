using System;
using System.Collections.Generic;
using UbzStuff.Core.Views;
using UbzStuff.Realtime.Client;

namespace UbzStuff.Realtime.Server.Comm.Tests
{
    public class TestLobbyRoom : BaseLobbyRoom
    {
        public TestLobbyRoom(BasePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnFullPlayerListUpdate(List<CommActorInfoView> players)
        {
            foreach (var player in players)
            {
                Console.WriteLine(player.PlayerName);
            }
        }
    }
}
