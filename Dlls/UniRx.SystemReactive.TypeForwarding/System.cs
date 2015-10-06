using System;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(IObservable<>))]
[assembly: TypeForwardedTo(typeof(IObserver<>))]

[assembly: TypeForwardedTo(typeof(Tuple))]
[assembly: TypeForwardedTo(typeof(Tuple<>))]
[assembly: TypeForwardedTo(typeof(Tuple<,>))]
[assembly: TypeForwardedTo(typeof(Tuple<,,>))]
[assembly: TypeForwardedTo(typeof(Tuple<,,,>))]
[assembly: TypeForwardedTo(typeof(Tuple<,,,,>))]
[assembly: TypeForwardedTo(typeof(Tuple<,,,,,>))]
[assembly: TypeForwardedTo(typeof(Tuple<,,,,,,>))]
[assembly: TypeForwardedTo(typeof(Tuple<,,,,,,,>))]

[assembly: TypeForwardedTo(typeof(ObservableExtensions))]
