using System;
using System.Threading;

namespace UberStrok.Realtime.Server
{
    public delegate void LoopHandler();
    public delegate void LoopExceptionHandler(Exception e);

    public class Loop : IDisposable
    {
        /* Figure out if we've been disposed. */
        private bool _disposed;

        /* Figure out if the loop has started. */
        private bool _started;

        /* Time at the start of the tick. */
        private DateTime _time;
        /* Time difference between the last tick. */
        private TimeSpan _deltaTime;

        /* Loop handler which is called every tick. */
        private LoopHandler _handler;
        /* Loop exception handler which is called when _handler throws an exception. */
        private LoopExceptionHandler _exceptionHandler;

        private readonly EventWaitHandle _pauseWaitHandle;
        /* Amount of time the thread needs to sleep in ms. */
        private readonly int _interval;
        /* Thread that is going to do the work. */
        private readonly Thread _thread;

        public Loop(int tickRate)
        {
            if (tickRate < 0)
                throw new ArgumentOutOfRangeException(nameof(tickRate), "Tick rate cannot be less than 0.");

            _pauseWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            _thread = new Thread(Work) { Name = "GameLoop-thread" };
            _interval = 1000 / tickRate;
        }

        public int Interval => _interval;
        public DateTime Time => _time;
        public TimeSpan DeltaTime => _deltaTime;

        public void Start(LoopHandler handler, LoopExceptionHandler exceptionHandler)
        {
            if (_started)
                throw new InvalidOperationException("Loop already started.");
            if (_handler != null || _exceptionHandler != null)
                throw new InvalidOperationException("Loop can be started only once.");

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (exceptionHandler == null)
                throw new ArgumentNullException(nameof(exceptionHandler));

            _started = true;

            _handler = handler;
            _exceptionHandler = exceptionHandler;

            _thread.Start();
        }

        public void Stop()
        {
            if (!_started)
                throw new InvalidOperationException("Loop not started.");

            _started = false;

            /* 
                Give the thread a chance to spin a couple of times to shutting down
                gracefully then kill if it does not.
             */
            if (!_thread.Join(Interval * 3))
                _thread.Abort();
        }

        public void Pause()
        {
            if (!_started)
                throw new InvalidOperationException("Loop not started");

            _pauseWaitHandle.Reset();
        }

        public void Resume()
        {
            if (!_started)
                throw new InvalidOperationException("Loop not started");

            _pauseWaitHandle.Set();
        }

        private void Work()
        {
            /* Interval between each sleep call. */
            var interval = _interval;
            /* Lag caused by sleep calls. */
            var lag = 0d;

            _time = DateTime.UtcNow;
            _deltaTime = new TimeSpan();

            try
            {
                while (_started)
                {
                    /* If we're paused. */
                    if (_pauseWaitHandle.WaitOne())
                    {
                        _time = DateTime.UtcNow;
                        _deltaTime = new TimeSpan();

                        lag = 0;
                    }

                    var now = DateTime.UtcNow;

                    /* Calculate the delta time & the lag. */
                    _deltaTime = now - _time;
                    _time = now;

                    lag += _deltaTime.TotalMilliseconds;

                    /* Do an update calling the user code. */
                    DoUpdate();

                    /* Catch up if we've lagged more than the a tick interval. */
                    while (lag >= interval)
                    {
                        DoUpdate();
                        lag -= interval;
                    }

                    Thread.Sleep(interval);
                }
            }
            catch (ThreadAbortException)
            {
                // Space
            }
        }

        private void DoUpdate()
        {
            /*
                Pass control to the user code and if it throws an exception, 
                pass it back to user exception handler.
             */
            try { _handler(); }
            catch (Exception e) { _exceptionHandler(e); }
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
                /* Stop the thread. */
                Stop();

                _pauseWaitHandle.Dispose();
            }

            _disposed = true;
        }
    }
}
