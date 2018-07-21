using System;
using System.Diagnostics;
using System.Threading;

namespace UberStrok
{
    public delegate void LoopHandler();
    public delegate void LoopExceptionHandler(Exception ex);

    public class Loop : IDisposable
    {
        /* Counter for loop threads, so they have unique names. */
        private static int s_threadId = 0;

        /* Figure out if we've been disposed. */
        private bool _disposed;

        /* Figure out if the loop has started. */
        private bool _started;

        /* Amount of milliseconds we've lagged in between ticks. */
        private double _lag;
        /* Time of the current tick/time of loop. */
        private DateTime _time;
        /* Time difference between the current tick and the last tick. */
        private TimeSpan _deltaTime;

        /* Loop handler which is called every tick. */
        private LoopHandler _handler;
        /* Loop exception handler which is called when _handler throws an exception. */
        private LoopExceptionHandler _exceptionHandler;

        /* Stopwatch to measure lag. */
        private readonly Stopwatch _stopwatch;
        /* EventWaitHandle to pause/resume the loop thread. */
        private readonly EventWaitHandle _pauseWaitHandle;
        /* Amount of time the thread needs to sleep in ms. */
        private readonly double _interval;
        /* Thread that is going to do the work. */
        private readonly Thread _thread;

        public Loop(int tps)
        {
            if (tps < 0)
                throw new ArgumentOutOfRangeException(nameof(tps), "Tick rate cannot be less than 0.");

            _stopwatch = new Stopwatch();
            _pauseWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            _thread = new Thread(Work) { Name = "GameLoop-thread" + Interlocked.Increment(ref s_threadId) };
            _interval = tps > 0 ? 1000d / tps : 0;
        }

        public double Interval => _interval;
        public DateTime Time => _time;
        public TimeSpan DeltaTime => _deltaTime;

        public void Start(LoopHandler handler, LoopExceptionHandler exceptionHandler)
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "Cannot access diposed Loop instance.");
            if (_started)
                throw new InvalidOperationException("Loop already started.");

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
            if (_disposed)
                throw new ObjectDisposedException(null, "Cannot access diposed Loop instance.");
            if (!_started)
                throw new InvalidOperationException("Loop not started.");

            _started = false;

            /* 
                Give the thread a chance to spin a couple of times to shut down
                gracefully then abort if it does not.
             */
            if (!_thread.Join((int)Math.Ceiling(Interval * 3)))
                _thread.Abort();
        }

        public void Pause()
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "Cannot access diposed Loop instance.");
            if (!_started)
                throw new InvalidOperationException("Loop not started");

            /* Set signal to wait */
            _pauseWaitHandle.Reset();
        }

        public void Resume()
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "Cannot access diposed Loop instance.");
            if (!_started)
                throw new InvalidOperationException("Loop not started");

            _lag = 0;
            _time = DateTime.Now;
            _deltaTime = new TimeSpan();

            /* Set signal */
            _pauseWaitHandle.Set();
        }

        public int ToTicks(TimeSpan span)
        {
            if (_interval == 0)
                return 0;

            return (int)(span.TotalMilliseconds / _interval);
        }

        private void Work()
        {
            /* Interval between each sleep call but ceiled. */
            var ceiledInterval = (int)Math.Ceiling(_interval);

            _lag = 0;
            _time = DateTime.Now;
            _deltaTime = new TimeSpan();

            try
            {
                while (_started)
                {
                    _lag += _stopwatch.Elapsed.TotalMilliseconds;
                    _stopwatch.Restart();

                    /* Wait to get the signal first. */
                    _pauseWaitHandle.WaitOne();

                    /* Catch up if we've lagged more than the a tick interval. */
                    while (_lag >= _interval)
                    {
                        DoUpdate();
                        _lag -= _interval;
                    }

                    Thread.Sleep(ceiledInterval);
                    _stopwatch.Stop();
                }
            }
            catch (ThreadAbortException)
            {
                Debug.WriteLine("[Loop] THreadAbortException reached.");
            }
        }

        private void DoTime()
        {
            /* Calculate the delta time & the lag. */
            var now = DateTime.Now;
            _deltaTime = now - _time;
            _time = now;
        }

        private void DoUpdate()
        {
            /*
                Pass control to the user code and if it throws an exception, 
                pass it back to user exception handler.
             */
            try { _handler(); }
            catch (Exception ex) { _exceptionHandler(ex); }

            DoTime();
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
