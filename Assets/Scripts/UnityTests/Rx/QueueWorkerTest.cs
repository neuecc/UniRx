using System.Collections.Generic;
using NUnit.Framework;

namespace UniRx.Tests
{
    
    public class QueueWorkerTest
    {
        [Test]
        public void Enq()
        {
            var q = new UniRx.InternalUtil.ThreadSafeQueueWorker();

            var l = new List<int>();
            q.Enqueue(x => l.Add((int)x), 1);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -1);
            q.Enqueue(x => l.Add((int)x), 2);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -2);
            q.Enqueue(x => l.Add((int)x), 3);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -3);
            q.Enqueue(x => l.Add((int)x), 4);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -4);
            q.Enqueue(x => l.Add((int)x), 5);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -5);
            q.Enqueue(x => l.Add((int)x), 6);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -6);
            q.Enqueue(x => l.Add((int)x), 7);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -7);
            q.Enqueue(x => l.Add((int)x), 8);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -8);
            q.Enqueue(x => l.Add((int)x), 9);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -9);
            q.Enqueue(x => l.Add((int)x), 10);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -10);
            q.Enqueue(x => l.Add((int)x), 11);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -11);
            q.Enqueue(x => l.Add((int)x), 12);

            q.ExecuteAll(ex => { });

            l.Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Is(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Count.Is(0);

            q.Enqueue(x => l.Add((int)x), 1);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -1);
            q.Enqueue(x => l.Add((int)x), 2);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -2);
            q.Enqueue(x => l.Add((int)x), 3);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -3);
            q.Enqueue(x => l.Add((int)x), 4);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -4);
            q.Enqueue(x => l.Add((int)x), 5);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -5);
            q.Enqueue(x => l.Add((int)x), 6);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -6);
            q.Enqueue(x => l.Add((int)x), 7);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -7);
            q.Enqueue(x => l.Add((int)x), 8);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -8);
            q.Enqueue(x => l.Add((int)x), 9);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -9);
            q.Enqueue(x => l.Add((int)x), 10);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -10);
            q.Enqueue(x => l.Add((int)x), 11);
            q.Enqueue(x => q.Enqueue(_ => l.Add((int)x), null), -11);
            q.Enqueue(x => l.Add((int)x), 12);

            q.ExecuteAll(ex => { });
            l.Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Is(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11);
            l.Clear();

            q.ExecuteAll(ex => { });
            l.Count.Is(0);
        }
    }
}
