using System;
using UnityEngine;

namespace BT.ScriptablesObject
{
    public class RuntimeScriptableEvent : ScriptableObject
    {
        public event Action action;
        public void Call() => action?.Invoke();
    }

    public class RuntimeScriptableEvent<T> : ScriptableObject
    {
        public event Action<T> action;
        public void Call(T t) => action?.Invoke(t);
    }

    public class RuntimeScriptableEvent<T,T1> : ScriptableObject
    {
        public event Action<T,T1> action;
        public void Call(T t, T1 t1) => action?.Invoke(t,t1);
    }
}