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
                MainThreadDispatcher.Post(x => d(x), state);
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            MainThreadDispatcher.Send(x => d(x), state);
        }
    }
}