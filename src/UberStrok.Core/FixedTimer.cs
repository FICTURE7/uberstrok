namespace UberStrok.Core
{
    public sealed class FixedTimer : BaseTimer
    {
        private float _tickTime;
        private float _accTime;

        public FixedTimer(ILoop loop, float interval)
            : base(loop, interval)
        {
            /* Space. */
        }

        public override void Reset()
        {
            _tickTime = 0;
            _accTime = 0;

            IsEnabled = false;
        }

        public override bool Tick()
        {
            if (!IsEnabled)
                return false;

            /* Check if Tick() was called in the same ILoop.Tick() call. */
            if (_tickTime != Loop.Time)
            {
                _tickTime = Loop.Time;
                _accTime += Loop.DeltaTime;
            }

            if (_accTime >= Interval)
            {
                OnElapsed();
                _accTime -= Interval;
                return true;
            }

            return false;
        }
    }
}
