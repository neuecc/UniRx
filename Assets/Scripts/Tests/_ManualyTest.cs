#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)

using UnityEngine;
using RuntimeUnitTestToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace UniRx.Tests
{
    public class _ManualyTest
    {
        public void ToReactiveProperty()
        {
            {
                var rxProp = new ReactiveProperty<int>();
                var calledCount = 0;

                var readRxProp = rxProp.ToReactiveProperty();
                readRxProp.Subscribe(x => calledCount++);

                calledCount.Is(1);
                rxProp.Value = 10;
                calledCount.Is(2);
                rxProp.Value = 10;
                calledCount.Is(2);

                rxProp.SetValueAndForceNotify(10);
                calledCount.Is(2);
            }
            {
                var rxProp = new ReactiveProperty<int>();
                var calledCount = 0;

                var readRxProp = rxProp.ToSequentialReadOnlyReactiveProperty();
                readRxProp.Subscribe(x => calledCount++);

                calledCount.Is(1);
                rxProp.Value = 10;
                calledCount.Is(2);
                rxProp.Value = 10;
                calledCount.Is(2);

                rxProp.SetValueAndForceNotify(10);
                calledCount.Is(3);
            }
        }
    }
}

#endif
