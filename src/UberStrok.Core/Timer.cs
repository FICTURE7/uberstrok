using System;

namespace UberStrok.Core
{
    public class Timer
    {
        private double _accumulator;
        private readonly ILoop _loop;

        public bool IsEnabled { get; set; }
        public float Interval { get; private set; }

        public event Action Tick;

        public Timer(ILoop loop, TimeSpan interval)
        {
            _loop = loop ?? throw new ArgumentNullException(nameof(loop));
            Interval = interval >= default(TimeSpan) ? 
                        (float)interval.TotalMilliseconds : 
                        throw new ArgumentOutOfRangeException(nameof(interval));

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

        public bool Update()
        {
            if (!IsEnabled)
                return false;

            _accumulator += _loop.DeltaTime;
            if (_accumulator >= Interval)
            {
                OnTick();
                return true;
            }

            return false;
        }

        protected virtual void OnTick()
        {
            _accumulator -= Interval;
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
