// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UniRx.UI;

namespace UniRx.Examples
{
    public class Sample12_ReactiveProperty : MonoBehaviour
    {
        // Open Sample12Scene. Set from canvas
        public Button MyButton;
        public Toggle MyToggle;
        public InputField MyInput;
        public Text MyText;
        public Slider MySlider;

        // You can monitor Inspectorable
        [InspectorDisplay]
        public IntReactiveProperty IntRxProp = new IntReactiveProperty();

        Enemy enemy = new Enemy(1000);

        void Start()
        {
            // UnityEvent as Observable
            MyButton.onClick.AsObservable().Subscribe(_ => enemy.CurrentHp.Value -= 99);

            // Toggle, Input etc as Observable
            MyToggle.isOn = false;
            MyToggle.onValueChanged.AsObservable()
                .StartWith(MyToggle.isOn) // Initial Value with StartWith
                .Subscribe(x => MyButton.interactable = x);

            // input shows delay after 1 second
            MyInput.onValueChange.AsObservable()
                .Delay(TimeSpan.FromSeconds(1))
                .Subscribe(x => MyText.text = x);

            // converting for human visibility
            MySlider.onValueChanged.AsObservable()
                .Select(x => Math.Round(x, 2).ToString())
                .Subscribe(x => MyText.text = x);

            // from RxProp, CurrentHp changing(Button Click) is observable
            enemy.CurrentHp.Subscribe(x => MyText.text = x.ToString());
        }
    }

    // Reactive Notification Model
    public class Enemy
    {
        public ReactiveProperty<long> CurrentHp { get; private set; }

        public ReactiveProperty<bool> IsDead { get; private set; }


        public Enemy(int initialHp)
        {
            // Declarative Property
            CurrentHp = new ReactiveProperty<long>(initialHp);
            IsDead = CurrentHp.Select(x => x <= 0).ToReactiveProperty();
        }
    }
}

#endif