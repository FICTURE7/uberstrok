using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class PeerState : State
    {
        private readonly GamePeer _peer;

        public PeerState(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peer = peer;
        }

        protected GamePeer Peer => _peer;
        protected BaseGameRoom Room => _peer.Room;

        public enum Id
        {
            None,
            Overview,
            WaitingForPlayers,
            Countdown,
            Playing,
            Killed
        }
    }
}
