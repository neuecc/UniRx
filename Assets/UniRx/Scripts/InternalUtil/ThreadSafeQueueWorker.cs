using System;

namespace UniRx.InternalUtil
{
    public class ThreadSafeQueueWorker
    {
        const int InitialSize = 10;

        object gate = new object();
        bool dequing = false;

        int actionListCount = 0;
        Action[] actionList = new Action[InitialSize];

        int waitingListCount = 0;
        Action[] waitingList = new Action[InitialSize];

        public void Enqueue(Action action)
        {
            lock (gate)
            {
                if (dequing)
                {
                    if (waitingList.Length == waitingListCount)
                    {
                        var newArray = new Action[checked(waitingListCount * 2)];
                        Array.Copy(waitingList, newArray, waitingListCount);
                        waitingList = newArray;
                    }
                    waitingList[waitingListCount++] = action;
                }
                else
                {
                    if (actionList.Length == actionListCount)
                    {
                        var newArray = new Action[checked(actionListCount * 2)];
                        Array.Copy(actionList, newArray, actionListCount);
                        actionList = newArray;
                    }
                    actionList[actionListCount++] = action;
                }
            }
        }

        public void ExecuteAll(Action<Exception> unhandledExceptionCallback)
        {
            lock (gate)
            {
                if (actionListCount == 0) return;

                dequing = true;
            }

            for (int i = 0; i < actionListCount; i++)
            {
                var action = actionList[i];

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    unhandledExceptionCallback(ex);
                }
            }

            lock (gate)
            {
                dequing = false;
                Array.Clear(actionList, 0, actionListCount);

                var swapTempActionList = actionList;

                actionListCount = waitingListCount;
                actionList = waitingList;

                waitingListCount = 0;
                waitingList = swapTempActionList;
            }
        }
    }
}