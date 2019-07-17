using System;
using System.Threading;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public class LoopSchedulerTests
    {
        private static float dtSum;
        private static float dtCount;

        private static float dtSampleSum;
        private static float dtSampleCount;
        
        private static Loop loop;
        private static LoopScheduler loopScheduler;

        public static void Test()
        {
            loop = new Loop(OnTick, OnTickException);

            loopScheduler = new LoopScheduler(64);
            loopScheduler.Start();

            Thread.Sleep(1000);

            loopScheduler.Schedule(loop);

            Console.ReadLine();
        }

        private static void OnTick()
        {
            dtSampleSum += loop.DeltaTime;
            dtSampleCount++;

            //Console.WriteLine($"{dtSampleSum / dtSampleCount} avg samp dt");

            if (dtSampleCount == loopScheduler.TickRate)
            {
                dtSum += dtSampleSum / dtSampleCount;
                dtCount++;

                dtSampleCount = 0;
                dtSampleSum = 0;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"> Reset {dtSum / dtCount} avg dt");
                Console.ResetColor();
            }
        }

        private static void OnTickException(Exception ex)
        {

        }
    }
}
