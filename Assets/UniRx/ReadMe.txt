UniRx - Reactive Extensions for Unity
===
Created by Yoshifumi Kawai(neuecc)

What is UniRx?
---
UniRx(Reactive Extensions for Unity) is re-implementation of .NET Reactive Extensions.    
It's free and open source on GitHub.
You can check latest info, source code and issues on https://github.com/neuecc/UniRx
We welcome to your contribute such as bug report, request, and pull request.

Unity Forums support thread, ask me any questions - http://forum.unity3d.com/threads/248535-UniRx-Reactive-Extensions-for-Unity

Other support, see and write GitHub issues 
https://github.com/neuecc/UniRx/issues

Last, send me an email at: ils@neue.cc

Why Rx?
---
Ordinary, Unity Network operation use `WWW` and `Coroutine` but `Coroutine` is not good practice for asynchronous operation.
Let me show some example.
1. Coroutine can't return result value. (because return type should be IEnumerator)
2. It also won't handle exception. (because yield return can't surrond with try-catch)
These lack of composability cause Operation close-coupled, and we have to write huge monolithic IEnumerator.
Rx curing asynchronous blues like that. Rx is a library to compose asynchronous and event-based programs using observable collections and LINQ-style query operators. 
  
GameLoop(every Update, OnCollisionEnter, etc), Sensor(like Kinect, Leap Motion, etc) is all of event.
Rx considere event as reactive sequence which is possible to compose and perform time-based operations easily by using many LINQ query operators.

Unity is single thread but UniRx helps multithreading for join, cancel, access GameObject etc.        

How to Use for WWW
---
async operation, use ObservableWWW, it's Get/Post returns IObservable.

```csharp
ObservableWWW.Get("http://google.co.jp/")
    .Subscribe(
        x => Debug.Log(x), // onSuccess
        ex => Debug.LogException(ex)); // onError
```

Rx is composable, cancelable and can query with LINQ query expressions.

```csharp
// composing asynchronous sequence with LINQ query expressions
var query = from google in ObservableWWW.Get("http://google.com/")
            from bing in ObservableWWW.Get("http://bing.com/")
            from unknown in ObservableWWW.Get(google + bing)
            select new { google, bing, unknown };

var cancel = query.Subscribe(x => Debug.Log(x));

// Call Dispose is cancel.
cancel.Dispose();
```

Parallel request use Observable.WhenAll
```csharp
// Observable.WhenAll is for parallel asynchronous operation
// (It's like Observable.Zip but specialized for single async operations like Task.WhenAll)
var parallel = Observable.WhenAll(
        ObservableWWW.Get("http://google.com/"),
        ObservableWWW.Get("http://bing.com/"),
        ObservableWWW.Get("http://yahoo.com/"));

parallel.Subscribe(xs =>
{
    Debug.Log(xs[0]); // google
    Debug.Log(xs[1]); // bing
    Debug.Log(xs[2]); // yahoo
});
```

with progress
```csharp
// notifier for progress
var progressNotifier = new ScheduledNotifier<float>();
progressNotifier.Subscribe(x => Debug.Log(x)); // write www.progress

// pass notifier to WWW.Get/Post
ObservableWWW.Get("http://google.com/", progress: progressNotifier).Subscribe();
```

Error Handling
```csharp
// If WWW has .error, ObservableWWW throws WWWErrorException to onError pipeline.
// WWWErrorException has RawErrorMessage, HasResponse, StatusCode, ResponseHeaders
ObservableWWW.Get("http://www.google.com/404")
    .CatchIgnore((WWWErrorException ex) =>
    {
        Debug.Log(ex.RawErrorMessage);
        if (ex.HasResponse)
        {
            Debug.Log(ex.StatusCode);
        }
        foreach (var item in ex.ResponseHeaders)
        {
            Debug.Log(item.Key + ":" + item.Value);
        }
    })
    .Subscribe();
```

How to Use for IEnumerator(as Coroutine)
---
IEnumerator(Coroutine) is Unity's primitive asynchronous tool. UniRx integrates coroutine and IObservable. Write async in coroutine, Orchestrate many coroutines by UniRx is best way for control asynchronous flow.

