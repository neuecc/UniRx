using System.Collections;

namespace UniRx.Examples
{
    public class Sample2_TypedBehaviour : ObservableMonoBehaviour
    {
        // all message is overridable, it's typesafe
        public override void Update()
        {
            base.Update();
        }

        // use Coroutine, use "new" keyword
        new public IEnumerator Start()
        {
            while (true)
            {
                yield return null;
            }
        }
    }
}