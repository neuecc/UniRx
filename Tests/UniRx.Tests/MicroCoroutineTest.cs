using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace UniRx.Tests
{
    class DecrementEnumerator : IEnumerator
    {
        readonly int original;
        int count;

        public DecrementEnumerator(int count)
        {
            this.original = count;
            this.count = count;
        }

        public object Current
        {
            get
            {
                return null;
            }
        }
        public int OriginalCount => original;

        public int Count => count;

        public bool MoveNext()
        {
            return count-- > 0;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return $"{count}/{original}";
        }
    }

    [TestClass]
    public class MicroCoroutineTest
    {
        static UniRx.InternalUtil.MicroCoroutine Create()
        {
            return new InternalUtil.MicroCoroutine(ex => Console.WriteLine(ex));
        }

        static int FindLast(UniRx.InternalUtil.MicroCoroutine mc)
        {
            var coroutines = mc.GetType().GetField("coroutines", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var enumerators = (IEnumerator[])coroutines.GetValue(mc);

            int tail = -1;
            for (int i = 0; i < enumerators.Length; i++)
            {
                if (enumerators[i] == null)
                {
                    if (tail == -1)
                    {
                        tail = i;
                    }
                }
                else
                {
                    if (tail != -1)
                    {
                        throw new Exception("what's happen?");
                    }
                }
            }

            if (tail == -1) tail = enumerators.Length;
            return tail;
        }

        static int GetTailDynamic(UniRx.InternalUtil.MicroCoroutine mc)
        {
            var tail = mc.GetType().GetField("tail", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            return (int)tail.GetValue(mc);
        }

        [TestMethod]
        public void EnumerationCycle()
        {
            var coroutines = new[]
            {
                new DecrementEnumerator(1),
                new DecrementEnumerator(2),
                new DecrementEnumerator(3),
                new DecrementEnumerator(4),
                new DecrementEnumerator(5),
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }

            mc.Run();
            GetTailDynamic(mc).Is(5);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(0, 1, 2, 3, 4);

            mc.Run();
            GetTailDynamic(mc).Is(4);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, 0, 1, 2, 3);

            mc.Run();
            GetTailDynamic(mc).Is(3);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, 0, 1, 2);

            mc.Run();
            GetTailDynamic(mc).Is(2);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, 0, 1);

            mc.Run();
            GetTailDynamic(mc).Is(1);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, 0);

            mc.Run();
            GetTailDynamic(mc).Is(0);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, -1);
        }

        [TestMethod]
        public void EnumerationCycleBlank()
        {
            var coroutines = new[]
            {
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
                new DecrementEnumerator(0),
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }

            GetTailDynamic(mc).Is(5);

            mc.Run();
            GetTailDynamic(mc).Is(0);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, -1);
        }

        [TestMethod]
        public void EnumerationCycleFull()
        {
            var coroutines = new[]
            {
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
                new DecrementEnumerator(1),
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }

            GetTailDynamic(mc).Is(16);

            mc.Run();
            GetTailDynamic(mc).Is(16);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            mc.Run();
            GetTailDynamic(mc).Is(0);
            coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
        }

        [TestMethod]
        public void EnumerationCycleRandom()
        {
            // pattern for shuffle
            var rand = new Random();
            // large number
            {
                for (int i = 0; i < 1000; i++)
                {
                    var expectedTail = rand.Next(1, 100);

                    var coroutines = Enumerable.Range(1, expectedTail)
                        .Select(x => new DecrementEnumerator(rand.Next(0, 100)))
                        .ToArray();

                    var mc = Create();
                    foreach (var item in coroutines)
                    {
                        mc.AddCoroutine(item);
                    }

                    var maxRunCount = coroutines.Max(x => x.OriginalCount);
                    var expected = coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).ToArray();



                    for (int j = 0; j < maxRunCount + 1; j++)
                    {
                        mc.Run();

                        // validate
                        expected = expected.Select(x => (x == -1) ? -1 : (x - 1)).ToArray();
                        coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(expected);

                        var tail = FindLast(mc);
                        GetTailDynamic(mc).Is(tail);
                    }
                    GetTailDynamic(mc).Is(0);
                }
            }
            {
                // small case
                for (int i = 0; i < 1000; i++)
                {
                    var expectedTail = rand.Next(1, 100);

                    var coroutines = Enumerable.Range(1, expectedTail)
                        .Select(x => new DecrementEnumerator(rand.Next(0, 5)))
                        .ToArray();

                    var mc = Create();
                    foreach (var item in coroutines)
                    {
                        mc.AddCoroutine(item);
                    }

                    var maxRunCount = coroutines.Max(x => x.OriginalCount);
                    var expected = coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).ToArray();

                    for (int j = 0; j < maxRunCount + 1; j++)
                    {
                        mc.Run();

                        // validate
                        expected = expected.Select(x => (x == -1) ? -1 : (x - 1)).ToArray();
                        coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(expected);

                        var tail = FindLast(mc);
                        GetTailDynamic(mc).Is(tail);
                    }
                    GetTailDynamic(mc).Is(0);
                }
            }
            {
                // small case2
                for (int i = 0; i < 1000; i++)
                {
                    var expectedTail = rand.Next(1, 10);

                    var coroutines = Enumerable.Range(1, expectedTail)
                        .Select(x => new DecrementEnumerator(rand.Next(0, 5)))
                        .ToArray();

                    var mc = Create();
                    foreach (var item in coroutines)
                    {
                        mc.AddCoroutine(item);
                    }

                    var maxRunCount = coroutines.Max(x => x.OriginalCount);
                    var expected = coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).ToArray();

                    for (int j = 0; j < maxRunCount + 1; j++)
                    {
                        mc.Run();

                        // validate
                        expected = expected.Select(x => (x == -1) ? -1 : (x - 1)).ToArray();
                        coroutines.OrderBy(x => x.OriginalCount).Select(x => x.Count).IsCollection(expected);

                        var tail = FindLast(mc);
                        GetTailDynamic(mc).Is(tail);
                    }
                    GetTailDynamic(mc).Is(0);
                }
            }
        }
    }
}
