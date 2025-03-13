using System;
using UnityEngine;

public class RuntimeScriptableObject<T> : ScriptableObject
{
    private T _value = default(T);
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            OnChanged?.Invoke(value);
        }
    }

    public event Action<T> OnChanged;
}
