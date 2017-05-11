using System;

namespace UniRx
{
    public static partial class Observable
    {
        // Adapted from: https://github.com/Reactive-Extensions/Rx.NET/blob/371c83c621562a2259580a03f0cb5bf8680ea720/Rx.NET/Source/System.Reactive.Linq/Reactive/Linq/QueryLanguage.Creation.cs#L417-L441
        public static IObservable<TSource> Using<TSource, TResource>(
            Func<TResource> resourceFactory,
            Func<TResource, IObservable<TSource>> observableFactory)
            where TResource : IDisposable
        {
            return Create<TSource>(observer =>
            {
                IObservable<TSource> source;
                var disposable = Disposable.Empty;
                try
                {
                    var resource = resourceFactory();
                    if (resource != null)
                        disposable = resource;

                    source = observableFactory(resource);
                }
                catch (Exception exception)
                {
                    return new CompositeDisposable(
                        Throw<TSource>(exception).Subscribe(observer),
                        disposable);
                }

                return new CompositeDisposable(
                    source.Subscribe(observer),
                    disposable);
            });
        }
    }
}