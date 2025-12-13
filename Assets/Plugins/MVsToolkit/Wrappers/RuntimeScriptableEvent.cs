using System;
using UnityEngine;

namespace MVsToolkit.Wrappers
{
    public class RuntimeScriptableEvent : ScriptableObject
    {
        public event Action Action;
        public void Call() => Action?.Invoke();
    }

    public class RuntimeScriptableEvent<T> : ScriptableObject
    {
        public event Action<T> Action;
        public void Call(T t) => Action?.Invoke(t);
    }

    public class RuntimeScriptableEvent<T, T1> : ScriptableObject
    {
        public event Action<T, T1> Action;
        public void Call(T t, T1 t1) => Action?.Invoke(t, t1);
    }

    public class RuntimeScriptableEvent<T, T1, T2> : ScriptableObject
    {
        public event Action<T, T1, T2> Action;
        public void Call(T t, T1 t1, T2 t2) => Action?.Invoke(t, t1, t2);
    }
}