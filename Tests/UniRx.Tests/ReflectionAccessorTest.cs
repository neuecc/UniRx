using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniRx.InternalUtil;
using System.Linq.Expressions;

namespace UniRx.Tests
{
    [TestClass]
    public class ReflectionAccessorTest
    {
        public class MyClass
        {
            public int MyProperty { get; set; }
            public Depth2 Depth2 { get; set; }
        }

        public struct Depth2
        {
            public int MyProperty { get; set; }
            public Depth3 Depth3 { get; set; }
        }

        public class Depth3
        {
            public int MyProperty { get; set; }
        }

        [TestMethod]
        public void GetValue()
        {
            var mc = new MyClass
            {
                MyProperty = 100,
                Depth2 = new Depth2
                {
                    MyProperty = 1000,
                    Depth3 = new Depth3
                    {
                        MyProperty = 10000
                    }
                }
            };

            {
                Expression<Func<MyClass, int>> selector = x => x.MyProperty;
                var accessor = ReflectionAccessor.Create(selector.Body as MemberExpression);
                accessor.GetValue(mc).Is(100);
            }

            {
                Expression<Func<MyClass, int>> selector = x => x.Depth2.MyProperty;
                var accessor = ReflectionAccessor.Create(selector.Body as MemberExpression);
                accessor.GetValue(mc).Is(1000);
            }

            {
                Expression<Func<MyClass, int>> selector = x => x.Depth2.Depth3.MyProperty;
                var accessor = ReflectionAccessor.Create(selector.Body as MemberExpression);
                accessor.GetValue(mc).Is(10000);
            }
        }
    }
}
