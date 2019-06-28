using System;

namespace UberStrok.Core
{
    public class Timer
    {
        private double _accumulator;
        private readonly Loop _loop;

        public bool IsEnabled { get; set; }
        public TimeSpan Interval { get; private set; }

        public event Action Tick;

        public Timer(Loop loop, TimeSpan interval)
        {
            _loop = loop ?? throw new ArgumentNullException(nameof(loop));
            Interval = interval >= default(TimeSpan) ? interval : throw new ArgumentOutOfRangeException(nameof(interval));

            Reset();
        }

        public bool Start()
        {
            if (IsEnabled)
                return false;

            IsEnabled = true;
            return true;
        }

        public bool Restart()
        {
            Reset();
            return Start();
        }

        public bool Stop()
        {
            if (!IsEnabled)
                return false;

            IsEnabled = false;
            return true;
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

            _accumulator += _loop.DeltaTime.TotalMilliseconds;
            if (_accumulator >= Interval.TotalMilliseconds)
                OnTick();
        }

        protected virtual void OnTick()
        {
            _accumulator -= Interval.TotalMilliseconds;
            Tick?.Invoke();
        }
    }

    public class Timer<T> : Timer
    {
        public new event Action<T> Tick;

        public T Data { get; set; }

        public Timer(Loop loop, TimeSpan interval) 
            : base(loop, interval)
        {
            /* Space */
        }

        protected override void OnTick()
        {
            base.OnTick();

            Tick?.Invoke(Data);
        }
    }
}
