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
            Peer.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);
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
