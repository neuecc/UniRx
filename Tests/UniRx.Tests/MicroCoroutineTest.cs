using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace UniRx.Tests
{
    class NamedEnumerator : IEnumerator
    {
        string x;

        public NamedEnumerator(string x)
        {
            this.x = x;
        }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return x;
        }
    }

    [TestClass]
    public class MicroCoroutineTest
    {
        static UniRx.InternalUtil.MicroCoroutine Create()
        {
            return new InternalUtil.MicroCoroutine(() => true, ex => Console.WriteLine(ex));
        }

        static void RefreshDynamic(UniRx.InternalUtil.MicroCoroutine mc)
        {
            var mi = mc.GetType().GetMethod("Refresh", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(mc, null);
        }

        static int GetTailDynamic(UniRx.InternalUtil.MicroCoroutine mc)
        {
            var tail = mc.GetType().GetField("tail", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            return (int)tail.GetValue(mc);
        }

        static IEnumerator[] GetInnerCoroutineDynamic(UniRx.InternalUtil.MicroCoroutine mc)
        {
            var coroutines = mc.GetType().GetField("coroutines", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            return (IEnumerator[])coroutines.GetValue(mc);
        }

        [TestMethod]
        public void ForceRefresh1()
        {
            var coroutines = new[]
            {
                new NamedEnumerator("0"), //0 
                null,// 1
                null,// 2
                new NamedEnumerator("3"), // 3
                new NamedEnumerator("4"),//4
                null,// 5
                null,//6
                new NamedEnumerator("7")//7
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(4);
        }

        [TestMethod]
        public void ForceRefresh2()
        {
            var coroutines = new IEnumerator[]
            {
                null, // 0
                null,// 1
                null,// 2
                null, // 3
                null, // 4
                null,// 5
                null,//6
                null // 7
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(0);
        }

        [TestMethod]
        public void ForceRefresh3()
        {
            var coroutines = new IEnumerator[]
            {
                new NamedEnumerator("0"),
                new NamedEnumerator("1"),
                new NamedEnumerator("2"),
                new NamedEnumerator("3"),
                null,
                null,
                null,
                null,
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(4);
        }

        [TestMethod]
        public void ForceRefresh4()
        {
            var coroutines = new IEnumerator[]
            {
                null,
                new NamedEnumerator("0"),
                new NamedEnumerator("1"),
                new NamedEnumerator("2"),
                null,
                null,
                null,
                null,
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(3);
        }

        [TestMethod]
        public void ForceRefresh5()
        {
            var coroutines = new IEnumerator[]
            {
                null,
                null,
                null,
                null,
                null,
                null,
                new NamedEnumerator("0"),
                null,
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(1);
        }

        [TestMethod]
        public void ForceRefresh6()
        {
            var coroutines = new IEnumerator[]
            {
                null,
                new NamedEnumerator("1"),
                null,
                new NamedEnumerator("3"),
                null,
                new NamedEnumerator("5"),
                null,
                null,
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(3);
        }

        [TestMethod]
        public void ForceRefresh7()
        {
            var coroutines = new IEnumerator[]
            {
                null,
                null,
                new NamedEnumerator("2"),
                new NamedEnumerator("3"),
                new NamedEnumerator("4"),
                null,
                new NamedEnumerator("6"),
                new NamedEnumerator("7"),
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(5);
        }

        [TestMethod]
        public void ForceRefresh8()
        {
            var coroutines = new IEnumerator[]
            {
                new NamedEnumerator("0"),
                new NamedEnumerator("1"),
                new NamedEnumerator("2"),
                new NamedEnumerator("3"),
                new NamedEnumerator("4"),
                new NamedEnumerator("5"),
                new NamedEnumerator("6"),
                new NamedEnumerator("7"),
                new NamedEnumerator("8"),
                new NamedEnumerator("9"),
                new NamedEnumerator("10"),
                new NamedEnumerator("11"),
                new NamedEnumerator("12"),
                new NamedEnumerator("13"),
                new NamedEnumerator("14"),
                new NamedEnumerator("15"),
            };

            var mc = Create();
            foreach (var item in coroutines)
            {
                mc.AddCoroutine(item);
            }
            RefreshDynamic(mc);

            GetTailDynamic(mc).Is(16);
        }


        [TestMethod]
        public void ForceRefreshRandom()
        {
            // random test
            var rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var mc = Create();
                var original = Enumerable.Range(0, rand.Next(1, 200)).Select((x, index) =>
                {
                    return (rand.Next() % 2 == 0) ? null : new NamedEnumerator(index.ToString());
                })
                .ToArray();
                var tailCount = 0;
                foreach (var e in original)
                {
                    if (e != null) tailCount++;
                    mc.AddCoroutine(e);
                }
                RefreshDynamic(mc);
                GetTailDynamic(mc).Is(tailCount);

                var eset = new HashSet<IEnumerator>(original.Where(x => x != null));
                IEnumerator[] inner = GetInnerCoroutineDynamic(mc);
                for (int j = 0; j < tailCount; j++)
                {
                    var item = inner[j];
                    if (!eset.Remove(item))
                    {
                        Assert.Fail("duplicate enumerator");
                    }
                }

                eset.Count.Is(0);
            }
        }
    }
}
