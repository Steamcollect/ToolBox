using System;
using UnityEngine;

namespace BT.ScriptablesObject
{
    public class RuntimeScriptableObject<T> : ScriptableObject
    {
        private T _value;
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
}
