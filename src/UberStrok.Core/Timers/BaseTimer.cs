using System;
using System.Runtime.CompilerServices;

namespace UberStrok.Core
{
    public abstract class BaseTimer : ITimer
    {
        private float _interval;

        protected ILoop Loop { get; }

        public bool IsEnabled { get; set; }
        public float Interval
        {
            get => _interval;
            set
            {
                ThrowInterval(value, nameof(value));
                Reset();

                _interval = value;
            }
        }

        public event Action Elapsed;

        public BaseTimer(ILoop loop, TimeSpan interval)
            : this(loop, (float)interval.TotalMilliseconds)
        {
            /* Space. */
        }

        protected BaseTimer(ILoop loop, float interval)
        {
            ThrowInterval(interval, nameof(interval));

            Loop = loop ?? throw new ArgumentNullException(nameof(loop));
            _interval = interval;

            Reset();
        }

        public void Start()
            => IsEnabled = true;

        public void Stop()
            => IsEnabled = false;

        public void Restart()
        {
            Reset();
            Start();
        }

        public abstract void Reset();

        public abstract bool Tick();

        protected void OnElapsed()
        {
            Elapsed?.Invoke();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowInterval(float value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName, "Interval cannot be less or equal to 0.");
        }
    }
}
