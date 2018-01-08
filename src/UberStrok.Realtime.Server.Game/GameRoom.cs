namespace UberStrok.Realtime.Server.Game
{
    public class GameRoom
    {
        public GameRoom(GamePeer peer, GameManager.Room room)
        {
            _peer = peer;
            _room = room;
            _events = new GameRoomEvents(peer);

            _peer.AddOpHandler(new GameRoomOperationHandler(peer));
        }

        private readonly GameManager.Room _room;
        private readonly GamePeer _peer;
        private readonly GameRoomEvents _events;

        public GameManager.Room Room => _room;
        public GameRoomEvents Events => _events;
    }
}
