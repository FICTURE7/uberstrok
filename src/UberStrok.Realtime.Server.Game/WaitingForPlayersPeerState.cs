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
