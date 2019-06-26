namespace UberStrok.Realtime.Server.Game
{
    public class CountdownPeerState : PeerState
    {
        public CountdownPeerState(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnEnter()
        {
            Peer.Events.Game.SendPrepareNextRound();
        }
    }
}
