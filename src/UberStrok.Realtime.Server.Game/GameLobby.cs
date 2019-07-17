using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class GameLobby : IDisposable
    {
        private bool _disposed;

        private readonly Loop _loop;
        private readonly LoopScheduler _loopScheduler;

        private readonly List<GamePeer> _peers;
        private readonly List<GamePeer> _unlockedPeers;

        protected ILog Log { get; }

        public GameRoomManager Rooms { get; }
        public ICollection<GamePeer> Peers { get; }

        public GameLobby()
        {
            _loop = new Loop(OnTick, OnTickError);
            _loopScheduler = new LoopScheduler(15);

            _peers = new List<GamePeer>();
            _unlockedPeers = new List<GamePeer>();

            Log = LogManager.GetLogger(GetType().Name);
            Rooms = new GameRoomManager();
            Peers = _unlockedPeers.AsReadOnly();

            _loopScheduler.Schedule(_loop);
            _loopScheduler.Start();
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

        public void Dispose()
        {
            if (_disposed)
                return;

            _loopScheduler.Dispose();
            _disposed = true;
        }

        private void OnTick()
        {
            _unlockedPeers.Clear();

            /* Best effort lock. */
            lock (_peers)
                _unlockedPeers.AddRange(_peers);

            /* Tick room manager. */
            Rooms.Tick();
        }

        private void OnTickError(Exception ex)
        {
            Log.Error("Failed to tick GameLobby loop.", ex);
        }
    }
}
