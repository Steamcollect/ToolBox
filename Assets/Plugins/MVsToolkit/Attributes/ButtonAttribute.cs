using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Creates a button below the method in the Inspector.
    /// <para>
    /// Can only be used once per method.
    /// </para>
    /// <para>Examples:</para>
    /// <list type="bullet">
    ///   <item><description><code>[Button]</code> below a method with no parameters</description></item>
    ///   <item><description><code>[Button(10, "PlayerName")]</code> passes values to the method</description></item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public readonly object[] Parameters;

        /// <param name="parameters">Values or field names to pass as arguments when the button is clicked.</param>
        public ButtonAttribute(params object[] parameters)
        {
            Parameters = parameters;
        }
    }
}