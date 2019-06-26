namespace UberStrok.Realtime.Server.Game
{
    public class WaitingForPlayersPeerState : PeerState
    {
        public WaitingForPlayersPeerState(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnEnter()
        {
            Peer.Events.Game.SendWaitingForPlayer();
        }
    }
}
