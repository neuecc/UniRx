﻿namespace UniRx.Tests
{
    // Move to Unity Tests...

    //
    //public class ReactriveDictionaryTest
    //{
    //    [Test]
    //    public void RxDictObserve()
    //    {
    //        var dict = new ReactiveDictionary<string, int>();

    //        var count = 0;
    //        DictionaryAddEvent<string, int> addE = default(DictionaryAddEvent<string, int>);
    //        DictionaryRemoveEvent<string, int> removeE = default(DictionaryRemoveEvent<string, int>);
    //        DictionaryReplaceEvent<string, int> replaceE = default(DictionaryReplaceEvent<string, int>);
    //        var resetCount = 0;

    //        dict.ObserveCountChanged().Subscribe(x => count = x);
    //        dict.ObserveAdd().Subscribe(x => addE = x);
    //        dict.ObserveRemove().Subscribe(x => removeE = x);
    //        dict.ObserveReplace().Subscribe(x => replaceE = x);
    //        dict.ObserveReset().Subscribe(x => resetCount += 1);

    //        dict.Add("a", 100);
    //        count.Is(1);
    //        addE.Key.Is("a"); addE.Value.Is(100);

    //        dict.Add("b", 200);
    //        count.Is(2);
    //        addE.Key.Is("b"); addE.Value.Is(200);

    //        count = -1;
    //        dict["a"] = 300;
    //        count.Is(-1); // not fired
    //        addE.Key.Is("b"); // not fired
    //        replaceE.Key.Is("a"); replaceE.OldValue.Is(100); replaceE.NewValue.Is(300);

    //        dict["c"] = 400;
    //        count.Is(3);
    //        replaceE.Key.Is("a"); // not fired
    //        addE.Key.Is("c"); addE.Value.Is(400);

    //        dict.Remove("b");
    //        count.Is(2);
    //        removeE.Key.Is("b"); removeE.Value.Is(200);

    //        count = -1;
    //        dict.Remove("z");
    //        count.Is(-1); // not fired
    //        removeE.Key.Is("b"); // not fired

    //        dict.Clear();
    //        count.Is(0);
    //        resetCount.Is(1);

    //        count = -1;
    //        dict.Clear();
    //        resetCount.Is(2);
    //        count.Is(-1); // not fired
    //    }
    //}
}
