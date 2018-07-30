namespace UberStrok.Realtime.Server.Game.Events
{
    public class PeerJoinedEvent : Event
    {
        public GamePeer Peer { get; set; }
        public GameActor Actor { get; set; }
    }
}
