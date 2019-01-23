using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomManager : IEnumerable<BaseGameRoom>
    {
        private int _roomId;
        private readonly ConcurrentDictionary<int, BaseGameRoom> _rooms;

        public GameRoomManager()
        {
            _rooms = new ConcurrentDictionary<int, BaseGameRoom>();
        }

        public int Count => _rooms.Count;

        public BaseGameRoom Get(int roomId)
        {
            var room = default(BaseGameRoom);
            _rooms.TryGetValue(roomId, out room);
            return room;
        }

        public void Remove(int roomId)
        {
            var room = default(BaseGameRoom);
            _rooms.TryRemove(roomId, out room);
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
                case GameModeType.DeathMatch:
                    room = new DeathMatchGameRoom(data);
                    break;

                case GameModeType.TeamDeathMatch:
                    room = new TeamDeathMatchGameRoom(data);
                    break;

                default:
                    throw new NotSupportedException();
            }

            room.Number = Interlocked.Increment(ref _roomId);
            room.Password = password;

            /* Should never really happen */
            if (!_rooms.TryAdd(room.Number, room))
                throw new Exception("Already contains a game room with the specified room ID.");

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
