#if UNITY_2018_1_OR_NEWER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using UniRx.Async.Internal;

namespace UniRx.Async
{
    public static class UniTaskLoopRunners
    {
        public struct UniTaskLoopRunnerInitialization { };
        public struct UniTaskLoopRunnerEarlyUpdate { };
        public struct UniTaskLoopRunnerFixedUpdate { };
        public struct UniTaskLoopRunnerPreUpdate { };
        public struct UniTaskLoopRunnerUpdate { };
        public struct UniTaskLoopRunnerPreLateUpdate { };
        public struct UniTaskLoopRunnerPostLateUpdate { };
    }

    public enum PlayerLoopTiming
    {
        Initialization = 0,
        EarlyUpdate = 1,
        FixedUpdate = 2,
        PreUpdate = 3,
        Update = 4,
        PreLateUpdate = 5,
        PostLateUpdate = 6
    }

    public interface IPlayerLoopItem
    {
        bool MoveNext();
    }

    public static class PlayerLoopHelper
    {
        static PlayerLoopRunner[] runners;

        static PlayerLoopSystem[] InsertRunner(Type type, PlayerLoopSystem loopSystem, PlayerLoopRunner runner)
        {
            var runnerLoop = new PlayerLoopSystem
            {
                type = type,
                updateDelegate = runner.Run
            };

            var dest = new PlayerLoopSystem[loopSystem.subSystemList.Length + 1];
            Array.Copy(loopSystem.subSystemList, 0, dest, 1, loopSystem.subSystemList.Length);
            dest[0] = runnerLoop;
            return dest;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (runners != null) return; // already initialized

            var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
            Initialize(ref playerLoop);
        }

        public static void Initialize(ref PlayerLoopSystem playerLoop)
        {
            runners = new PlayerLoopRunner[7];

            var copyList = playerLoop.subSystemList.ToArray();

            copyList[0].subSystemList = InsertRunner(typeof(UniTaskLoopRunners.UniTaskLoopRunnerInitialization), copyList[0], runners[0] = new PlayerLoopRunner());
            copyList[1].subSystemList = InsertRunner(typeof(UniTaskLoopRunners.UniTaskLoopRunnerEarlyUpdate), copyList[1], runners[1] = new PlayerLoopRunner());
            copyList[2].subSystemList = InsertRunner(typeof(UniTaskLoopRunners.UniTaskLoopRunnerFixedUpdate), copyList[2], runners[2] = new PlayerLoopRunner());
            copyList[3].subSystemList = InsertRunner(typeof(UniTaskLoopRunners.UniTaskLoopRunnerPreUpdate), copyList[3], runners[3] = new PlayerLoopRunner());
            copyList[4].subSystemList = InsertRunner(typeof(UniTaskLoopRunners.UniTaskLoopRunnerUpdate), copyList[4], runners[4] = new PlayerLoopRunner());
            copyList[5].subSystemList = InsertRunner(typeof(UniTaskLoopRunners.UniTaskLoopRunnerPreLateUpdate), copyList[5], runners[5] = new PlayerLoopRunner());
            copyList[6].subSystemList = InsertRunner(typeof(UniTaskLoopRunners.UniTaskLoopRunnerPostLateUpdate), copyList[6], runners[6] = new PlayerLoopRunner());

            playerLoop.subSystemList = copyList;
            PlayerLoop.SetPlayerLoop(playerLoop);
        }


        public static void AddAction(PlayerLoopTiming timing, IPlayerLoopItem action)
        {
            runners[(int)timing].AddAction(action);
        }
    }
}

#endif