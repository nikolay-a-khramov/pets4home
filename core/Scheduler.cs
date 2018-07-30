using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace pets4home.core
{
    class Scheduler
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<ISchedulerTask, Timer> tasks = new Dictionary<ISchedulerTask, Timer>();

        public void AddTask(ISchedulerTask task)
        {
            if (tasks.ContainsKey(task))
            {
                log.Warn($"Skipped attemt to add task that is already scheduled: {task.Name}");
                return;
            }
            var timer = new Timer((e) => ExecuteAndReschedule(task), 
                null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
            tasks.Add(task, timer);
            log.Info($"Scheduled task {task.Name} with interval {task.Interval}");
        }

        private void ExecuteAndReschedule(ISchedulerTask task) 
        {
            task.Execute();
            tasks[task].Dispose();
            int seed = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & (long)int.MaxValue);
            Random rnd = new Random(seed);

            long randomInterval = Convert.ToInt32(task.Interval.TotalMilliseconds)
                + (rnd.Next(
                    Convert.ToInt32(-(task.RangeSize.TotalMilliseconds / 2)),
                    Convert.ToInt32(task.RangeSize.TotalMilliseconds / 2)));
            if (randomInterval < 0)
            {
                randomInterval = 0;
            }
            log.Debug($"[{task}] Scheduling next task in {randomInterval} ms");

            var timer = new Timer((e) => ExecuteAndReschedule(task),
                null, randomInterval, Timeout.Infinite);
            
            tasks[task] = timer;
        }

        public void RemoveTask(ISchedulerTask task)
        {
            if (!tasks.ContainsKey(task))
            {
                log.Error($"Unable to find taks to remove: {task.Name}");
                return;
            }

            WaitUntilCompleted(new List<Timer> { tasks[task] });
            tasks.Remove(task);
            log.Info($"Task removed {task.Name}");
        }

        public void RemoveAllTasks()
        {
            WaitUntilCompleted(new List<Timer>(tasks.Values));
            tasks.Clear();
            log.Info("All tasks removed.");
        }

        private void WaitUntilCompleted(List<Timer> timers)
        {
            List<WaitHandle> waitHandles = new List<WaitHandle>();
            foreach (var timer in timers)
            {
                WaitHandle h = new AutoResetEvent(false);
                if (!timer.Dispose(h))
                {
                    string error = $"Timer already disposed: {timer}";
                    log.Error(error);
                    throw new Exception(error);
                }
                waitHandles.Add(h);
            }
            WaitHandle.WaitAll(waitHandles.ToArray());
        }
    }
}
