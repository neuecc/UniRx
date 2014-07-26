using System;
using System.Collections;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample5_ConvertToCoroutine : TypedMonoBehaviour
    {
        // convert IObservable to Coroutine
        new public IEnumerator Start()
        {
            var v = default(int);
            yield return Observable.Range(1, 10).StartAsCoroutine(x => v = x);

            Debug.Log(v); // 10(callback is last value)
        }
    }
}