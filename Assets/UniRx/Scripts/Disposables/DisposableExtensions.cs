using System;
using System.Collections.Generic;

namespace UniRx
{
    public static class DisposableExtensions
    {
        /// <summary>Add disposable(self) to CompositeDisposable(or other ICollection)</summary>
        public static void AddTo(this IDisposable disposable, ICollection<IDisposable> container)
        {
            container.Add(disposable);
        }
    }
}