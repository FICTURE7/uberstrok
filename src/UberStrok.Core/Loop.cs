using System;
using System.Collections.Concurrent;
using System.Threading;

namespace UberStrok.Core
{
    public delegate void LoopHandler();

    public delegate void LoopExceptionHandler(Exception e);

    public class Loop : IDisposable
    {
        /* Figure out if we've been disposed. */
        private bool _disposed;

        /* Figure out if the loop has started. */
        private bool _started;

        /* Amount of milliseconds we've lagged in between ticks. */
        private double _lag;

        /* Loop handler which is called every tick. */
        private LoopHandler _handler;
        /* Loop exception handler which is called when _handler throws an exception. */
        private LoopExceptionHandler _exceptionHandler;

        private readonly EventWaitHandle _pauseWaitHandle;

        /* Queue of Action to execute before calling the loop handler. */
        private readonly ConcurrentQueue<Action> _workQueue;

        /* Thread that is going to do the work. */
        private readonly Thread _thread;

        public double Interval { get; }
        public bool CatchUp { get; }
        public DateTime Time { get; private set; }
        public TimeSpan DeltaTime { get; private set; }

        public Loop(int tps)
            : this(tps, false)
        {
            /* Space */
        }

        public Loop(int tps, bool catchUp)
        {
            if (tps < 0)
                throw new ArgumentOutOfRangeException(nameof(tps), "Tick rate cannot be less than 0.");

            CatchUp = catchUp;
            Interval = 1000d / tps;

            _workQueue = new ConcurrentQueue<Action>();
            _pauseWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            _thread = new Thread(Work) { Name = "Loop-thread" };
        }

        public void Start(LoopHandler handler, LoopExceptionHandler exceptionHandler)
        {
            if (_started)
                throw new InvalidOperationException("Loop already started.");
            if (_handler != null || _exceptionHandler != null)
                throw new InvalidOperationException("Loop can be started only once.");

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));

            _started = true;
            _thread.Start();
        }

        public void Stop()
        {
            if (!_started)
                throw new InvalidOperationException("Loop not started.");

            _started = false;

            /* 
             * Give the thread a chance to spin a couple of times to shut
             * down gracefully then kill if it does not.
             */
            if (!_thread.Join((int)Math.Ceiling(Interval * 3)))
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

            _lag = 0;
            Time = DateTime.UtcNow;
            DeltaTime = new TimeSpan();

            _pauseWaitHandle.Set();
        }

        public void Enqueue(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _workQueue.Enqueue(action);
        }

        public int ToTicks(TimeSpan span)
        {
            return (int)(span.TotalMilliseconds / Interval);
        }

        /* Loop with catch up logic. */
        private void Work()
        {
            /* Interval between each sleep call. */
            var interval = Interval;
            /* Interval between each sleep call but ceiled. */
            var ceiledInterval = (int)Math.Ceiling(interval);

            _lag = 0;
            Time = DateTime.UtcNow;
            DeltaTime = default(TimeSpan);

            try
            {
                while (_started)
                {
                    /* Wait to get the signal first. */
                    _pauseWaitHandle.WaitOne();

                    /* Execute all the actions enqueued before updating. */
                    DoActions();

                    /* Do time calculatations such as delta time. */
                    DoTime();
                    /* Do an update calling the user code. */
                    DoUpdate();

                    /* Catch up if we've lagged more than the a tick interval. */
                    while (CatchUp && _lag >= interval)
                    {
                        DoTime();
                        DoUpdate();

                        _lag -= interval;
                    }

                    Thread.Sleep(ceiledInterval);
                }
            }
            catch (ThreadAbortException)
            {
                /* Space */
            }
        }

        private void DoActions()
        {
            while (!_workQueue.IsEmpty)
            {
                if (!_workQueue.TryDequeue(out Action action))
                    continue;

                try { action(); }
                catch (Exception e) { _exceptionHandler(e); }
            }
        }

        private void DoTime()
        {
            var now = DateTime.UtcNow;

            /* Calculate the delta time & the lag. */
            DeltaTime = now - Time;
            Time = now;

            if (CatchUp)
                _lag += DeltaTime.TotalMilliseconds;
        }

        private void DoUpdate()
        {
            /*
             * Pass control to the user code and if it throws an exception,
             * pass it back to user exception handler.
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
