using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class YieldTest : MonoBehaviour
{
    public void Start()
    {
        //Observable.Create<Unit>(observer =>
        //{
        //    observer.OnNext(Unit.Default);
        //    observer.OnCompleted();
        //    return Disposable.Empty;
        //})
        //Observable.ReturnUnit()
        //    .Finally(() =>
        //    {
        //        Debug.Log("finally");
        //    })
        //    .Subscribe(_ =>
        //    {
        //        Debug.Log("onnext");
        //    }, () => Debug.Log("comp"));
        this.StartCoroutine(this.SomeCoroutine());
    }

    private IEnumerator SomeCoroutine()
    {
        Debug.Log("begin SomeCoroutine");
        yield return this.SomeObservable().StartAsCoroutine();
        Debug.Log("end SomeCoroutine"); //not called!
    }

    private IObservable<Unit> SomeObservable()
    {
        Debug.Log("begin SomeObservable");
        return Observable.ReturnUnit()
            .Do(_ => Debug.Log("end SomeObservable"));
    }
}
