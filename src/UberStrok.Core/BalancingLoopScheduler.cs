using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UberStrok.Core
{
    public class BalancingLoopScheduler : ILoopScheduler, IDisposable
    {
        private bool _disposed;

        private readonly LoopScheduler[] _schedulers;
        private readonly Dictionary<ILoop, LoopScheduler> _loops;

        public float TickInterval { get; }
        public float TickRate { get; }
        public ICollection<LoopScheduler> Schedulers => _schedulers;

        public BalancingLoopScheduler(float tickRate)
        {
            if (tickRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(tickRate), "Tick rate cannot be less or equal to 0.");

            TickRate = tickRate;
            TickInterval = 1000 / tickRate;

            _loops = new Dictionary<ILoop, LoopScheduler>();
            _schedulers = new LoopScheduler[Environment.ProcessorCount * 2];

            for (int i = 0; i < _schedulers.Length; i++)
            {
                _schedulers[i] = new LoopScheduler(tickRate);
                _schedulers[i].Start();

                /* 
                 * We don't have any Loop to schedule yet; so we cant pause it.
                 */
                _schedulers[i].Pause();
            }
        }

        public void Schedule(ILoop loop)
        {
            var scheduler = GetLeastLoadScheduler();

            Debug.Assert(scheduler != null);

            scheduler.Schedule(loop);
            _loops.Add(loop, scheduler);

            /* If scheduler is paused; resume it. */
            if (scheduler.IsPaused)
                scheduler.Resume();
        }

        public bool Unschedule(ILoop loop)
        {
            if (!_loops.TryGetValue(loop, out LoopScheduler scheduler))
                return false;

            Debug.Assert(!scheduler.IsPaused);

            var result = scheduler.Unschedule(loop) & _loops.Remove(loop);

            /* If no Loop on the LoopScheduler; we can pause it. */
            if (scheduler.Loops.Count == 0)
                scheduler.Pause();
            return result;
        }

        public float GetLoad()
        {
            var sum = 0f;
            for (int i = 0; i < _schedulers.Length; i++)
                sum += _schedulers[i].GetLoad();
            return sum / _schedulers.Length;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            foreach (var scheduler in _schedulers)
                scheduler.Dispose();

            _loops.Clear();
            _disposed = true;
        }

        /* Get the LoopScheduler with the least load. */
        public LoopScheduler GetLeastLoadScheduler()
        {
            var minScheduler = default(LoopScheduler);
            var minLoad = float.PositiveInfinity;
            for (int i = 0; i < _schedulers.Length; i++)
            {
                var scheduler = _schedulers[i];
                var load = scheduler.GetLoad();
                if (load < minLoad)
                {
                    minLoad = load;
                    minScheduler = scheduler;
                }
            }

            /* wat ? Should never happen, but in case. */
            if (minScheduler == null)
                minScheduler = _schedulers[0];
            return minScheduler;
        }
    }
}
