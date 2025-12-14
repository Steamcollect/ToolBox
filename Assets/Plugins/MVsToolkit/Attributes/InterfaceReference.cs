using System;
using System.Linq;
using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Serializable wrapper that allows referencing Unity objects by interface.
    /// 
    /// <para>
    /// Useful when you want to expose an interface in the Inspector while still assigning MonoBehaviours.
    /// </para>
    /// 
    /// <para>
    /// Only works with UnityEngine.Object types (e.g., MonoBehaviour) that implement the interface.
    /// </para>
    /// 
    /// <para>
    /// To access the interface, use the <code>.Value</code> property:
    /// </para>
    /// 
    /// <list type="bullet">
    ///   <item><description><code>interactable.Value.Interact();</code></description></item>
    /// </list>
    /// 
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>public interface IInteractable { void Interact(); }</code></description></item>
    ///   <item><description><code>public class Door : MonoBehaviour, IInteractable { public void Interact() { ... } }</code></description></item>
    ///   <item><description><code>public InterfaceReference&lt;IInteractable&gt; interactable;</code></description></item>
    ///   <item><description><code>interactable.Value.Interact();</code> Calls the method on the assigned object</description></item>
    /// </list>
    /// 
    /// <para>
    /// Supports returning <see cref="System.Collections.Generic.IEnumerable{T}"/> when the stored object is a <see cref="GameObject"/> containing components implementing the element interface.
    /// Falls back to direct cast otherwise.
    /// </para>
    /// </summary>
    [System.Serializable]
    public class InterfaceReference<T> where T : class
    {
        [SerializeField] private UnityEngine.Object _object;

        /// <summary>
        /// Returns the referenced object cast as the interface type.
        /// Use this to access interface methods or properties.
        /// </summary>
        public T Value
        {
            get
            {
                // Direct cast if possible
                if (_object is T direct)
                    return direct;

                // Support for IEnumerable<TElem> where stored object is a GameObject
                var tType = typeof(T);
                if (_object is GameObject go && tType.IsGenericType && tType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>))
                {
                    var elemType = tType.GetGenericArguments()[0];

                    // Get all components and filter by element type
                    var components = go.GetComponents<Component>();
                    var query = components.Where(c => elemType.IsAssignableFrom(c.GetType()));

                    // Use Enumerable.Cast(elemType) via reflection and box to object, then cast to T
                    var castMethod = typeof(Enumerable).GetMethods()
                        .First(m => m.Name == nameof(Enumerable.Cast) && m.GetParameters().Length == 1)
                        .MakeGenericMethod(elemType);

                    var casted = castMethod.Invoke(null, new object[] { query });
                    return (T)casted;
                }

                return null;
            }
        }

        public static implicit operator T(InterfaceReference<T> reference)
        {
            return reference.Value;
        }
    }
}