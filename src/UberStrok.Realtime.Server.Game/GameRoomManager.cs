using System;
using System.Collections;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomManager : IEnumerable<BaseGameRoom>
    {
        public GameRoomManager()
        {
            _rooms = new Dictionary<int, BaseGameRoom>();
        }

        private int _roomId;
        private readonly Dictionary<int, BaseGameRoom> _rooms;

        public int Count => _rooms.Count;

        public BaseGameRoom Get(int roomId)
        {
            var room = default(BaseGameRoom);
            _rooms.TryGetValue(roomId, out room);
            return room;
        }

        public void Remove(int roomId)
        {
            _rooms.Remove(roomId);
        }

        public BaseGameRoom Create(GameRoomDataView data, string password)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            /* Set those to 0, so the client knows there is no level restriction. */
            if (data.LevelMin == 1 && data.LevelMax == 80)
            {
                data.LevelMin = 0;
                data.LevelMax = 0;
            }

            var room = default(BaseGameRoom);
            switch (data.GameMode)
            {
                case GameModeType.TeamDeathMatch:
                    room = new TeamDeathMatchGameRoom(data);
                    break;

                default:
                    throw new NotSupportedException();
            }

            room.Id = ++_roomId;
            room.Password = password;

            _rooms.Add(room.Id, room);
            return room;
        }

        public IEnumerator<BaseGameRoom> GetEnumerator()
        {
            return _rooms.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rooms.Values.GetEnumerator();
        }
    }
}
