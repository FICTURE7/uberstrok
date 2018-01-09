using System.Collections.Generic;
using System.Threading;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameManager
    {
        public GameManager()
        {
            _rooms = new Dictionary<int, Room>();
        }

        // Next room ID.
        private int _roomId = 0;
        private Dictionary<int, Room> _rooms;

        public Dictionary<int, Room> Rooms => _rooms;

        public void AddRoom(GameRoomDataView roomData, string password)
        {
            // Set those to 0, so the client knows there is not level restriction.
            if (roomData.LevelMin == 1 && roomData.LevelMax == 80)
            {
                roomData.LevelMin = 0;
                roomData.LevelMax = 0;
            }

            roomData.IsPasswordProtected = password != string.Empty;
            roomData.IsPermanentGame = false;
            roomData.Number = Interlocked.Increment(ref _roomId);

            Rooms.Add(roomData.Number, new Room(roomData, password));
        }

        public class Room
        {
            public Room(GameRoomDataView roomData, string password)
            {
                _roomData = roomData;
                _password = password;
            }

            private readonly GameRoomDataView _roomData;
            private readonly string _password;

            public GameRoomDataView Data => _roomData;
            public string Password => _password;
        }
    }
}
