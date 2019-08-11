using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UberStrok.Core
{
    public class LoopScheduler : ILoopScheduler, IDisposable
    {
        private static int s_id = 0;

        private bool _started;
        private bool _disposed;

        private float _lag;
        private float _load;
        private int _loadTick;

        private readonly ManualResetEventSlim _pauseHandle;

        private readonly Thread _thread;
        private readonly List<ILoop> _loops;
        private readonly Stopwatch _stopwatch;

        public float TickInterval { get; }
        public float TickRate { get; }
        public IReadOnlyCollection<ILoop> Loops { get; }

        public bool IsPaused => !_pauseHandle.IsSet;

        public LoopScheduler(float tickRate)
        {
            if (tickRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(tickRate), "Tick rate cannot be less or equal to 0.");

            _pauseHandle = new ManualResetEventSlim(true);

            _started = false;
            _stopwatch = new Stopwatch();
            _loops = new List<ILoop>();
            _thread = new Thread(DoScheduling) { Name = "LoopScheduler-thread-" + Interlocked.Increment(ref s_id) };

            TickRate = tickRate;
            TickInterval = 1000 / tickRate;

            Loops = _loops.AsReadOnly();
        }

        public void Schedule(ILoop loop)
        {
            ThrowIfDisposed();

            if (loop == null)
                throw new ArgumentNullException(nameof(loop));

            /* Setup loop for scheduling. */
            loop.Setup();

            bool wasPaused = IsPaused;
            if (!wasPaused)
                PauseInternal();

            lock (_loops)
            {
                if (!_loops.Contains(loop))
                    _loops.Add(loop);
                else
                    throw new InvalidOperationException("Already scheduling the specified ILoop instance.");
            }

            if (!wasPaused)
                ResumeInternal();
        }

        public bool Unschedule(ILoop loop)
        {
            ThrowIfDisposed();

            if (loop == null)
                throw new ArgumentNullException(nameof(loop));

            bool result = false;

            /* Teardown loop for being unscheduled. */
            loop.Teardown();

            bool wasPaused = IsPaused;
            if (!wasPaused)
                PauseInternal();

            lock (_loops)
                result = _loops.Remove(loop);

            if (!wasPaused)
                ResumeInternal();

            return result;
        }

        public void Start()
        {
            ThrowIfDisposed();

            if (_started)
                throw new InvalidOperationException("LoopScheduler already started.");

            _started = true;
            _thread.Start();
        }

        public void Stop()
        {
            ThrowIfDisposed();

            if (!_started)
                throw new InvalidOperationException("LoopScheduler not started.");

            Dispose();
        }

        public void Pause()
        {
            ThrowIfDisposed();
            PauseInternal();
        }

        public void Resume()
        {
            ThrowIfDisposed();

            _lag = 0f;
            ResumeInternal();
        }

        public float GetLoad()
        {
            ThrowIfDisposed();
            return (!IsPaused ? _load / Math.Max(_loadTick, 1) : 0) + Loops.Count;
        }

        private void DoScheduling()
        {
            int interval = (int)Math.Ceiling(TickInterval);
            int loadInterval = (int)Math.Ceiling(TickRate * 3);

            try
            {
                while (_started)
                {
                    _pauseHandle.Wait();
                    _stopwatch.Start();

                    for (int i = 0; i < _loops.Count; i++)
                        _loops[i].Tick();

                    _loadTick++;

                    Thread.Sleep(interval);

                    /* 
                     * Stop stopwatch after Thread.Sleep to measure inaccuracy
                     * in sleep call as well.
                     */
                    _stopwatch.Stop();

                    var elapsed = (float)_stopwatch.Elapsed.TotalMilliseconds;

                    _stopwatch.Restart();
                    _lag += elapsed;

                    if (_loadTick < loadInterval)
                    {
                        _load += elapsed - TickInterval;
                    }
                    else
                    {
                        /* Reset load measurement every loadInterval. */
                        _load = 0;
                        _loadTick = 0;
                    }

                    _stopwatch.Stop();
                }
            }
            catch (ThreadAbortException)
            {
                /* Space */
            }
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
            {
                _started = false;

                /* 
                 * Give the thread a chance to spin a couple of times to shut
                 * down gracefully then kill if it does not.
                 */

                ResumeInternal();

                if (!_thread.Join(Math.Min((int)(TickInterval * 3), 300)))
                    _thread.Abort();

                foreach (var loop in _loops)
                    loop.Teardown();

                _loops.Clear();
                _pauseHandle.Dispose();
            }

            _disposed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PauseInternal()
            => _pauseHandle.Reset();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResumeInternal()
            => _pauseHandle.Set();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
        }
    }
}
