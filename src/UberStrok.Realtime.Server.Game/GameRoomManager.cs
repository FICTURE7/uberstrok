using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

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
            if (_rooms.TryGetValue(roomId, out GameRoom room) && room.Actors.Count == 0)
            {
                Remove(roomId);
                return null;
            }

            return room;
        }

        public void Remove(int roomId)
        {
            if (_rooms.TryRemove(roomId, out GameRoom room))
                room.Dispose();
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

            GameRoom room;
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

            room.RoomId = Interlocked.Increment(ref _roomId);
            room.Password = password;

            /* Should never really happen */
            if (!_rooms.TryAdd(room.RoomId, room))
            {
                room.Dispose();
                throw new Exception("Unable to add game room to game room list");
            }

            return room;
        }

        public IEnumerator<GameRoom> GetEnumerator()
        {
            var emptyRooms = new List<GameRoom>();

            foreach (var kv in _rooms)
            {
                var room = kv.Value;
                /* Filter out empty rooms. */
                if (room.Actors.Count == 0)
                    emptyRooms.Add(room);
                else
                    yield return room;
            }

            foreach (var room in emptyRooms)
                Remove(room.RoomId);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
