using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class DispatcherCullingTest : MonoBehaviour
    {
        public GUIText cullLabel;
        private string cullingModeDescription = string.Empty;

        void Start()
        {
            var cullClicker = cullLabel.gameObject.AddComponent<Clicker>();
            var cullClickerColor = cullClicker.GetComponent<GUIText>().color;
            cullClicker.OnEntered += () => cullLabel.GetComponent<GUIText>().color = Color.blue;
            cullClicker.OnExited += () => cullLabel.GetComponent<GUIText>().color = cullClickerColor;
            cullClicker.OnClicked += () =>
            {
                var values = Enum.GetValues(typeof(MainThreadDispatcher.CullingMode));
                var currentValue = (int)MainThreadDispatcher.cullingMode;
                MainThreadDispatcher.cullingMode = (MainThreadDispatcher.CullingMode)((currentValue + 1 == values.Length) ? 0 : currentValue + 1);
            };
        }

        void Update()
        {
            switch (MainThreadDispatcher.cullingMode)
            {
                case MainThreadDispatcher.CullingMode.Disabled:
                    cullingModeDescription = "Won't remove any MainThreadDispatchers.";
                    break;

                case MainThreadDispatcher.CullingMode.Self:
                    cullingModeDescription = "A new MainThreadDispatcher will remove itself when there's an existing dispatcher.";
                    break;

                case MainThreadDispatcher.CullingMode.All:
                    cullingModeDescription = "When a new MainThreadDispatcher is added, search and destroy any excess dispatchers.";
                    break;
            }
            cullLabel.text = string.Format("Click to toggle Dispatcher Culling Mode: <b>{0}</b>\n{1}"
                , MainThreadDispatcher.cullingMode.ToString(), cullingModeDescription);
        }
    }
}
