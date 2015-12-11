using System;

namespace UniRx.InternalUtil
{
    public class ThreadSafeQueueWorker
    {
        const int InitialSize = 10;

        object gate = new object();
        bool dequing = false;

        int actionListCount = 0;
        Action<object>[] actionList = new Action<object>[InitialSize];
        object[] actionStates = new object[InitialSize];

        int waitingListCount = 0;
        Action<object>[] waitingList = new Action<object>[InitialSize];
        object[] waitingStates = new object[InitialSize];

        public void Enqueue(Action<object> action, object state)
        {
            lock (gate)
            {
                if (dequing)
                {
                    if (waitingList.Length == waitingListCount)
                    {
                        var newArray = new Action<object>[checked(waitingListCount * 2)];
                        var newArrayState = new object[checked(waitingListCount * 2)];
                        Array.Copy(waitingList, newArray, waitingListCount);
                        Array.Copy(waitingStates, newArrayState, waitingListCount);
                        waitingList = newArray;
                        waitingStates = newArrayState;
                    }
                    waitingList[waitingListCount] = action;
                    waitingStates[waitingListCount] = state;
                    waitingListCount++;
                }
                else
                {
                    if (actionList.Length == actionListCount)
                    {
                        var newArray = new Action<object>[checked(actionListCount * 2)];
                        var newArrayState = new object[checked(actionListCount * 2)];
                        Array.Copy(actionList, newArray, actionListCount);
                        Array.Copy(actionStates, newArrayState, actionListCount);
                        actionList = newArray;
                        actionStates = newArrayState;
                    }
                    actionList[actionListCount] = action;
                    actionStates[actionListCount] = state;
                    actionListCount++;
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
                var state = actionStates[i];
                try
                {
                    action(state);
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
                Array.Clear(actionStates, 0, actionListCount);

                var swapTempActionList = actionList;
                var swapTempActionStates = actionStates;

                actionListCount = waitingListCount;
                actionList = waitingList;
                actionStates = waitingStates;

                waitingListCount = 0;
                waitingList = swapTempActionList;
                waitingStates = swapTempActionStates;
            }
        }
    }
}