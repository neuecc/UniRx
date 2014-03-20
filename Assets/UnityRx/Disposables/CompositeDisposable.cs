using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityRx
{
    public class CompositeDisposable : ICollection<IDisposable>, IDisposable
    {
        readonly LinkedList<IDisposable> list = new LinkedList<IDisposable>();

        public bool IsDisposed { get; private set; }

        public void Add(IDisposable item)
        {
            if (!IsDisposed)
            {
                list.AddLast(item);
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
            throw new NotSupportedException();
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