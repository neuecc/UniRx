#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UnityEngine;
using RuntimeUnitTestToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Threading;
using Unity.Collections;

namespace UniRx.Tests
{
    public enum MyMyMyEnum
    {
        Orange, Apple, Grape
    }

    public class _ManualyTest
    {
        public void RPTest2()
        {
            var rp = new ReactiveProperty<MyMyMyEnum>();

            var result = rp.Record();
            result.Values.IsCollection(MyMyMyEnum.Orange);
            rp.Value = MyMyMyEnum.Apple;
            result.Values.IsCollection(MyMyMyEnum.Orange, MyMyMyEnum.Apple);
            rp.Value = MyMyMyEnum.Apple;
            result.Values.IsCollection(MyMyMyEnum.Orange, MyMyMyEnum.Apple);
            rp.Value = MyMyMyEnum.Grape;
            result.Values.IsCollection(MyMyMyEnum.Orange, MyMyMyEnum.Apple, MyMyMyEnum.Grape);
        }
    }


}

#endif