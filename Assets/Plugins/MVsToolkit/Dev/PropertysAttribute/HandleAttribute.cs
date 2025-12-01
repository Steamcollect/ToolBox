using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Draws a scene handle for a Vector3 field in the Scene view.
    /// <para>
    /// Can be displayed in Global or Local coordinates.
    /// </para>
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[Handle(TransformLocationType.Local)]</code> shows handle in local space</description></item>
    /// </list>
    /// </summary>
    public class HandleAttribute : PropertyAttribute
    {
        public readonly TransformLocationType HandleType;

        /// <param name="handleType">Coordinate space for the handle (Global or Local).</param>
        public HandleAttribute(TransformLocationType handleType = TransformLocationType.Global)
        {
            HandleType = handleType;
        }
    }

    public enum TransformLocationType
    {
        Global,
        Local,
    }
}