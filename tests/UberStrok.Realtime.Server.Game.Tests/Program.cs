using System;
using System.Threading;

namespace UberStrok.Realtime.Server.Game.Tests
{
    public static class Program
    {
        public static void Main()
        {
            var loop = new Loop(64);
            Console.WriteLine(loop.Interval);

            loop.Start(HandleLoop, HandleLoopException);
            loop.Pause();

            Thread.Sleep(1000);

            loop.Resume();

            Thread.Sleep(1000);

            loop.Pause();

            Thread.Sleep(1000);

            loop.Resume();

            loop.Stop();

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

            tps = totalTime / totalTick;

            Console.WriteLine(1 / (now - lastTick).TotalSeconds);
            lastTick = now;

        }

        public static void HandleLoopException(Exception e)
        {

        }
    }
}
