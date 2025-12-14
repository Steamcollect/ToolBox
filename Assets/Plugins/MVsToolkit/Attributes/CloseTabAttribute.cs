using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Closes the current tab section in the Inspector.
    /// <para>
    /// Use this to explicitly end a tab group started with <see cref="TabAttribute"/>.
    /// </para>
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[CloseTab]</code> ends the current tab section</description></item>
    /// </list>
    /// </summary>
    public class CloseTabAttribute : PropertyAttribute { }
}