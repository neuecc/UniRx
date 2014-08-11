using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public class ObservableLogger : IObservable<LogEntry>
    {
        static readonly Dictionary<string, Logger> loggerList = new Dictionary<string, Logger>();
        static readonly Subject<LogEntry> logPublisher = new Subject<LogEntry>();

        public static readonly ObservableLogger Listener = new ObservableLogger();

        private ObservableLogger()
        {

        }

        public static Action<LogEntry> RegisterLogger(Logger logger)
        {
            lock (loggerList)
            {
                if (logger.Name == null) throw new ArgumentNullException("logger.Name is null");
                if (loggerList.ContainsKey(logger.Name)) throw new ArgumentException("Duplicate Key" + logger.Name);

                loggerList.Add(logger.Name, logger);
            }

            return logPublisher.OnNext;
        }

        public IDisposable Subscribe(IObserver<LogEntry> observer)
        {
            return logPublisher.Subscribe(observer);
        }
    }
}