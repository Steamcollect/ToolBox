using MVsToolkit.Dev;
using System.Collections.Generic;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [System.Serializable]
    /// <summary>
    /// Generic pool for Unity objects.
    /// 
    /// <para>
    /// Use <see cref="Init"/> to initialize/prewarm, <see cref="Get"/> to obtain an active instance,
    /// </para>
    /// <para>
    /// and <see cref="Release"/> to return it to the pool. Configure <see cref="prefab"/>, <see cref="parent"/>, 
    /// </para>
    /// <para>
    /// <see cref="MaximumPoolSize"/> and <see cref="PrewarmCount"/> as needed.
    /// </para>
    public class MVsPool<T> where T : Object
    {
        /// <summary>
        /// Prefab used to instantiate pool items.
        /// Should be a <see cref="GameObject"/> or a <see cref="Component"/> to preserve hierarchy.
        /// </summary>
        public T prefab;
        Queue<T> queue;

        int objCount = 0;

        [SerializeField] bool m_SetParent;
        /// <summary>
        /// Parent transform assigned to instances when retrieved and released.
        /// </summary>
        public Transform parent;

        [SerializeField] bool m_LimitSize;
        /// <summary>
        /// Maximum pool size. If > 0, limits the total number of instances created and stored.
        /// </summary>
        public int MaximumPoolSize;

        [SerializeField] bool m_Prewarm;
        /// <summary>
        /// Number of instances created at initialization to avoid runtime allocations.
        /// </summary>
        public int PrewarmCount;

        /// <summary>
        /// Initializes the pool. Creates and deactivates <see cref="PrewarmCount"/> instances if prewarming is enabled.
        /// Call before use or it will be called automatically on first access.
        /// </summary>
        public MVsPool<T> Init()
        {
            queue = new Queue<T>();

            if(m_LimitSize && m_Prewarm && MaximumPoolSize < PrewarmCount)
                PrewarmCount = MaximumPoolSize;

            for (int i = 0; i < PrewarmCount; i++)
            {
                T instance = Create();
                SetActive(instance, false);
                queue.Enqueue(instance);
            }

            return this;
        }


        /// <summary>
        /// Retrieves an instance from the pool and activates it.
        /// Returns <c>true</c> if an instance is provided; otherwise <c>false</c> if the maximum size is reached.
        /// </summary>
        /// <param name="t">Retrieved instance.</param>
        /// <param name="parent">Optional parent for the retrieved instance, otherwise uses <see cref="MVsPool{T}.parent"/>.</param>
        public bool Get(out T t, Transform parent = null)
        {
            if (queue == null) Init();

            if (queue.Count > 0)
            {
                t = queue.Dequeue();
            }
            else if(m_LimitSize && MaximumPoolSize > 0 && objCount >= MaximumPoolSize)
            {
                t = default;
                return false;
            }
            else
            {
                t = Create();
            }

            SetParent(t, parent == null ? this.parent : parent);
            SetActive(t, true);

            return true;
        }

        /// <summary>
        /// Returns an instance to the pool. The instance is deactivated and reassigned under <see cref="parent"/>.
        /// If the limit is reached, the instance is destroyed.
        /// </summary>
        /// <param name="c">Instance to return.</param>
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

        /// <summary>
        /// Creates a new instance from the <see cref="prefab"/> and increments the internal counter.
        /// Automatically called if the pool is empty.
        /// </summary>
        public T Create()
        {
            if (queue == null) Init();

            T instance = CreateInternal();
            objCount++;
            return instance;
        }

        /// <summary>
        /// Sets the default parent used during pool operations.
        /// </summary>
        /// <param name="parent">Parent transform to use.</param>
        /// <returns>Pool reference for chaining.</returns>
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
            return obj;
        }

        void DestroyInternal(T c)
        {
            if (c == null) return;
            Object.Destroy(GetGameObject(c));
        }

        void SetParent(T c, Transform newParent)
        {
            GameObject go = GetGameObject(c);
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