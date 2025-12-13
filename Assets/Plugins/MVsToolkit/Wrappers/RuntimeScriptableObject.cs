using System;
using UnityEngine;

namespace MVsToolkit.Wrappers
{
    public class RuntimeScriptableObject<T> : ScriptableObject
    {
        private T value = default(T);

        public event Action<T> OnChanged;

        public T Get()
        {
            return value;
        }
        public void Set(T value)
        {
            this.value = value;
            OnChanged?.Invoke(value);
        }


        public static implicit operator T(RuntimeScriptableObject<T> runtimeScriptableObject)
        {
            return runtimeScriptableObject.value;
        }
    }
}