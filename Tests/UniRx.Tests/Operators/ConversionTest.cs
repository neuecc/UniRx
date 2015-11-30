using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [TestClass]
    public class ConversionTest
    {
        [TestMethod]
        public void AsObservable()
        {
            Observable.Range(1, 10).AsObservable().ToArrayWait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [TestMethod]
        public void AsUnitObservable()
        {
            Observable.Range(1, 3).AsUnitObservable().ToArrayWait().Is(Unit.Default, Unit.Default, Unit.Default);
        }
    }
}
