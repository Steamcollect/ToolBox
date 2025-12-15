using MVsToolkit.Dev;
using System.Collections.Generic;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [System.Serializable]
    public class MVsPool<T> where T : Object
    {
        public T prefab;
        Queue<T> queue;

        [SerializeField] bool m_SetParent;
        public Transform parent;

        [SerializeField] bool m_LimitSize;
        public int MaximumPoolSize;

        [SerializeField] bool m_Prewarm;
        public int PrewarmCount;

        public MVsPool<T> Init(Transform parent = null)
        {
            queue = new Queue<T>();

            for (int i = 0; i < PrewarmCount; i++)
            {
                T instance = CreateInternal();
                SetActive(instance, false);
                queue.Enqueue(instance);
            }

            return this;
        }


        public T Get()
        {
            if (queue == null) Init();

            T instance;
            if (queue.Count > 0)
            {
                instance = queue.Dequeue();
            }
            else
            {
                instance = CreateInternal();
            }

            SetParent(instance, parent);
            SetActive(instance, true);
            return instance;
        }

        public void Release(T c)
        {
            if (c == null) return;

            if (queue == null) Init();

            if (m_LimitSize && MaximumPoolSize > 0 && queue.Count >= MaximumPoolSize)
            {
                DestroyInternal(c);
                return;
            }

            SetActive(c, false);
            SetParent(c, parent);
            queue.Enqueue(c);
        }

        public T Create()
        {
            if (queue == null) Init();

            T instance = CreateInternal();
            return instance;
        }

        public MVsPool<T> SetParent(Transform parent)
        {
            this.parent = parent;
            return this;
        }

        T CreateInternal()
        {
            if (prefab == null) return default;

            if (prefab is GameObject goPrefab)
            {
                GameObject go = Object.Instantiate(goPrefab, parent);
                return go as T;
            }
            else if (prefab is Component compPrefab)
            {
                GameObject go = Object.Instantiate(compPrefab.gameObject, parent);
                T comp = go.GetComponent(compPrefab.GetType()) as T;
                return comp;
            }

            var obj = Object.Instantiate(prefab);
            return obj as T;
        }

        void DestroyInternal(T c)
        {
            if (c == null) return;
            Object.Destroy(GetGameObject(c));
        }

        void SetParent(T c, Transform newParent)
        {
            var go = GetGameObject(c);
            if (go != null)
            {
                go.transform.SetParent(newParent, worldPositionStays: false);
            }
        }

        void SetActive(T c, bool active)
        {
            GameObject go = GetGameObject(c);
            if (go != null)
            {
                go.SetActive(active);
            }
        }

        GameObject GetGameObject(T c)
        {
            if (c == null) return null;
            if (c is GameObject go) return go;
            if (c is Component comp) return comp.gameObject;
            return null;
        }
    }
}