using System;
using System.Collections.Generic;
using System.Text;

namespace pets4home.core
{
    public interface ISchedulerTask
    {
        string Name { get; set; }
        TimeSpan Interval { get; set; }
        bool Enabled { get; set; }
        void Execute();
    }
}
