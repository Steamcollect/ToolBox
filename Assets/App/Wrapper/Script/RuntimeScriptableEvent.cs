using System;
using UnityEngine;

namespace BT.ScriptablesObject
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

    public class RuntimeScriptableEvent<T,T1> : ScriptableObject
    {
        public event Action<T,T1> Action;
        public void Call(T t, T1 t1) => Action?.Invoke(t,t1);
    }
}