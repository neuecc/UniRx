using UnityEngine;
using UniRx.UI;
using System.Collections;
using UniRx;
using System.Threading;
using System;
using System.Text;

// test sandbox
public class NewBehaviourScript : ObservableMonoBehaviour
{
    // IDisposable _____cancel;

    public class MyClassPropertyField
    {
        public int Property { get; set; }
        public int Field { get; set; }
    }

    GameObject text;

    public override void Awake()
    {
        text = GameObject.Find("myGuiText");

        //var mc = new MyClassPropertyField { Field = 10, Property = 100 };


        //mc.ObserveEveryValueChanged(x => x.Field)
        //    .Subscribe(x => text.guiText.text = x.ToString());

        //Observable.Interval(TimeSpan.FromSeconds(1))
        //    .Subscribe(x => mc.Field = (int)x);


        base.Awake();
    }

    MultipleAssignmentDisposable disp = new MultipleAssignmentDisposable();

    

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "TestA"))
        {
            var form = new WWWForm();
            //form.AddField("player", "hogehoge");
            //form.AddField("score", 10000);
            form.AddBinaryData("test", Encoding.UTF8.GetBytes("hogehoge"));

            var header = new Hashtable();
            header["Content-Type"] = "application/json";


            disp.Disposable = ObservableWWW.Post("http://localhost/", Encoding.UTF8.GetBytes("hogehoge"), header)
                .Select(x => x.Substring(0, 1000))
                .Subscribe(x => text.guiText.text = x);
        }
    }
}
