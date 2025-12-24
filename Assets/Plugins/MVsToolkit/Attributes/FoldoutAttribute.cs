using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Groups fields under a collapsible foldout section in the Inspector.
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[Foldout("Player Settings")]</code> groups related fields</description></item>
    /// </list>
    /// 
    /// Close the foldout with [CloseFoldout] attribute.
    /// </summary>
    public class FoldoutAttribute : PropertyAttribute
    {
        public string foldoutName;

        /// <param name="foldoutName">Label for the foldout section.</param>
        public FoldoutAttribute(string foldoutName)
        {
            this.foldoutName = foldoutName;
        }
    }
}
