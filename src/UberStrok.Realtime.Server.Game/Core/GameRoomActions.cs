using System;

namespace UberStrok.Realtime.Server.Game.Core
{
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