```csharp
// two coroutines

IEnumerator AsyncA()
{
    Debug.Log("a start");
    yield return new WaitForSeconds(1);
    Debug.Log("a end");
}

IEnumerator AsyncB()
{
    Debug.Log("b start");
    yield return new WaitForEndOfFrame();
    Debug.Log("b end");
}

// main code
// Observable.FromCoroutine convert IEnumerator to Observable<Unit>.
// other shorthand, AsyncA().ToObservable()
        
// after completed AsyncA, run AsyncB as continuous routine.
// UniRx expands SelectMany(IEnumerator) as SelectMany(IEnumerator.ToObservable())
var cancel = Observable.FromCoroutine(AsyncA)
    .SelectMany(_ => AsyncB())
    .Subscribe();

// you can stop coroutine use subscription's Dispose.
cancel.Dispose();
```

If you needs coroutine with return value, normally we use callback. Observable.FromCoroutine supports convert coroutine to IObservable[T] with cancellation.

```csharp
// public method
public static IObservable<string> GetWWW(string url)
{
    // convert coroutine to IObservable
    return Observable.FromCoroutine<string>((observer, cancellationToken) => GetWWWCore(url, observer, cancellationToken));
}

// IObserver is callback publisher
// note: Principles of IObserver is "OnNext* (OnError | Oncompleted)?" 
static IEnumerator GetWWWCore(string url, IObserver<string> observer, CancellationToken cancellationToken)
{
    var www = new UnityEngine.WWW(url);
    while (!www.isDone && !cancellationToken.IsCancellationRequested)
    {
        yield return null;
    }

    if (cancellationToken.IsCancellationRequested) yield break;

    if (www.error != null)
    {
        observer.OnError(new Exception(www.error));
    }
    else
    {
        observer.OnNext(www.text);
        observer.OnCompleted(); // IObserver needs OnCompleted after OnNext!
    }
}
```

more application examples, following is multiple OnNext pattern.

```csharp
public static IObservable<float> ToObservable(this UnityEngine.AsyncOperation asyncOperation)
{
    if (asyncOperation == null) throw new ArgumentNullException("asyncOperation");

    return Observable.FromCoroutine<float>((observer, cancellationToken) => RunAsyncOperation(asyncOperation, observer, cancellationToken));
}

static IEnumerator RunAsyncOperation(UnityEngine.AsyncOperation asyncOperation, IObserver<float> observer, CancellationToken cancellationToken)
{
    while (!asyncOperation.isDone && !cancellationToken.IsCancellationRequested)
    {
        observer.OnNext(asyncOperation.progress);
        yield return null;
    }
    if (!cancellationToken.IsCancellationRequested)
    {
        observer.OnNext(asyncOperation.progress); // push 100%
        observer.OnCompleted();
    }
}

// usecase
Application.LoadLevelAsync("testscene")
    .ToObservable()
    .Do(x => Debug.Log(x)) // output progress
    .Last() // last sequence is load completed
    .Subscribe();
```

How to Use for MultiThreading
---

```csharp
// Observable.Start is start factory methods on specified scheduler
// default is on ThreadPool
var heavyMethod = Observable.Start(() =>
{
    // heavy method...
    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
    return 10;
});

var heavyMethod2 = Observable.Start(() =>
{
    // heavy method...
    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
    return 10;
});

// Join and await two other thread values
Observable.WhenAll(heavyMethod, heavyMethod2)
    .ObserveOnMainThread() // return to main thread
    .Subscribe(xs =>
    {
        // Unity can't touch GameObject from other thread
        // but use ObserveOnMainThread, you can touch GameObject naturally.
        (GameObject.Find("myGuiText")).guiText.text = xs[0] + ":" + xs[1];
    }); 
```

How to Use for MonoBehaviour
---
UniRx has two extended MonoBehaviour. TypedMonoBehaviour is typesafe MonoBehaviour.

```csharp
public class Test : TypedMonoBehaviour
{
    // all message is overridable, it's typesafe
    public override void Update()
    {
        base.Update();
    }

    // use Coroutine, use "new" keyword
    new public IEnumerator Start()
    {
        while (true)
        {
            yield return null;
        }
    }
}
```

