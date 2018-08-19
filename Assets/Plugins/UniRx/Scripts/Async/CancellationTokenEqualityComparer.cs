#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if (CSHARP_7_OR_LATER)
using System.Collections.Generic;
using System.Threading;

namespace UniRx.Async
{
    public class CancellationTokenEqualityComparer : IEqualityComparer<CancellationToken>
    {
        public static readonly IEqualityComparer<CancellationToken> Default = new CancellationTokenEqualityComparer();

        public bool Equals(CancellationToken x, CancellationToken y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(CancellationToken obj)
        {
            return obj.GetHashCode();
        }
    }
}

#endif