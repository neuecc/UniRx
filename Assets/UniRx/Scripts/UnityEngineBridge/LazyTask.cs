using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRx
{
    public abstract class LazyTask
    {
        public enum TaskStatus
        {
            WaitingToRun,
            Running,
            Completed,
            Canceled,
            Faulted
        }

        public TaskStatus Status { get; protected set; }

        protected readonly BooleanDisposable cancellation = new BooleanDisposable();

        public abstract Coroutine Start();

        public void Cancel()
        {
            if (Status == TaskStatus.WaitingToRun || Status == TaskStatus.Running)
            {
                Status = TaskStatus.Canceled;
                cancellation.Dispose();
            }
        }

        public static LazyTask<T> FromResult<T>(T value)
        {
            return LazyTask<T>.FromResult(value);
        }


        public static Coroutine WhenAll(params LazyTask[] tasks)
        {
            return WhenAll(tasks.AsEnumerable());
        }

        public static Coroutine WhenAll(IEnumerable<LazyTask> tasks)
        {
            var coroutines = tasks.Select(x => x.Start()).ToArray();

            return MainThreadDispatcher.StartCoroutine(WhenAllCore(coroutines));
        }

        static IEnumerator WhenAllCore(Coroutine[] coroutines)
        {
            foreach (var item in coroutines)
            {
                // wait sequential, but all coroutine is already started, it's parallel
                yield return item;
            }
        }
    }

    public class LazyTask<T> : LazyTask
    {
        readonly IObservable<T> source;

        T result;
        public T Result
        {
            get
            {
                if (Status != TaskStatus.Completed) throw new InvalidOperationException("Task is not completed");
                return result;
            }
        }

        /// <summary>
        /// If faulted stock error. If completed or canceld, returns null.
        /// </summary>
        public Exception Exception { get; private set; }

        public LazyTask(IObservable<T> source)
        {
            this.source = source;
            this.Status = TaskStatus.WaitingToRun;
        }

        public override Coroutine Start()
        {
            if (Status != TaskStatus.WaitingToRun) throw new InvalidOperationException("Task already started");

            Status = TaskStatus.Running;

            var coroutine = source.StartAsCoroutine(
                onResult: x => { result = x; Status = TaskStatus.Completed; },
                onError: ex => { Exception = ex; Status = TaskStatus.Faulted; },
                cancel: new CancellationToken(cancellation));

            return coroutine;
        }

        public override string ToString()
        {
            switch (Status)
            {
                case TaskStatus.WaitingToRun:
                    return "Status:WaitingToRun";
                case TaskStatus.Running:
                    return "Status:Running";
                case TaskStatus.Completed:
                    return "Status:Completed, Result:" + Result.ToString();
                case TaskStatus.Canceled:
                    return "Status:Canceled";
                case TaskStatus.Faulted:
                    return "Status:Faulted, Result:" + Result.ToString();
                default:
                    return "";
            }
        }

        public static LazyTask<T> FromResult(T value)
        {
            var t = new LazyTask<T>(null);
            t.result = value; ;
            t.Status = TaskStatus.Completed;
            return t;
        }
    }

    public static class LazyTaskExtensions
    {
        public static LazyTask<T> ToLazyTask<T>(this IObservable<T> source)
        {
            return new LazyTask<T>(source);
        }
    }
}