ObservableMonoBehaviour is extended TypedMonoBehaviour. All message is Observable.

```csharp
public class Test : ObservableMonoBehaviour
{
    public override void Awake()
    {
        // All events can subscribe by ***AsObservable
        this.OnMouseDownAsObservable()
            .SelectMany(_ => this.UpdateAsObservable())
            .TakeUntil(this.OnMouseUpAsObservable())
            .Select(_ => Input.mousePosition)
            .Repeat()
            .Subscribe(x => Debug.Log(x));

        // If you use ObservableMonoBehaviour, must call base method
        base.Awake();
    }
}
```

Convert Unity callback to IObservable
---
Use Subject(or AsyncSubject for async operation). For example...

```csharp
public class LogCallback
{
    public string Condition;
    public string StackTrace;
    public UnityEngine.LogType LogType;
}

public static class LogHelper
{
    static Subject<LogCallback> subject;

    public static IObservable<LogCallback> LogCallbackAsObservable()
    {
        if (subject == null)
        {
            subject = new Subject<LogCallback>();

            // Publish to Subject in callback
            UnityEngine.Application.RegisterLogCallback((condition, stackTrace, type) =>
            {
                subject.OnNext(new LogCallback { Condition = condition, StackTrace = stackTrace, LogType = type });
            });
        }

        return subject.AsObservable();
    }
}

// method is separatable and composable
LogHelper.LogCallbackAsObservable()
    .Where(x => x.LogType == LogType.Warning)
    .Subscribe();

LogHelper.LogCallbackAsObservable()
    .Where(x => x.LogType == LogType.Error)
    .Subscribe();
```

Unity specified extra gems
---
```csharp
// Unity's singleton UiThread Queue Scheduler
Scheduler.MainThreadScheduler 
ObserveOnMainThread()/SubscribeOnMainThread()

// Global StartCoroutine runner
MainThreadDispatcher.StartCoroutine(enumerator)

// push value on every update time
Observable.EveryUpdate().Subscribe();

// push value on every fixedUpdate time
Observable.EveryFixedUpdate().Subscribe();

// delay on frame time
Observable.Return(42).DelayFrame(10);

// convert Coroutine to IObservable
Observable.FromCoroutine((observer, token) => enumerator(observer, token)); 

// convert IObservable to Coroutine
Observable.Range(1, 10).ToCoroutine();
```

vs iOS AOT
---
UniRx has AotSafe Utilities.

```csharp
// create safety iterator
Enumerable.Range(1, 10).AsSafeEnumerable().ToArray();

// elements wrap to class
Enumerable.Range(1, 10).WrapValueToClass(); // IEnumerable<Tuple<int>>
Observable.Range(1, 10).WrapValueToClass(); // IObservable<Tuple<int>>
```

Please see [AOT Exception Patterns and Hacks](https://github.com/neuecc/UniRx/wiki/AOT-Exception-Patterns-and-Hacks).

Reference
---
RxJava Wiki | https://github.com/Netflix/RxJava/wiki
This wiki is recommended way for learn Rx.
You can understand behavior of all operators by graphical marble diagram.

Reactive Game Architectures | http://sugarpillstudios.com/wp/?page_id=279
Introduction to how to use Rx for Game. 

Introduction to Rx | http://introtorx.com/
Great online tutorial and eBook.

Rx(Reactive Extensions) | https://rx.codeplex.com/
Original project home.

Beginner's Guide to the Reactive Extensions | http://msdn.microsoft.com/en-us/data/gg577611
Many Videos and slides and documents.

Author Info
---
Yoshifumi Kawai(a.k.a. neuecc) is software developer in Japan.
He is Director/CTO at Grani, Inc.
Grani is top social game developer in Japan. 
He awarded Microsoft MVP for Visual C# since 2011.
He is known by creator of [linq.js](http://linqjs.codeplex.com/)(LINQ to Objects for JavaScript)

Mail: ils@neue.cc
Blog: http://neue.cc/ (JPN)
Twitter: https://twitter.com/neuecc (JPN)
Facebook: http://facebook.com/neuecc (JPN)