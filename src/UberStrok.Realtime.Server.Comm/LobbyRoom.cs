namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoom
    {
        public LobbyRoom(CommPeer peer)
        {
            _events = new LobbyRoomEvents(peer);

            peer.AddOperationHandler(new LobbyRoomOperationHandler());
        }

        public LobbyRoomEvents Events => _events;
        private readonly LobbyRoomEvents _events;
    }
}
