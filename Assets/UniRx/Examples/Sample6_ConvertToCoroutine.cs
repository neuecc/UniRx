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
            var enumerator = Observable.Range(1, 10)
              .ToAwaitableEnumerator(
                  x => Debug.Log(x),
                  ex => Debug.Log(ex),
                  () => Debug.Log("completed"));

            yield return StartCoroutine(enumerator);
        }
    }
}