using System;

namespace UberStrok.Core
{
    public class Timer
    {
        private double _accumulator;
        private readonly Loop _loop;

        public bool IsEnabled { get; private set; }
        public TimeSpan Interval { get; set; }

        public event Action Tick;

        public Timer(Loop loop, TimeSpan interval)
        {
            _loop = loop ?? throw new ArgumentNullException(nameof(loop));
            Interval = interval > default(TimeSpan) ? interval : throw new ArgumentOutOfRangeException(nameof(interval));

            Reset();
        }

        public void Start()
        {
            if (IsEnabled)
                throw new InvalidOperationException("Timer already started");

            IsEnabled = true;
        }

        public void Stop()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("Timer not started");

            IsEnabled = false;
        }

        public void Reset()
        {
            _accumulator = 0;
            IsEnabled = false;
        }

        public void Update()
        {
            if (!IsEnabled)
                return;

            if (Interval == default)
            {
                OnTick();
            }
            else
            {
                _accumulator += _loop.DeltaTime.Milliseconds;
                if (_accumulator >= Interval.TotalMilliseconds)
                    OnTick();
            }
        }

        protected virtual void OnTick()
        {
            _accumulator -= Interval.TotalMilliseconds;
            Tick?.Invoke();
        }
    }
}
