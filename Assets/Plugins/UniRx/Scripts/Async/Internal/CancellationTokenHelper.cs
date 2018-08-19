#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;

namespace UniRx.Async.Internal
{
    public static class CancellationTokenHelper
    {
        public static bool TrySetOrLinkCancellationToken(ref CancellationToken field, CancellationToken newCancellationToken)
        {
            if (newCancellationToken == CancellationToken.None)
            {
                return false;
            }
            else if (field == CancellationToken.None)
            {
                field = newCancellationToken;
                return true;
            }
            else if (field == newCancellationToken)
            {
                return false;
            }

            field = CancellationTokenSource.CreateLinkedTokenSource(field, newCancellationToken).Token;
            return true;
        }
    }
}

#endif