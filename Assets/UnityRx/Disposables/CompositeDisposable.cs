using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    public class CompositeDisposable : ICollection<IDisposable>, IDisposable
    {
        readonly List<IDisposable> list;

        public bool IsDisposed { get; private set; }

        public CompositeDisposable()
        {
            list = new List<IDisposable>();
        }

        public CompositeDisposable(int capacity)
        {
            list = new List<IDisposable>(capacity);
        }

        public CompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            list = new List<IDisposable>(disposables);
        }

        public CompositeDisposable(params IDisposable[] disposables)
        {
            list = new List<IDisposable>(disposables);
        }

        public void Add(IDisposable item)
        {
            if (!IsDisposed)
            {
                list.Add(item);
            }
            else
            {
                item.Dispose();
            }
        }

        public void Clear()
        {
            if (!IsDisposed)
            {
                foreach (var item in list)
                {
                    item.Dispose();
                }
                list.Clear();
            }
        }

        public bool Contains(IDisposable item)
        {
            return list.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IDisposable item)
        {
            return list.Remove(item);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<IDisposable> IEnumerable<IDisposable>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Clear();
            }
        }
    }
}