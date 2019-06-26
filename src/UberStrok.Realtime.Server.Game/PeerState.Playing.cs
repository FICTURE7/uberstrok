namespace UberStrok.Realtime.Server.Game
{
    public class PlayingPeerState : PeerState
    {
        public double _timer;

        public PlayingPeerState(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnEnter()
        {
            /* 
                MatchStart event changes the match state of the client to match running,
                which in turn changes the player state to playing.

                The client does not care about the roundNumber apparently (in TeamDeathMatch atleast).
             */
            Peer.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);
            /*
                This is to reset the top scoreboard to not display "STARTS IN".
             */
            Peer.Events.Game.SendUpdateRoundScore(Room.RoundNumber, 0, 0);
        }

        public override void OnResume()
        {
            _timer = 0;
        }

        public override void OnUpdate()
        {
            double dt = Peer.Room.Loop.DeltaTime.TotalMilliseconds;
            if (Peer.Actor.Info.Health > 100 || Peer.Actor.Info.ArmorPoints > Peer.Actor.Info.ArmorPointCapacity)
                _timer += dt;
            else
                _timer = 0;

            if (_timer > 1000)
            {
                if (Peer.Actor.Info.Health > 100)
                    Peer.Actor.Info.Health -= 1;
                if (Peer.Actor.Info.ArmorPoints > Peer.Actor.Info.ArmorPointCapacity)
                    Peer.Actor.Info.ArmorPoints -= 1;

                _timer -= 1000;
            }
        }
    }
}
