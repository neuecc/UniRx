using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public class LogEntry
    {
        // must
        public string Name { get; private set; }
        public LogType LogType { get; private set; }
        public string Message { get; private set; }
        public DateTime Time { get; private set; }

        // option
        public UnityEngine.Object Context { get; private set; }
        public Exception Exception { get; private set; }
        public string StackTrace { get; private set; }

        public LogEntry(string name, LogType logType, DateTime time, string message, UnityEngine.Object context = null, Exception exception = null, string stackTrace = null)
        {
            this.Name = name;
            this.LogType = LogType;
            this.Time = time;
            this.Message = message;
            this.Context = context;
            this.Exception = exception;
            this.StackTrace = StackTrace;
        }

        public override string ToString()
        {
            return "[" + Time.ToString() + "]"
                + "[" + Name + "]"
                + "[" + LogType.ToString() + "]"
                + Message;
        }
    }
}
