#pragma warning disable CS1591

using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

public class NewTestScript
{
    [UnityTest]
    public void NewTestScriptSimplePasses()
    {
        Enumerable.Range(1, 10).Is(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    }

    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // await Task.Delay(10);
        100.Is(100);
        yield return null;
    }
}
