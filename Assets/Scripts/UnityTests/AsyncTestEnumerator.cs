using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AsyncTestEnumerator : IEnumerator
{
    public object Current { get; }

    public bool MoveNext()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}