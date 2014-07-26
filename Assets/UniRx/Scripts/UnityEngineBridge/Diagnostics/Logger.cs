using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public partial class Logger
    {
        static readonly bool IsDebugBuild = UnityEngine.Debug.isDebugBuild;

        public string Name { get; private set; }
        protected readonly Action<LogEntry> logPublisher;

        public Logger(string loggerName)
        {
            this.Name = loggerName;
            this.logPublisher = ObservableLogger.RegisterLogger(this);
        }

        public virtual void Debug(object message, UnityEngine.Object context = null)
        {
            if (IsDebugBuild)
            {
                logPublisher(new LogEntry(
                    message: message.ToString(),
                    logType: LogType.Log,
                    time: DateTime.Now,
                    name: Name,
                    context: context));
            }
        }

        public virtual void Log(object message, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: message.ToString(),
                logType: LogType.Log,
                time: DateTime.Now,
                name: Name,
                context: context));
        }

        public virtual void Warning(object message, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: message.ToString(),
                logType: LogType.Warning,
                time: DateTime.Now,
                name: Name,
                context: context));
        }

        public virtual void Error(object message, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: message.ToString(),
                logType: LogType.Error,
                time: DateTime.Now,
                name: Name,
                context: context));
        }

        public virtual void Exception(Exception exception, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: exception.ToString(),
                exception: exception,
                logType: LogType.Exception,
                time: DateTime.Now,
                name: Name,
                context: context));
        }
    }
}