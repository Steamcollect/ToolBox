using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Makes the field read-only (grayed out) in the Inspector.
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[ReadOnly]</code> disables editing in Inspector</description></item>
    /// </list>
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }
}