using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniRx.Tests
{
    [TestClass]
    public class ReactivePropertyTest
    {
        [TestMethod]
        public void ValueType()
        {
            {
                var rp = new ReactiveProperty<int>(); // 0

                var result = rp.Record();
                result.Values.IsCollection(0);

                rp.Value = 0;
                result.Values.IsCollection(0);

                rp.Value = 10;
                result.Values.IsCollection(0, 10);

                rp.Value = 100;
                result.Values.IsCollection(0, 10, 100);

                rp.Value = 100;
                result.Values.IsCollection(0, 10, 100);
            }
            {
                var rp = new ReactiveProperty<int>(20);

                var result = rp.Record();
                result.Values.IsCollection(20);

                rp.Value = 0;
                result.Values.IsCollection(20, 0);

                rp.Value = 10;
                result.Values.IsCollection(20, 0, 10);

                rp.Value = 100;
                result.Values.IsCollection(20, 0, 10, 100);

                rp.Value = 100;
                result.Values.IsCollection(20, 0, 10, 100);
            }
        }

        [TestMethod]
        public void ClassType()
        {
            {
                var rp = new ReactiveProperty<string>(); // null

                var result = rp.Record();
                result.Values.IsCollection((string)null);

                rp.Value = null;
                result.Values.IsCollection((string)null);

                rp.Value = "a";
                result.Values.IsCollection((string)null, "a");

                rp.Value = "b";
                result.Values.IsCollection((string)null, "a", "b");

                rp.Value = "b";
                result.Values.IsCollection((string)null, "a", "b");
            }
            {
                var rp = new ReactiveProperty<string>("z");

                var result = rp.Record();
                result.Values.IsCollection("z");

                rp.Value = "z";
                result.Values.IsCollection("z");

                rp.Value = "a";
                result.Values.IsCollection("z", "a");

                rp.Value = "b";
                result.Values.IsCollection("z", "a", "b");

                rp.Value = "b";
                result.Values.IsCollection("z", "a", "b");

                rp.Value = null;
                result.Values.IsCollection("z", "a", "b", null);
            }
        }
    }
}
