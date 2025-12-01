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
    /// </summary>
    [System.Serializable]
    public class InterfaceReference<T> where T : class
    {
        [SerializeField] private Object _object;

        /// <summary>
        /// Returns the referenced object cast as the interface type.
        /// Use this to access interface methods or properties.
        /// </summary>
        public T Value => _object as T;

        public static implicit operator T(InterfaceReference<T> reference)
        {
            return reference.Value;
        }
    }
}