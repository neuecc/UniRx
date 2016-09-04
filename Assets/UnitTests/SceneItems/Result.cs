#if !UNITY_METRO && !UNITY_4_5

using UnityEngine;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine.UI;
using System;

namespace UniRx.Tests
{
    public class Result : MonoBehaviour
    {
        public UnityEngine.UI.Text text;

        public ReactiveProperty<string> Message { get; private set; }
        public ReactiveProperty<Color> Color { get; private set; }

        void Start()
        {
            var image = this.GetComponent<Image>();

            Message = new ReactiveProperty<string>("");
            Message.SubscribeToText(text);

            Color = new ReactiveProperty<UnityEngine.Color>();
            Color.Subscribe(x => image.color = x);
        }
    }
}

#endif