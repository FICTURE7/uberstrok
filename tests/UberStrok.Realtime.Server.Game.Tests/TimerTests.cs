using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public static class TimerTests
    {
        private static Random random;

        private static Timer timer;
        private static FixedTimer fixedTimer;

        private static Loop loop;
        private static LoopScheduler loopScheduler;

        private static TickSampler timerSampler;
        private static TickSampler fixedTimerSampler;

        public static void Test()
        {
            random = new Random();

            loop = new Loop(OnTick, OnTickError);
            loopScheduler = new LoopScheduler(64);

            timer = new Timer(loop, 100);
            fixedTimer = new FixedTimer(loop, 100);

            int sampleSize = (int)timer.Interval;

            timerSampler = new TickSampler(sampleSize);
            fixedTimerSampler = new TickSampler(sampleSize);
            
            timer.Start();
            fixedTimer.Start();

            loopScheduler.Schedule(loop);
            loopScheduler.Start();
        }

        private static void OnTick()
        {
            if (random.Next() >= 0.95f)
                System.Threading.Thread.Sleep(200);

            while (timer.Tick())
            {
                if (timerSampler.Sample(1))
                    Console.WriteLine($"Timer => {timerSampler.Avg}");
            }

            while (fixedTimer.Tick())
            {
                if (fixedTimerSampler.Sample(1))
                    Console.WriteLine($"FixedTimer => {fixedTimerSampler.Avg}");
            }
        }

        private static void OnTickError(Exception ex)
        {

        }
    }
}
