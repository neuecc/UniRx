using System;
using System.Collections;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample06_ConvertToCoroutine : TypedMonoBehaviour
    {
        // convert IObservable to Coroutine
        new public IEnumerator Start()
        {
            var v = default(int);

            yield return Observable.Range(1, 10).StartAsCoroutine(x => v = x);

            Debug.Log(v); // 10(callback is last value)
        }

        // like WWW.text/error, LazyTask is awaitable value container
        IEnumerator LazyTaskTest()
        {
            // IObservable<T>
            var task = Observable.Start(() => 100).ToLazyTask();

            yield return task.Start(); // wait for OnCompleted

            Debug.Log(task.Result); // or task.Exception
        }
    }
}