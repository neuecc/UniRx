using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace UniRx.Tests.Operators
{
    
    public class ToTest
    {

        [Test]
        public void ToArray()
        {
            Observable.Empty<int>().ToArray().Wait().Is();
            Observable.Return(10).ToArray().Wait().Is(10);
            Observable.Range(1, 10).ToArray().Wait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [Test]
        public void ToList()
        {
            Observable.Empty<int>().ToList().Wait().Is();
            Observable.Return(10).ToList().Wait().Is(10);
            Observable.Range(1, 10).ToList().Wait().Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }
    }
}
