using System;
using System.Collections.Generic;
using System.Threading;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameManager
    {
        public static GameManager Instance => s_instance;
        private static GameManager s_instance = new GameManager();

        public GameManager()
        {
            if (s_instance != null)
                throw new Exception();

            Rooms = new Dictionary<int, Room>();
        }

        private int _roomId = 0;

        public Dictionary<int, Room> Rooms { get; set; }

        public void AddRoom(GameRoomDataView roomData, string password)
        {
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
