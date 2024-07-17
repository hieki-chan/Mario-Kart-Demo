using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KartDemo
{
    //[System.Serializable]
    public class SimplePool<T> where T : Component
    {
        public Queue<T> pool = new Queue<T>();

        public void Create(T component, int size)
        {
            for (int i = 0; i < size; i++)
            {
                T go = Object.Instantiate(component);
                go.gameObject.SetActive(false);
                pool.Enqueue(go);
            }
        }

        public T Get(bool activeObject = true)
        {
            T cpn = pool.Dequeue();
            cpn.gameObject.SetActive(activeObject);
            return cpn;
        }

        public T Get(Vector3 position, Quaternion rotation, bool activeObject = true)
        {
            T cpn = pool.Dequeue();
            cpn.transform.SetPositionAndRotation(position, rotation);
            cpn.gameObject.SetActive(activeObject);
            return cpn;
        }

        public T Get(Vector3 position, Quaternion rotation, Action<T> OnGet, bool activeObject = true)
        {
            T cpn = pool.Dequeue();
            OnGet?.Invoke(cpn);
            cpn.transform.SetPositionAndRotation(position, rotation);
            cpn.gameObject.SetActive(activeObject);
            return cpn;
        }

        public T GetOrCreate(T component, Vector3 position, Quaternion rotation, bool activeObject = true, int createCount = 1)
        {
            if (pool.Count == 0)
                Create(component, createCount);

            return Get(position, rotation, activeObject);
        }

        public T GetOrCreate(T component, Vector3 position, Quaternion rotation, Action<T> OnGet, bool activeObject = true, int createCount = 1)
        {
            if (pool.Count == 0)
                Create(component, createCount);

            return Get(position, rotation, OnGet, activeObject);
        }

        public void AddExisting(T component)
        {
            pool.Enqueue(component);
        }

        public void Return(T component, bool active = false)
        {
            pool.Enqueue(component);
            component.gameObject.SetActive(active);
        }
    }

    public class Pool<TKey, TValue> where TValue : Component
    {
        Dictionary<TKey, SimplePool<TValue>> m_Pools = new Dictionary<TKey, SimplePool<TValue>>();
        public int Capacity = 1;

        public Pool() { }

        public Pool(int Capacity)
        {
            this.Capacity = Capacity;
        }

        public TValue Get(TKey key, Vector3 position, Quaternion rotation)
        {
            if (!m_Pools.TryGetValue(key, out SimplePool<TValue> pool))
            {
                return null;
            }

            return pool.Get(position, rotation);
        }

        public TValue GetOrCreate(TKey key, TValue component, Vector3 position, Quaternion rotation, bool activeObject = true)
        {
            if (!m_Pools.TryGetValue(key, out SimplePool<TValue> pool))
            {
                pool = new SimplePool<TValue>();
                m_Pools.Add(key, pool);
            }

            return pool.GetOrCreate(component, position, rotation, activeObject, Capacity);
        }

        public T GetOrCreate<T>(TKey key, TValue component, Vector3 position, Quaternion rotation, bool activeObject = true) where T : TValue
        {
            if (!m_Pools.TryGetValue(key, out SimplePool<TValue> pool))
            {
                pool = new SimplePool<TValue>();
                m_Pools.Add(key, pool);
            }

            return pool.GetOrCreate(component, position, rotation, activeObject, Capacity) as T;
        }

        public TValue GetOrCreate(TKey key, TValue component, Vector3 position, Quaternion rotation, Action<TValue> OnGet, bool activeObject = true)
        {
            if (!m_Pools.TryGetValue(key, out SimplePool<TValue> pool))
            {
                pool = new SimplePool<TValue>();
                m_Pools.Add(key, pool);
            }

            return pool.GetOrCreate(component, position, rotation, OnGet, activeObject, Capacity);
        }

        public void Return(TKey key, TValue component)
        {
            if (!m_Pools.TryGetValue(key, out SimplePool<TValue> pool))
            {
                pool = new SimplePool<TValue>();
                m_Pools.Add(key, pool);
            }

            pool.Return(component);
        }
    }
}
/*
[Serializable]
public class PoolObject<TKey, TValue>
{
   public TKey key;
   public TValue component;
}*/