using System;

namespace UberStrok.Core
{
    public class PingManager
    {
        public int Value { get; private set; }
        public int FalsePositive { get; private set; }

        public bool CanUpdate => (DateTime.UtcNow - _lastUpdate).TotalSeconds >= 5;

        private DateTime _lastUpdate;

        public PingManager()
        {
            _lastUpdate = DateTime.UtcNow;
        }

        public void Update(int value)
        {
            if (!CanUpdate)
                FalsePositive++;

            Value = value;
            _lastUpdate = DateTime.UtcNow;
        }
    }
}
