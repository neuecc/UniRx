using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public class ConsoleSink : IObserver<LogEntry>
    {
        public void OnCompleted()
        {
            // do nothing
        }

        public void OnError(Exception error)
        {
            // do nothing
        }

        public void OnNext(LogEntry value)
        {
            Console.WriteLine(value.ToString());
        }
    }
}