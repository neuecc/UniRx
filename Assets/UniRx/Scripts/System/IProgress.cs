// defined from .NET Framework 4.5 and NETFX_CORE

namespace UniRx
{
    public interface IProgress<T>
    {
        void Report(T value);
    }
}