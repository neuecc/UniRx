// defined from .NET Framework 4.5 and NETFX_CORE

#if !(NETFX_CORE || NET_4_6)

using System;

namespace UniRx
{
    public interface IProgress<T>
    {
        void Report(T value);
    }

    public class Progress<T> : IProgress<T>
    {
        readonly Action<T> report;

        public Progress(Action<T> report)
        {
            this.report = report;
        }

        public void Report(T value)
        {
            report(value);
        }
    }
}

#endif