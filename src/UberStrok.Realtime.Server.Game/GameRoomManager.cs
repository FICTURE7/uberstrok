using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Game.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomManager : IEnumerable<GameRoom>
    {
        private int _roomId;
        private readonly ConcurrentDictionary<int, GameRoom> _rooms;

        public GameRoomManager()
        {
            _rooms = new ConcurrentDictionary<int, GameRoom>();
        }

        public int Count => _rooms.Count;

        public GameRoom Get(int roomId)
        {
            var room = default(GameRoom);
            _rooms.TryGetValue(roomId, out room);
            return room;
        }

        public void Remove(int roomId)
        {
            var room = default(GameRoom);
            _rooms.TryRemove(roomId, out room);
        }

        public GameRoom Create(GameRoomDataView data, string password)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            /* Set those to 0, so the client knows there is no level restriction. */
            if (data.LevelMin == 1 && data.LevelMax == 80)
            {
                data.LevelMin = 0;
                data.LevelMax = 0;
            }

            var room = default(GameRoom);
            switch (data.GameMode)
            {
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

        public IEnumerator<GameRoom> GetEnumerator()
        {
            return _rooms.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rooms.Values.GetEnumerator();
        }
    }
}
