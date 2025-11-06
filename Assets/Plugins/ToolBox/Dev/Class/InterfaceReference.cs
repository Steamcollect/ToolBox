using UnityEngine;

namespace ToolBox.Dev
{
    [System.Serializable]
    public class InterfaceReference<T> where T : class
    {
        [SerializeField] private Object _object;

        public T Value => _object as T;
    }
}