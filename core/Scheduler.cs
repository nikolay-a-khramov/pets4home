using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
                log.Warn("Skipped attemt to add task that is already scheduled: " + task.Name);
                return;
            }

            var timer = new System.Threading.Timer((e) => task.Execute(), null, TimeSpan.Zero, task.Interval);
            tasks.Add(task, timer);
            log.Info(String.Format("Scheduled task {0} with interval {1}", task.Name, task.Interval));
        }

        public void RemoveTask(ISchedulerTask task)
        {
            if (!tasks.ContainsKey(task))
            {
                log.Error("Unable to find taks to remove: " + task.Name);
                return;
            }

            WaitUntilCompleted(new List<Timer> { tasks[task] });
            tasks.Remove(task);
            log.Info("Task removed " + task.Name);
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
                    string error = "Timer already disposed: " + timer;
                    log.Error(error);
                    throw new Exception(error);
                }
                waitHandles.Add(h);
            }
            WaitHandle.WaitAll(waitHandles.ToArray());
        }
    }
}
