using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests.Operators
{
    [TestClass]
    public class ToTest
    {

        [TestMethod]
        public void ToArray()
        {
            Observable.Empty<int>().ToArray().Wait().Is();
            Observable.Return(10).ToArray().Wait().Is(10);
            Observable.Range(1, 10).ToArray().Wait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [TestMethod]
        public void ToList()
        {
            Observable.Empty<int>().ToList().Wait().Is();
            Observable.Return(10).ToList().Wait().Is(10);
            Observable.Range(1, 10).ToList().Wait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }
    }
}
