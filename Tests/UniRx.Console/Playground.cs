using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UniRx
{
    public class Playground
    {
        public void Run()
        {


            var now = Scheduler.ThreadPool.Now;
            var xs = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1))
                .Take(3)
                .Timestamp()
                .Select(x => Math.Round((x.Timestamp - now).TotalSeconds, 0))
                .ToArray()
                .Wait();

            /*
            xs[0].Value.Is(0L);
            (now.AddMilliseconds(800) <= xs[0].Timestamp && xs[0].Timestamp <= now.AddMilliseconds(1200)).IsTrue();

            xs[1].Value.Is(1L);
            (now.AddMilliseconds(1800) <= xs[1].Timestamp && xs[1].Timestamp <= now.AddMilliseconds(2200)).IsTrue();

            xs[2].Value.Is(2L);
            (now.AddMilliseconds(2800) <= xs[2].Timestamp && xs[2].Timestamp <= now.AddMilliseconds(3200)).IsTrue();
            */

        }
    }
}
