using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnityRx.Tests
{
    [TestClass]
    public class SchedulerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // var scheduler = new ();

            var count = 0;

            //scheduler.Schedule(rec =>
            //{
            //    Console.WriteLine("hoge");
            //    count++;
            //    if (count == 3) return;
            //    rec();
            //});

            //var d = scheduler.Schedule(rec =>
            //{
            //    Console.WriteLine(count++);
            //    if (count != 3)
            //    {
            //        rec();
            //    }
            //});

        }
    }
}
