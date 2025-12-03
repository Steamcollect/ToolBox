using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Draws a handle in the Scene view for a Vector3 or Vector2 field.
    /// <para>
    /// You can configure:
    /// - The coordinate space (Local or Global)
    /// - The handle shape (Default, Sphere, Cube)
    /// - The color (via <see cref="ColorPreset"/>)
    /// - The size
    /// </para>
    /// <para>Examples:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <code>[Handle]</code> → Default handle (Local, Default, White, size 0.2)
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[Handle(TransformLocationType.Global)]</code> → Handle in global coordinates
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[Handle(HandleDrawType.Sphere)]</code> → Handle drawn as a sphere
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[Handle(TransformLocationType.Local, HandleDrawType.Cube, ColorPreset.Red, 1f)]</code>
    ///       → Local handle, cube shape, red color, size 1
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class HandleAttribute : PropertyAttribute
    {
        public readonly Space spaceType;
        public readonly HandleDrawType DrawType;
        public readonly Color Color;
        public readonly float Size;

        /// <param name="spaceType">Coordinate space (Self or World). Default is Self.</param>
        /// <param name="drawType">Handle shape (Default, Sphere, Cube). Default is Default.</param>
        /// <param name="preset">Handle color (see <see cref="ColorPreset"/>). Default is White.</param>
        /// <param name="size">Handle size. Default is 0.2.</param>
        public HandleAttribute(
            Space spaceType = Space.Self,
            HandleDrawType drawType = HandleDrawType.Default,
            ColorPreset preset = ColorPreset.White,
            float size = .2f)
        {
            this.spaceType = spaceType;
            DrawType = drawType;
            Color = PresetToColor(preset);
            Size = size;
        }

        private Color PresetToColor(ColorPreset preset)
        {
            return preset switch
            {
                ColorPreset.Red => Color.red,
                ColorPreset.Green => Color.green,
                ColorPreset.Blue => Color.blue,
                ColorPreset.Yellow => Color.yellow,
                ColorPreset.Cyan => Color.cyan,
                ColorPreset.Magenta => Color.magenta,
                _ => Color.white
            };
        }
    }

    public enum HandleDrawType
    {
        Default,
        Sphere,
        Cube
    }

    public enum ColorPreset
    {
        White,
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta
    }
}