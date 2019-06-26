using System.Diagnostics;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class KilledPeerState : PeerState
    {
        private Countdown _respawnCountdown;
        private Countdown _disconnectCountdown;

        public KilledPeerState(GamePeer peer) : base(peer)
        {
            /* Space */
        }

        public sealed override void OnEnter()
        {
            _respawnCountdown = new Countdown(Room.Loop, 5, 0);
            _respawnCountdown.Counted += OnRespawnCounted;
            _respawnCountdown.Completed += OnRespawnCompleted;

            _disconnectCountdown = new Countdown(Room.Loop, 60, 0);
            _disconnectCountdown.Counted += OnDisconnectCounted;
            _disconnectCountdown.Completed += OnDisconnectCompleted;

            _respawnCountdown.Start();
        }

        public sealed override void OnUpdate()
        {
            _respawnCountdown.Update();
            _disconnectCountdown.Update();
        }

        public sealed override void OnResume()
        {
            Debug.Fail("KilledPeerState should never be resumed");
        }

        private void OnRespawnCounted(int count)
        {
            Peer.Events.Game.SendPlayerRespawnCountdown(count);
        }

        private void OnRespawnCompleted()
        {
            /* 
             * Start disconnect countdown after the respawn countdown is done.
             */
            _disconnectCountdown.Start();
        }

        private void OnDisconnectCounted(int count)
        {
            /* Start sending count after 10th countdown. */
            if (count <= 10)
                Peer.Events.Game.SendDisconnectCountdown(count);
        }

        private void OnDisconnectCompleted()
        {
            Peer.Disconnect();
        }
    }
}
