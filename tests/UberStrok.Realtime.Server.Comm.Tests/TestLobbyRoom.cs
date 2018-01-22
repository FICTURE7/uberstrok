using System;
using System.Collections.Generic;
using UberStrok.Core.Views;
using UberStrok.Realtime.Client;

namespace UberStrok.Realtime.Server.Comm.Tests
{
    public class TestLobbyRoom : BaseLobbyRoom
    {
        public TestLobbyRoom(BasePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnFullPlayerListUpdate(List<CommActorInfoView> players)
        {
            Console.WriteLine("-----------------");
            Console.WriteLine("Player List: ");
            foreach (var player in players)
                Console.WriteLine($"[{player.Cmid}] {player.PlayerName}");
            Console.WriteLine("-----------------");
        }

        public override void OnLobbyChatMessage(int cmid, string name, string message)
        {
            Console.WriteLine($"LOBBY: [{cmid}][{name}] {message}");

            if (!name.StartsWith("Realtime.Comm.Tests"))
                Operations.SendChatToAll($"Yooo mah bitch {name}");
        }
    }
}
