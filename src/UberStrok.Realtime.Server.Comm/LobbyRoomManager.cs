using log4net;
using System.Collections.Generic;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoomManager
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LobbyRoomManager));

        /* List of lobby rooms. */
        private readonly List<LobbyRoom> _rooms;

        public IReadOnlyList<LobbyRoom> Rooms => _rooms.AsReadOnly();

        public LobbyRoomManager()
        {
            _rooms = new List<LobbyRoom>(16);

            /* Create default initial lobby room. */
            CreateRoom();
        }

        /* Finds a room which is available. */
        public LobbyRoom FindAvailable()
        {
            /* 
                TODO: Implement multi-room handling and stuff. Throw everyone in 
                single room for now.
             */
            return _rooms[0];
        }

        private LobbyRoom CreateRoom()
        {
            _log.Info("Creating new LobbyRoom.");

            var room = new LobbyRoom();
            _rooms.Add(room);

            return room;
        }
    }
}
