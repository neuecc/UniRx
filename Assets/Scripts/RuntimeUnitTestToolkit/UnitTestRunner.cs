using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

namespace RuntimeUnitTestToolkit
{
    public static class UnitTest
    {
        public static void AddTest(Action test)
        {
            AddTest(test.Target.GetType().Name, test.Method.Name, test);
        }

        public static void AddTest(string group, string title, Action test)
        {
            UnitTestRunner.AddTest(group, title, test);
        }

        public static void AddAsyncTest(Func<IEnumerator> asyncTestCoroutine)
        {
            AddAsyncTest(asyncTestCoroutine.Target.GetType().Name, asyncTestCoroutine.Method.Name, asyncTestCoroutine);
        }

        public static void AddAsyncTest(string group, string title, Func<IEnumerator> asyncTestCoroutine)
        {
            UnitTestRunner.AddAsyncTest(group, title, asyncTestCoroutine);
        }

        public static void AddCustomButton(Button button)
        {
            UnitTestRunner.AddCustomButton(button);
        }

        public static void RegisterAllMethods<T>()
            where T : new()
        {
            try
            {
            // test, only new
            var test = new T();
            /*
            var methods = typeof(T).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var item in methods)
            {
                if (item.GetParameters().Length != 0) continue;

                if (item.ReturnType == typeof(IEnumerator))
                {
                    var factory = (Func<IEnumerator>)Delegate.CreateDelegate(typeof(Func<IEnumerator>), test, item);
                    AddAsyncTest(factory);
                }
                else if (item.ReturnType == typeof(void))
                {
                    var invoke = (Action)Delegate.CreateDelegate(typeof(Action), test, item);
                    AddTest(invoke);
                }
            }
            */
            }
            catch
            {
                // LogException...
            }
        }
    }

    public class UnitTestRunner : MonoBehaviour
    {
        // object is IEnumerator or Func<IEnumerator>
        static Dictionary<string, List<KeyValuePair<string, object>>> tests = new Dictionary<string, List<KeyValuePair<string, object>>>();

        static List<Button> additionalButtonsOnFirst = new List<Button>();


        public Button clearButton;
        public RectTransform list;
        public Scrollbar listScrollBar;

        public Text logText;
        public Scrollbar logScrollBar;

        readonly Color passColor = new Color(0f, 1f, 0f, 1f); // green
        readonly Color failColor = new Color(1f, 0f, 0f, 1f); // red
        readonly Color normalColor = new Color(1f, 1f, 1f, 1f); // white

        void Start()
        {
            // register unexpected log
            Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
            {
                if (type == LogType.Error || type == LogType.Exception)
                {
                    logText.text += "<color=red>" + condition + "\n" + stackTrace + "</color>\n";
                }
                else
                {
                    logText.text += condition + "\n";
                }
            };

            var executeAll = new List<Func<Coroutine>>();
            foreach (var ___item in tests)
            {
                var actionList = ___item; // be careful, capture in lambda

                executeAll.Add(() => StartCoroutine(RunTestInCoroutine(actionList)));
                Add(actionList.Key, () => StartCoroutine(RunTestInCoroutine(actionList)));
            }

            var executeAllButton = Add("Run All Tests", () => StartCoroutine(ExecuteAllInCoroutine(executeAll)));

            clearButton.gameObject.GetComponent<Image>().color = new Color(170 / 255f, 170 / 255f, 170 / 255f, 1);
            executeAllButton.gameObject.GetComponent<Image>().color = new Color(250 / 255f, 150 / 255f, 150 / 255f, 1);
            executeAllButton.transform.SetSiblingIndex(1);

            additionalButtonsOnFirst.Reverse();
            foreach (var item in additionalButtonsOnFirst)
            {
                item.transform.SetParent(list);
                item.transform.SetSiblingIndex(1);
            }

            clearButton.onClick.AddListener(() =>
            {
                logText.text = "";
                foreach (var btn in list.GetComponentsInChildren<Button>())
                {
                    btn.interactable = true;
                    btn.GetComponent<Image>().color = normalColor;
                }
                executeAllButton.gameObject.GetComponent<Image>().color = new Color(250 / 255f, 150 / 255f, 150 / 255f, 1);
            });

            listScrollBar.value = 1;
            logScrollBar.value = 1;
        }

