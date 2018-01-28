using System;
using System.Threading;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public static class LoopTestsIThink
    {
        public static void NotMain()
        {
            var loop = new Loop(128);
            Console.WriteLine(loop.Interval);

            loop.Start(HandleLoop, HandleLoopException);
            Console.ReadLine();
        }

        private static int totalTick = 0;
        private static double totalTime = 0;
        private static DateTime lastTick = DateTime.Now;

        private static double tps = 0f;
        public static void HandleLoop()
        {
            var now = DateTime.Now;

            totalTick++;
            totalTime += (now - lastTick).TotalSeconds;

            tps = totalTick / totalTime;

            Console.WriteLine(tps);
            lastTick = now;
        }

        public static void HandleLoopException(Exception e)
        {

        }
    }
}
