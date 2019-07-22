using System;

namespace UberStrok.Core
{
    public interface ITimer
    {
        bool IsEnabled { get; set; }
        float Interval { get; set; }

        event Action Elapsed;

        void Start();
        void Stop();
        void Reset();
        void Restart();
        bool Tick();
    }
}
