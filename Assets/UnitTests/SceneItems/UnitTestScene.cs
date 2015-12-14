using UnityEngine;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine.UI;
using System;

namespace UniRx.Tests
{
    public class UnitTestScene : MonoBehaviour
    {
        public Button buttonPrefab;
        public GameObject buttonVertical;

        public Result resultPrefab;
        public GameObject resultVertical;

        public Text text;

        public Button sample;

        int counter;


        void Start()
        {
            try
            {
                text.text = "sample...";

                //sample.OnClickAsObservable().Subscribe(_ =>
                //{
                //    text.text = "start...";
                //    MainThreadDispatcher.StartCoroutine(ObservableConcurrencyTest.Run(resultPrefab, resultVertical));
                //});

                //// UnitTest uses Wait, it can't run on MainThreadScheduler.
                //Scheduler.DefaultSchedulers.SetDotNetCompatible();
                //MainThreadDispatcher.Initialize();

                //UnitTests.SetButtons(buttonPrefab, buttonVertical, resultPrefab, resultVertical);
            }
            catch (Exception ex)
            {
                text.text = ex.ToString();
            }
        }

        public void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 100), (counter++).ToString()))
            {

            }
        }

        public void Update()
        {
            text.text = (counter++).ToString();
        }
    }
}