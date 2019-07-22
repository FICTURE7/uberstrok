using System;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public class Sampler
    {
        protected int SampleSize { get; }

        protected float Sum { get; set; }
        protected float Count { get; set; }
        protected float SampleSum { get; set; }
        protected float SampleCount { get; set; }

        public float Avg => Sum / Count;
        public float AvgSample => SampleSum / SampleCount;


        public Sampler(int sampleSize)
        {
            if (sampleSize < 0)
                throw new ArgumentOutOfRangeException(nameof(sampleSize));

            SampleSize = sampleSize;
        }

        public virtual bool Sample(float value)
        {
            SampleSum += value;
            SampleCount++;

            if (SampleCount >= SampleSize)
            {
                Sum += AvgSample;
                Count++;

                SampleSum = 0;
                SampleCount = 0;
                return true;
            }

            return false;
        }
    }
}
