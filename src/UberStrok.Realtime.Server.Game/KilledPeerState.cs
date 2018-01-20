using System;

namespace UberStrok.Realtime.Server.Game
{
    public class KilledPeerState : PeerState
    {
        private double _countdown;
        private double _countdownOld;
        private DateTime _countdownEndTime;

        /* TODO: Calculate delta time in the game loop. */
        private double _deltaTime;

        private DateTime _lastUpdate;

        public KilledPeerState(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnEnter()
        {
            Peer.Killed += OnKilled;
        }

        public override void OnExit()
        {
            Peer.Killed -= OnKilled;
        }

        public override void OnUpdate()
        {
            var now = DateTime.UtcNow;

            _countdownOld = _countdown;

            _deltaTime = (now - _lastUpdate).TotalMilliseconds;
            _countdown -= _deltaTime;

            var countdownOldRound = (int)Math.Round(_countdownOld / 1000);
            var countdownRound = (int)Math.Round(_countdown / 1000);
            if (countdownOldRound < 0)
            {
                // Do things?
            }
            else if (countdownOldRound > countdownRound)
                Peer.Events.Game.SendPlayerRespawnCountdown(countdownOldRound);

            _lastUpdate = DateTime.UtcNow;
        }

        private void OnKilled(object sender, PlayerKilledEventArgs e)
        {
            var now = DateTime.UtcNow;

            /* TODO: Allow user to set the countdown timer duration in a config or something. */
            _countdown = 3 * 1000;
            _countdownEndTime = now.AddSeconds(_countdown);
            _lastUpdate = now;

            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerKilled(e.AttackerCmid, e.VictimCmid, e.ItemClass, e.Damage, e.Part, e.Direction);
        }
    }
}
