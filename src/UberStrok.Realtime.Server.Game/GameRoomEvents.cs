namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomEvents : BaseEventSender
    {
        public GameRoomEvents(GamePeer peer) : base(peer)
        {
            _peer = peer;
        }

        private GamePeer _peer;
    }
}
