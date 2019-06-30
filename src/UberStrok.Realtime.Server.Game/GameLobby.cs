using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class GameLobby
    {
        private readonly Loop _loop;
        private readonly List<GamePeer> _peers;
        private readonly List<GamePeer> _unlockedPeers;

        protected ILog Log { get; }

        public GameRoomManager Rooms { get; }
        public ICollection<GamePeer> Peers { get; }

        public GameLobby()
        {
            _loop = new Loop(15);
            _peers = new List<GamePeer>();
            _unlockedPeers = new List<GamePeer>();

            Log = LogManager.GetLogger(GetType().Name);
            Rooms = new GameRoomManager();
            Peers = _unlockedPeers;

            _loop.Start(OnTick, OnTickError);
        }

        public void Join(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            lock (_peers)
                _peers.Add(peer);
        }

        public void Leave(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            lock (_peers)
                _peers.Remove(peer);
        }

        private void OnTick()
        {
            _unlockedPeers.Clear();

            /* Best effort lock. */
            lock (_peers)
                _unlockedPeers.AddRange(_peers);

            Rooms.Tick();
        }

        private void OnTickError(Exception ex)
        {
            Log.Error("Failed to tick GameLobby loop.", ex);
        }
    }
}
