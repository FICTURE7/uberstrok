using System;
using UberStrok.Realtime.Server.Game.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class KilledPeerState : PeerState
    {
        private double _countdown;
        private double _countdownOld;
        private DateTime _countdownEndTime;

        public KilledPeerState(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnEnter()
        {
            var now = DateTime.UtcNow;

            /* TODO: Allow user to set the countdown timer duration in a config or something. */
            _countdown = 3 * 1000;
            _countdownEndTime = now.AddSeconds(_countdown);
        }

        public override void OnResume()
        {
            // Space
        }

        public override void OnExit()
        {
            // Space
        }

        public override void OnUpdate()
        {
            var now = DateTime.UtcNow;

            _countdownOld = _countdown;
            _countdown -= Room.Loop.DeltaTime.TotalMilliseconds;

            var countdownOldRound = (int)Math.Round(_countdownOld / 1000);
            var countdownRound = (int)Math.Round(_countdown / 1000);

            if (countdownOldRound > -1 && countdownOldRound > countdownRound)
                Peer.Events.Game.SendPlayerRespawnCountdown(countdownOldRound);
        }
    }
}
