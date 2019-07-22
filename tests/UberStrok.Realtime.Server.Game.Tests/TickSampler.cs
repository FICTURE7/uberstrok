using System.Diagnostics;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public class TickSampler : Sampler
    {
        private readonly Stopwatch _stopwatch;

        public TickSampler(int sampleSize)
            : base(sampleSize)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public override bool Sample(float value = 0)
        {
            _stopwatch.Stop();
            var result = base.Sample((float)_stopwatch.Elapsed.TotalMilliseconds);
            _stopwatch.Restart();
            return result;
        }
    }
}
