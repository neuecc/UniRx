using System;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public struct LogEntry
    {
        // requires
        public string LoggerName { get; private set; }
        public LogType LogType { get; private set; }
        public string Message { get; private set; }
        public DateTime Timestamp { get; private set; }

        // options

        /// <summary>[Optional]</summary>
        public UnityEngine.Object Context { get; private set; }
        /// <summary>[Optional]</summary>
        public Exception Exception { get; private set; }
        /// <summary>[Optional]</summary>
        public string StackTrace { get; private set; }
        /// <summary>[Optional]</summary>
        public object State { get; private set; }

        public LogEntry(string loggerName, LogType logType, DateTime timestamp, string message, UnityEngine.Object context = null, Exception exception = null, string stackTrace = null, object state = null)
            : this()
        {
            LoggerName = loggerName;
            LogType = logType;
            Timestamp = timestamp;
            Message = message;
            Context = context;
            Exception = exception;
            StackTrace = stackTrace;
            State = state;
        }

        public override string ToString()
        {
            var plusEx = (Exception != null) ? (Environment.NewLine + Exception.ToString()) : "";
            return "[" + Timestamp.ToString() + "]"
                + "[" + LoggerName + "]"
                + "[" + LogType.ToString() + "]"
                + Message
                + plusEx;
        }
    }
}
