using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Closes the current foldout section in the Inspector.
    /// <para>
    /// Use this to explicitly end a foldout group started with <see cref="FoldoutAttribute"/>.
    /// </para>
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[CloseFoldout]</code> ends the current foldout section</description></item>
    /// </list>
    /// </summary>
    public class CloseFoldoutAttribute : PropertyAttribute { }
}