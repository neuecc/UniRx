using System;
using System.Collections;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample06_ConvertToCoroutine : MonoBehaviour
    {
        // convert IObservable to Coroutine
        void Start()
        {
            StartCoroutine(ComplexCoroutineTest());
        }

        IEnumerator ComplexCoroutineTest()
        {
            yield return new WaitForSeconds(1);

            var v = default(int);
            yield return Observable.Range(1, 10).StartAsCoroutine(x => v = x);

            Debug.Log(v); // 10(callback is last value)
            yield return new WaitForSeconds(3);

            yield return Observable.Return(100).StartAsCoroutine(x => v = x);

            Debug.Log(v); // 100
        }

        // like WWW.text/error, LazyTask is awaitable value container
        IEnumerator LazyTaskTest()
        {
            // IObservable<T> to LazyTask
            var task = Observable.Start(() => 100).ToLazyTask();

            yield return task.Start(); // wait for OnCompleted

            Debug.Log(task.Result); // or task.Exception
        }
    }
}