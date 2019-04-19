using System;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    [Obsolete]
    public class GameRoomActions
    {
        private readonly BaseGameRoom _room;

        public GameRoomActions(BaseGameRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _room = room;
        }

        protected BaseGameRoom Room => _room;

        public void ChatMessage(GamePeer peer, string message, ChatContext chatCtx)
        {
            var cmid = peer.Actor.Cmid;
            var playerName = peer.Actor.PlayerName;
            var accessLevel = peer.Actor.AccessLevel;

            foreach (var otherPeer in Room.Peers)
            {
                if (otherPeer.Actor.Cmid != cmid)
                    otherPeer.Events.Game.SendChatMessage(cmid, playerName, message, accessLevel, chatCtx);
            }
        }

        public void MatchCountdown(int countdown)
        {

        }

        public void RespawnCountdown(GamePeer peer, int countdown)
        {

        }

        public void PlayerKilled(GamePeer peer)
        {

        }

        public void PlayerRespawned(GamePeer peer)
        {

        }

        public void PlayerJoined(GamePeer player)
        {

        }

        public void PlayerLeft(GamePeer player)
        {
            foreach (var peer in Room.Peers)
            {
                peer.Events.Game.SendPlayerLeftGame(player.Actor.Cmid);
                peer.KnownActors.Remove(player.Actor.Cmid);
            }
        }
    }
}
