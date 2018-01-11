using log4net;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace UberStrok.Realtime.Server
{
    public class JobManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(nameof(JobManager));

        public JobManager()
        {
            _schedulerThread = new Thread(Work);
            _jobs = new ConcurrentQueue<Job>();

            _hasJob = false;

            _schedulerThread.Start();
        }

        private bool _hasJob;
        private Job _currentJob;
        private ConcurrentQueue<Job> _jobs;
        private Thread _schedulerThread;

        public void Add(Action action, DateTime when)
        {
            var job = new Job(action, when);
            if (_jobs.IsEmpty)
                _currentJob = job;

            _jobs.Enqueue(job);
            _hasJob = true;
        }

        private void Work()
        {
            try
            {
                while (true)
                {
                    if (_hasJob)
                    {
                        var currentJob = _currentJob;
                        if (DateTime.UtcNow >= currentJob.When)
                        {
                            currentJob.Action();
                            _hasJob = _jobs.TryDequeue(out _currentJob);

                            s_log.Info($"Running Job! {_jobs.Count}:{_hasJob}");
                        }
                    }

                    Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException)
            {
                // Not much.
            }
            catch (Exception e)
            {
                s_log.Error("Out-Loop scheduler failed.", e);
            }
        }

        private struct Job
        {
            public Job(Action action, DateTime when)
            {
                Action = action;
                When = when;
            }

            public Action Action;
            public DateTime When;
        }
    }
}
