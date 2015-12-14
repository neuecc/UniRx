using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx.Tests
{
    public static class TestUtil
    {
        public static T[] ToArrayWait<T>(this IObservable<T> source)
        {
            return source.ToArray().Wait();
        }
    }
}
