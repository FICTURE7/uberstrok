using System;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoomManager
    {
        public LobbyRoom Global { get; }

        public LobbyRoomManager()
        {
            Global = new LobbyRoom();
        }

        public void Create()
        {
            throw new NotImplementedException();
        }
    }
}
