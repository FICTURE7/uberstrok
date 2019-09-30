namespace UberStrok.Core
{
    public sealed class Timer : BaseTimer
    {
        private float _nextElapsed;

        public Timer(ILoop loop, float interval)
            : base(loop, interval)
        {
            /* Space. */
        }

        public override void Reset()
        {
            ResetNextElapsed();
            IsEnabled = false;
        }

        public override bool Tick()
        {
            if (IsEnabled && Loop.Time >= _nextElapsed)
            {
                OnElapsed();
                ResetNextElapsed();
                return true;
            }

            return false;
        }

        /* Calculate the time of when the next Elapsed should happen. */
        private void ResetNextElapsed()
            => _nextElapsed = Loop.Time + Interval;
    }
}
