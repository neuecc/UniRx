using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnityRx.Tests
{
    [TestClass]
    public class ObservableGeneratorTest
    {
        [TestMethod]
        public void Range()
        {
            Observable.Range(1, 5).ToArray().Wait().Is(1, 2, 3, 4, 5);

        }
    }
}
