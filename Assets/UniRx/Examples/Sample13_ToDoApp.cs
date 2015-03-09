// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace UniRx.Examples
{
    public class Sample13_ToDoApp : MonoBehaviour
    {
        public InputField ToDoInput;
        public Button AddButton;


        [InspectorDisplay]
        IntReactiveProperty ItemCount = new IntReactiveProperty();

        void Start()
        {
            // merge Button click and push enter key on input field.
            var submit = Observable.Merge(
                AddButton.OnClickAsObservable().Select(_ => ToDoInput.text),
                ToDoInput.OnEndEditAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Return)));

            submit.Where(x => x != "")
                  .Subscribe(x =>
                  {
                      ToDoInput.text = ""; // clear input field
                      ItemCount.Value += 1;
                      // TODO:Add to list
                  });
        }
    }
}

#endif