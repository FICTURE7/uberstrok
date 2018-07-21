using System;
using System.Diagnostics;
using System.Threading;
using UberStrok;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public static class LoopTestsIThink
    {
        public static void Main()
        {
            loop = new Loop(10);
            Console.WriteLine(loop.Interval);

            loop.Start(HandleLoop, HandleLoopException);
            sw.Start();
            Console.ReadLine();
        }

        private static Loop loop;
        private static int totalTick = 0;
        private static Stopwatch sw = new Stopwatch();

        private static double tps = 0f;
        public static void HandleLoop()
        {
            totalTick++;
            tps = totalTick / sw.Elapsed.TotalSeconds;

            Console.WriteLine($"tps: {tps}, DeltaTime: {loop.DeltaTime.TotalMilliseconds}");
        }

        public static void HandleLoopException(Exception e)
        {

        }
    }
}
