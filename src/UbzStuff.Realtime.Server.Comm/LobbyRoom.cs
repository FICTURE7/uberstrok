namespace UbzStuff.Realtime.Server.Comm
{
    public class LobbyRoom
    {
        public LobbyRoom(CommPeer peer)
        {
            _events = new LobbyRoomEvents(peer);
        }

        private readonly LobbyRoomEvents _events;
        public LobbyRoomEvents Events => _events;
    }
}
