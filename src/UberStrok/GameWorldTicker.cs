using System;
using System.Collections.Generic;

namespace UberStrok
{
    public class GameWorldTicker : IDisposable
    {
        private bool _disposed;

        private bool _started;
        private readonly Loop _loop;
        private readonly List<GameWorld> _games;

        public GameWorldTicker(int tps)
        {
            if (tps < 0)
                throw new ArgumentOutOfRangeException(nameof(tps), "Tick rate cannot be less than 0.");

            _loop = new Loop(tps);
            _games = new List<GameWorld>(16);
        }

        public Loop Loop => _loop;
        public IReadOnlyCollection<GameWorld> Games => _games;

        public event EventHandler<ExceptionEventArgs> Exception;

        public void Start()
        {
            _loop.Start(HandleLoop, HandleLoopException);
        }

        public void Stop()
        {
            _loop.Stop();
        }

        public void Add(GameWorld game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            lock (_games)
            {
                if (_games.Contains(game))
                    throw new InvalidOperationException("Game instances in a GameTicker must be unique.");

                _games.Add(game);
            }
        }

        public bool Remove(GameWorld game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            lock (_games)
                return _games.Remove(game);
        }

        private void HandleLoop()
        {
            /* Tick through all Game instances in this Game Ticker instance. */
            for (int i = 0; i < _games.Count; i++)
            {
                try { _games[i].DoTick(); }
                catch (Exception ex)
                {
                    OnException(ex);
                }
            }
        }

        private void HandleLoopException(Exception ex)
        {
            OnException(ex);
        }

        protected virtual void OnException(Exception ex)
        {
            Exception?.Invoke(this, new ExceptionEventArgs(ex));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _loop.Dispose();

            _disposed = true;
        }

        public class ExceptionEventArgs : EventArgs
        {
            private readonly Exception _ex;

            internal ExceptionEventArgs(Exception ex)
            {
                _ex = ex;
            }

            public Exception Exception => _ex;
        }
    }
}
