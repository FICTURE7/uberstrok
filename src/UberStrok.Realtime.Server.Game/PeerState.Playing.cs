using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class PlayingPeerState : PeerState
    {
        private Timer _overflowTimer;

        public PlayingPeerState(GamePeer peer) : base(peer)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            _overflowTimer = new Timer(Room.Loop, TimeSpan.FromSeconds(1));
            _overflowTimer.Tick += OnOverflowTick;

            /* 
             * MatchStart event changes the match state of the client to match
             * running, which in turn changes the player state to playing.
             *
             * The client does not care about the roundNumber apparently (in
             * TeamDeathMatch atleast).
             */
            Peer.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);
            /*
             * This is to reset the top scoreboard to not display "STARTS IN".
             */
            Peer.Events.Game.SendUpdateRoundScore(Room.RoundNumber, 0, 0);
        }

        public override void OnUpdate()
        {
            int healthCapacity = 100;
            int armorCapacity = Peer.Actor.Info.ArmorPointCapacity;

            _overflowTimer.IsEnabled = Peer.Actor.Info.Health > healthCapacity || Peer.Actor.Info.ArmorPoints > armorCapacity;
            _overflowTimer.Update();
        }

        public override void OnResume()
        {
            _overflowTimer.Reset();
        }

        private void OnOverflowTick()
        {
            int healthCapacity = 100;
            int armorCapacity = Peer.Actor.Info.ArmorPointCapacity;

            if (Peer.Actor.Info.Health > healthCapacity)
                Peer.Actor.Info.Health--;
            if (Peer.Actor.Info.ArmorPoints > armorCapacity)
                Peer.Actor.Info.ArmorPoints--;
        }
    }
}
