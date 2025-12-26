using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Draws the field using a custom GUI method within a specified rectangular area.
    /// <para>
    /// The custom method must have the signature: <code>void MethodName(Rect rect, SerializedProperty property)</code>
    /// </para>
    /// <para>
    /// This allows complete control over how the field is rendered in the Inspector.
    /// </para>
    /// <para>Examples:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <code>[DrawInRect("DrawCustomField")]</code> → Calls DrawCustomField method with default height of 30px
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[DrawInRect("DrawCustomField", 50f)]</code> → Uses custom height of 50px
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    public class DrawInRectAttribute : PropertyAttribute
    {
        public string methodName;
        public float height;

        /// <param name="methodName">Name of the custom drawing method to call.</param>
        /// <param name="height">Height of the rect area in pixels. Default is 30.</param>
        public DrawInRectAttribute(string methodName, float height = 30f)
        {
            this.methodName = methodName;
            this.height = height;
        }
    }
}