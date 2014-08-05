using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public static partial class Scheduler
    {
        public static IScheduler MainThread = UniRx.Scheduler.ThreadPool;
    }
}
