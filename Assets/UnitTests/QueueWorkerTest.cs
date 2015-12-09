using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniRx.InternalUtil;

namespace UniRx.Tests
{
    [TestClass]
    public class QueueWorkerTest
    {
        [TestMethod]
        public void Enq()
        {
            var q = new ThreadSafeQueueWorker();

            var l = new List<int>();
            q.Enqueue(() => l.Add(1));
            q.Enqueue(() => q.Enqueue(() => l.Add(-1)));
            q.Enqueue(() => l.Add(2));
            q.Enqueue(() => q.Enqueue(() => l.Add(-2)));
            q.Enqueue(() => l.Add(3));
            q.Enqueue(() => q.Enqueue(() => l.Add(-3)));
            q.Enqueue(() => l.Add(4));
            q.Enqueue(() => q.Enqueue(() => l.Add(-4)));
            q.Enqueue(() => l.Add(5));
            q.Enqueue(() => q.Enqueue(() => l.Add(-5)));
            q.Enqueue(() => l.Add(6));
            q.Enqueue(() => q.Enqueue(() => l.Add(-6)));
            q.Enqueue(() => l.Add(7));
            q.Enqueue(() => q.Enqueue(() => l.Add(-7)));
            q.Enqueue(() => l.Add(8));
            q.Enqueue(() => q.Enqueue(() => l.Add(-8)));
            q.Enqueue(() => l.Add(9));
            q.Enqueue(() => q.Enqueue(() => l.Add(-9)));
            q.Enqueue(() => l.Add(10));
            q.Enqueue(() => q.Enqueue(() => l.Add(-10)));
            q.Enqueue(() => l.Add(11));
            q.Enqueue(() => q.Enqueue(() => l.Add(-11)));
            q.Enqueue(() => l.Add(12));

            q.ExecuteAll(ex => { });

            l.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.IsCollection(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Count.Is(0);

            q.Enqueue(() => l.Add(1));
            q.Enqueue(() => q.Enqueue(() => l.Add(-1)));
            q.Enqueue(() => l.Add(2));
            q.Enqueue(() => q.Enqueue(() => l.Add(-2)));
            q.Enqueue(() => l.Add(3));
            q.Enqueue(() => q.Enqueue(() => l.Add(-3)));
            q.Enqueue(() => l.Add(4));
            q.Enqueue(() => q.Enqueue(() => l.Add(-4)));
            q.Enqueue(() => l.Add(5));
            q.Enqueue(() => q.Enqueue(() => l.Add(-5)));
            q.Enqueue(() => l.Add(6));
            q.Enqueue(() => q.Enqueue(() => l.Add(-6)));
            q.Enqueue(() => l.Add(7));
            q.Enqueue(() => q.Enqueue(() => l.Add(-7)));
            q.Enqueue(() => l.Add(8));
            q.Enqueue(() => q.Enqueue(() => l.Add(-8)));
            q.Enqueue(() => l.Add(9));
            q.Enqueue(() => q.Enqueue(() => l.Add(-9)));
            q.Enqueue(() => l.Add(10));
            q.Enqueue(() => q.Enqueue(() => l.Add(-10)));
            q.Enqueue(() => l.Add(11));
            q.Enqueue(() => q.Enqueue(() => l.Add(-11)));
            q.Enqueue(() => l.Add(12));

            q.ExecuteAll(ex => { });
            l.IsCollection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.IsCollection(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Count.Is(0);
        }
    }
}
