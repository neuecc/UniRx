using System;
using System.Collections;
using UnityEngine;

namespace UniRx.ObjectTest
{
    public class GameObjectDisplacer : MonoBehaviour
    {
        public GameObject originalObject;
        public Vector3 displacement;

        private int Counter = 0;
        private static GameObjectDisplacer _instance;

        void Awake()
        {
            originalObject.AddComponent<AwakeDetector>();

            if (_instance == null)
            {
                _instance = this;

                AwakeDetector.OnAwake += (g) => GameObjectCloned(g);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void GameObjectCloned(GameObject g)
        {
            if (originalObject != null)
            {
                g.name = string.Format("{0} #{1}", originalObject.name, Counter++);
                g.transform.position += displacement * Counter;
            }
        }
    }

    public class AwakeDetector : MonoBehaviour
    {
        public static event Action<GameObject> OnAwake = delegate { };

        void Awake()
        {
            OnAwake(this.gameObject);
        }
    }
}