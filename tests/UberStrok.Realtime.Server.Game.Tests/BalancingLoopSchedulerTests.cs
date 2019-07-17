using System;
using System.Threading;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public class BalancingLoopSchedulerTests
    {
        private static Random random;
        private static BalancingLoopScheduler loopScheduler;

        public static void Test()
        {
            random = new Random();
            loopScheduler = new BalancingLoopScheduler(64);

            for (int i = 0; i < 8; i++)
            {
                var loop = new Loop(OnTick, OnTickError);
                loopScheduler.Schedule(loop);
                Thread.Sleep(100);
            }

            Console.ReadLine();
        }

        public static void OnTick()
        {
            /*
            if (random.Next() < 0.01f)
                Thread.Sleep(8);
            */
        }

        public static void OnTickError(Exception ex)
        {

        }
    }
}
