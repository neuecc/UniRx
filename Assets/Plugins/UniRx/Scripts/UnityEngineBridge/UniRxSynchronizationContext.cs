#if (ENABLE_MONO_BLEEDING_EDGE_EDITOR || ENABLE_MONO_BLEEDING_EDGE_STANDALONE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UniRx
{
    public class UniRxSynchronizationContext : SynchronizationContext
    {
        static bool autoInstall = true;
        public static bool AutoInstall { get { return autoInstall; } set { autoInstall = value; } }

        public override void Post(SendOrPostCallback d, object state)
        {
            // If is in mainthread, call direct.
            if (MainThreadDispatcher.IsInMainThread)
            {
                d(state);
            }
            else
            {
                MainThreadDispatcher.Post(x =>
                {
                    var pair = (Pair)x;
                    pair.Callback(pair.State);
                }, new Pair(d, state));
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            MainThreadDispatcher.Send(x =>
            {
                var pair = (Pair)x;
                pair.Callback(pair.State);
            }, new Pair(d, state));
        }

        struct Pair
        {
            public readonly SendOrPostCallback Callback;
            public readonly object State;

            public Pair(SendOrPostCallback callback, object state)
            {
                this.Callback = callback;
                this.State = state;
            }
        }
    }
}

#endif