using System;

namespace UberStrok.Core
{
    public class Countdown
    {
        private readonly Timer _timer;

        public bool IsEnabled => _timer.IsEnabled;

        public int CurrentCount { get; private set; }
        public int StartCount { get; }
        public int EndCount { get; }

        public event Action Completed;
        public event Action<int> Counted;

        public Countdown(Loop loop, int startCount, int endCount)
        {
            if (startCount <= endCount)
                throw new ArgumentOutOfRangeException();

            StartCount = startCount;
            EndCount = endCount;

            _timer = new Timer(loop, TimeSpan.FromSeconds(1));
            _timer.Tick += OnTick;

            Reset();
        }

        public void Start()
        {
            _timer.Start();

            /* Tick event on the first count. */
            OnTick();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Reset()
        {
            CurrentCount = StartCount;
            _timer.Reset();
        }

        public void Restart()
        {
            Reset();
            Start();
        }

        public void Tick()
        {
            _timer.Update();
        }

        private void OnTick()
        {
            Counted?.Invoke(CurrentCount);

            int newCount = CurrentCount - 1;
            if (newCount < EndCount)
            {
                Stop();
                Completed?.Invoke();
            }
            else
            {
                CurrentCount = newCount;
            }
        }
    }
}
