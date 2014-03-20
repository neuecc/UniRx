using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnityRx.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var hoge = new List<int>();
            Observable.Range(1, 10).Select(x => x).Subscribe(x => hoge.Add(x));

            hoge.Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }
    }
}
