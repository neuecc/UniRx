using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniRx
{
    public interface IProgress<T>
    {
        void Report(T value);
    }
}