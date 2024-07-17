using KartDemo.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KartDemo
{
    public class Activator : MonoBehaviour
    {
        static List<IActiveHandler> m_InactiveObject = new List<IActiveHandler>();
        static bool ready;

        private void Awake()
        {
            if (ready)
            {
                Debug.LogWarning("There's more than 1 Activator", this);
            }
            ready = true;
        }

        private void Update()
        {
            for (int i = 0; i < m_InactiveObject.Count; i++)
            {
                IActiveHandler inactiveObj = m_InactiveObject[i];
                inactiveObj.timer += Time.deltaTime;

                if (!inactiveObj.IsActivating())
                {
                    inactiveObj.OnActive();
                    m_InactiveObject.RemoveAtSwapBack(i);
                    i--;
                }
            }
        }

        public static void Active(IActiveHandler activeObject)
        {
            if (!ready)
            {
                GameObject activator = new GameObject("Activator Manager");
                activator.AddComponent<Activator>();

            }
            if (m_InactiveObject.Contains(activeObject))
            {
                return;
            }

            activeObject.OnInactive();
            activeObject.timer = 0;
            m_InactiveObject.Add(activeObject);
        }

        public static void Enable(GameObject gameObject, float time, Action<GameObject> OnEnable = null)
        {
            Active(new Enableable()
            {
                gameObject = gameObject,
                reactivationTime = time,
                OnEnable = OnEnable
            });
        }

        public static void Disable(GameObject gameObject, float time, Action<GameObject> OnDisable = null)
        {
            Active(new Disableable()
            {
                gameObject = gameObject,
                reactivationTime = time,
                OnDisable = OnDisable
            });
        }

        class Enableable : IActiveHandler
        {
            public float reactivationTime { get; set; }

            public float timer { get; set; }

            public GameObject gameObject;

            public Action<GameObject> OnEnable;

            public void OnActive()
            {
                gameObject.SetActive(true);
                OnEnable?.Invoke(gameObject);
            }

            public void OnInactive()
            {
                //gameObject.SetActive(false);
            }
        }

        class Disableable : IActiveHandler
        {
            public float reactivationTime { get; set; }

            public float timer { get; set; }

            public GameObject gameObject;

            public Action<GameObject> OnDisable;

            public void OnActive()
            {
                gameObject.SetActive(false);
                OnDisable?.Invoke(gameObject);
            }

            public void OnInactive()
            {
                //gameObject.SetActive(false);
            }
        }
    }

    public interface IActiveHandler
    {
        float reactivationTime { get; }

        float timer { get; set; }

        void OnInactive();

        void OnActive();
    }

    public static class ActivaHandlerExtends
    {
        public static bool IsActivating(this IActiveHandler activeObject)
        {
            return activeObject.timer < activeObject.reactivationTime;
        }
    }

    class WaitActivator : IActiveHandler
    {
        public float reactivationTime => seconds;

        public float timer { get; set; } = 0;

        public void OnActive()
        {
            OnEnd?.Invoke();
        }

        public void OnInactive()
        {
            OnStart?.Invoke();
        }

        float seconds;
        UnityAction OnStart;
        UnityAction OnEnd;

        public WaitActivator(float seconds, UnityAction OnStart, UnityAction OnEnd)
        {
            this.seconds = seconds;
            this.OnStart = OnStart;
            this.OnEnd = OnEnd;
            timer = seconds;
        }
    }
}
