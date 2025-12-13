using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Draws a struct or class inline in the Inspector without a foldout.
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[Inline]</code> displays nested fields compactly</description></item>
    /// </list>
    /// </summary>
    public class InlineAttribute : PropertyAttribute { }
}