        Button Add(string title, UnityAction test)
        {
            var newButton = GameObject.Instantiate(clearButton);
            newButton.name = title;
            newButton.onClick.RemoveAllListeners();
            newButton.GetComponentInChildren<Text>().text = title;
            newButton.onClick.AddListener(test);

            newButton.transform.SetParent(list);
            return newButton;
        }

        public static void AddTest(string group, string title, Action test)
        {
            List<KeyValuePair<string, object>> list;
            if (!tests.TryGetValue(group, out list))
            {
                list = new List<KeyValuePair<string, object>>();
                tests[group] = list;
            }

            list.Add(new KeyValuePair<string, object>(title, test));
        }

        public static void AddAsyncTest(string group, string title, Func<IEnumerator> asyncTestCoroutine)
        {
            List<KeyValuePair<string, object>> list;
            if (!tests.TryGetValue(group, out list))
            {
                list = new List<KeyValuePair<string, object>>();
                tests[group] = list;
            }

            list.Add(new KeyValuePair<string, object>(title, asyncTestCoroutine));
        }

        public static void AddCustomButton(Button button)
        {
            additionalButtonsOnFirst.Add(button);
        }

        System.Collections.IEnumerator ScrollLogToEndNextFrame()
        {
            yield return null;
            yield return null;
            logScrollBar.value = 0;
        }

        IEnumerator RunTestInCoroutine(KeyValuePair<string, List<KeyValuePair<string, object>>> actionList)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Button self = null;
            foreach (var btn in list.GetComponentsInChildren<Button>())
            {
                btn.interactable = false;
                if (btn.name == actionList.Key) self = btn;
            }
            if (self != null)
            {
                self.GetComponent<Image>().color = normalColor;
            }

            var allGreen = true;

            logText.text += "<color=yellow>" + actionList.Key + "</color>\n";
            yield return null;

            foreach (var item2 in actionList.Value)
            {
                logText.text += "<color=teal>" + item2.Key + "</color>\n";
                yield return null;

                var v = item2.Value;

                var methodStopwatch = System.Diagnostics.Stopwatch.StartNew();
                Exception exception = null;
                if (v is Action)
                {
                    try
                    {
                        ((Action)v).Invoke();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }
                else
                {
                    var coroutineFactory = (Func<IEnumerator>)v;
                    yield return StartCoroutine(UnwrapEnumerator(coroutineFactory(), ex =>
                    {
                        exception = ex;
                    }));
                }

                if (exception == null)
                {
                    methodStopwatch.Stop();
                    logText.text += "OK, " + methodStopwatch.Elapsed.TotalMilliseconds.ToString("0.00") + "ms\n";
                }
                else
                {
                    // found match line...
                    var line = string.Join("\n", exception.StackTrace.Split('\n').Where(x => x.Contains(actionList.Key) || x.Contains(item2.Key)).ToArray());
                    logText.text += "<color=red>" + exception.Message + "\n" + line + "</color>\n";
                    allGreen = false;
                }
            }

            sw.Stop();
            logText.text += "[" + actionList.Key + "]" + sw.Elapsed.TotalMilliseconds.ToString("0.00") + "ms\n\n";
            foreach (var btn in list.GetComponentsInChildren<Button>()) btn.interactable = true;
            if (self != null)
            {
                self.GetComponent<Image>().color = allGreen ? passColor : failColor;
            }

            yield return StartCoroutine(ScrollLogToEndNextFrame());
        }

        IEnumerator ExecuteAllInCoroutine(List<Func<Coroutine>> tests)
        {
            foreach (var item in tests)
            {
                yield return item();
            }
        }

        IEnumerator UnwrapEnumerator(IEnumerator enumerator, Action<Exception> exceptionCallback)
        {
            var hasNext = true;
            while (hasNext)
            {
                try
                {
                    hasNext = enumerator.MoveNext();
                }
                catch (Exception ex)
                {
                    exceptionCallback(ex);
                    hasNext = false;
                }

                if (hasNext)
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
}
