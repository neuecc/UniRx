using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace OfficialRx
{
    // [Ignore]
    [TestClass]
    public class DisposableTestCopy
    {
        [TestMethod]
        public void SingleAssignmentRxOfficial()
        {
            var d = new SingleAssignmentDisposable();
            d.IsDisposed.IsFalse();
            var id1 = new IdDisp(1);
            var id2 = new IdDisp(2);
            var id3 = new IdDisp(3);

            // dispose first
            d.Dispose();
            d.IsDisposed.IsTrue();

            d.Disposable = id1; id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // normal flow
            d = new SingleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);

            d.Disposable = id1; id1.IsDisposed.IsFalse();
            d.Dispose();
            id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // exception flow
            d = new SingleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);
            d.Disposable = id1;
            AssertEx.Catch<InvalidOperationException>(() => d.Disposable = id2);

            // null
            d = new SingleAssignmentDisposable();
            id1 = new IdDisp(1);
            d.Disposable = null;
            d.Dispose();
            d.Disposable = null;
        }

        [TestMethod]
        public void MultipleAssignmentRxOfficial()
        {
            var d = new MultipleAssignmentDisposable();
            d.IsDisposed.IsFalse();
            var id1 = new IdDisp(1);
            var id2 = new IdDisp(2);
            var id3 = new IdDisp(3);

            // dispose first
            d.Dispose();
            d.IsDisposed.IsTrue();

            d.Disposable = id1; id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // normal flow
            d = new MultipleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);

            d.Disposable = id1; id1.IsDisposed.IsFalse();
            d.Dispose();
            id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // exception flow
            d = new MultipleAssignmentDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);
            d.Disposable = id1;
            d.Disposable = id2;
            d.Disposable = id3;
            d.Dispose();
            id1.IsDisposed.IsFalse();
            id2.IsDisposed.IsFalse();
            id3.IsDisposed.IsTrue();

            // null
            d = new MultipleAssignmentDisposable();
            id1 = new IdDisp(1);
            d.Disposable = null;
            d.Dispose();
            d.Disposable = null;
        }

        [TestMethod]
        public void SerialRxOfficial()
        {
            var d = new SerialDisposable();
            d.IsDisposed.IsFalse();
            var id1 = new IdDisp(1);
            var id2 = new IdDisp(2);
            var id3 = new IdDisp(3);

            // dispose first
            d.Dispose();
            d.IsDisposed.IsTrue();

            d.Disposable = id1; id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // normal flow
            d = new SerialDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);

            d.Disposable = id1; id1.IsDisposed.IsFalse();
            d.Dispose();
            id1.IsDisposed.IsTrue();
            d.Disposable = id2; id2.IsDisposed.IsTrue();
            d.Disposable = id3; id3.IsDisposed.IsTrue();

            // exception flow
            d = new SerialDisposable();
            id1 = new IdDisp(1);
            id2 = new IdDisp(2);
            id3 = new IdDisp(3);
            d.Disposable = id1;
            id1.IsDisposed.IsFalse();
            d.Disposable = id2;
            id1.IsDisposed.IsTrue();
            id2.IsDisposed.IsFalse();
            d.Disposable = id3;
            id2.IsDisposed.IsTrue();
            id3.IsDisposed.IsFalse();

            d.Dispose();

            id3.IsDisposed.IsTrue();

            // null
            d = new SerialDisposable();
            id1 = new IdDisp(1);
            d.Disposable = null;
            d.Dispose();
            d.Disposable = null;
        }

        [TestMethod]
        public void BooleanRxOfficial()
        {
            var bd = new BooleanDisposable();
            bd.IsDisposed.IsFalse();
            bd.Dispose();
            bd.IsDisposed.IsTrue();
        }


        class IdDisp : IDisposable
        {
            public bool IsDisposed { get; set; }
            public int Id { get; set; }

            public IdDisp(int id)
            {
                this.Id = id;
            }


            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }



}
