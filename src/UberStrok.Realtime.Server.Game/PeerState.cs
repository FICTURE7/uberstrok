using log4net;
using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class PeerState : State
    {
        public PeerState(GamePeer peer)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
            Log = LogManager.GetLogger(GetType().Name);
        }

        protected ILog Log { get; }
        protected GamePeer Peer { get; }
        protected BaseGameRoom Room => Peer.Room;

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
