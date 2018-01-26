using UberStrok.Realtime.Server.Game.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class PlayingPeerState : PeerState
    {
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

            /* Sync the power ups to the server side. */
            Peer.Events.Game.SendSetPowerUpState(Room.PowerUps.Respawning);
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
            // Space
        }
    }
}
