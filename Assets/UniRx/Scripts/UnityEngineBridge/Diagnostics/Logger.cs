using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.Diagnostics
{
    public partial class Logger
    {
        static bool isInitialized = false;
        static bool isDebugBuild = false;

        public string Name { get; private set; }
        protected readonly Action<LogEntry> logPublisher;

        public Logger(string loggerName)
        {
            this.Name = loggerName;
            this.logPublisher = ObservableLogger.RegisterLogger(this);
        }

        public virtual void Debug(object message, UnityEngine.Object context = null)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                isDebugBuild = UnityEngine.Debug.isDebugBuild;
            }

            if (isDebugBuild)
            {
                logPublisher(new LogEntry(
                    message: message.ToString(),
                    logType: LogType.Log,
                    timestamp: DateTime.Now,
                    loggerName: Name,
                    context: context));
            }
        }

        public virtual void Log(object message, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: message.ToString(),
                logType: LogType.Log,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }

        public virtual void Warning(object message, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: message.ToString(),
                logType: LogType.Warning,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }

        public virtual void Error(object message, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: message.ToString(),
                logType: LogType.Error,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }

        public virtual void Exception(Exception exception, UnityEngine.Object context = null)
        {
            logPublisher(new LogEntry(
                message: exception.ToString(),
                exception: exception,
                logType: LogType.Exception,
                timestamp: DateTime.Now,
                loggerName: Name,
                context: context));
        }
    }
}