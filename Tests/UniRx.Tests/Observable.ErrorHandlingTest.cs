using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniRx.Tests
{
    [TestClass]
    public class ErrorHandlingTest
    {
        [TestMethod]
        public void Finally()
        {
            var called = false;
            try
            {
                Observable.Range(1, 10, Scheduler.Immediate)
                    .Do(x => { throw new Exception(); })
                    .Finally(() => called = true)
                    .Subscribe();
            }
            catch
            {
            }
            finally
            {
                called.IsTrue();
            }
        }

        [TestMethod]
        public void Catch()
        {
            var xs = Observable.Return(2, Scheduler.ThreadPool).Concat(Observable.Throw<int>(new InvalidOperationException()))
                .Catch((InvalidOperationException ex) =>
                {
                    return Observable.Range(1, 3);
                })
                .ToArrayWait();

            xs.Is(2, 1, 2, 3);
        }

        [TestMethod]
        public void CatchEnumerable()
        {
            {
                var xs = new[]
                {
                    Observable.Return(2).Concat(Observable.Throw<int>(new Exception())),
                    Observable.Return(99).Concat(Observable.Throw<int>(new Exception())),
                    Observable.Range(10,2)
                }
                .Catch()
                .Materialize()
                .ToArrayWait();

                xs[0].Value.Is(2);
                xs[1].Value.Is(99);
                xs[2].Value.Is(10);
                xs[3].Value.Is(11);
                xs[4].Kind.Is(NotificationKind.OnCompleted);
            }
            {
                var xs = new[]
                {
                    Observable.Return(2).Concat(Observable.Throw<int>(new Exception())),
                    Observable.Return(99).Concat(Observable.Throw<int>(new Exception()))
                }
                .Catch()
                .Materialize()
                .ToArrayWait();

                xs[0].Value.Is(2);
                xs[1].Value.Is(99);
                xs[2].Kind.Is(NotificationKind.OnError);
            }
        }
    }
}
