using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Displays a min-max range slider in the Inspector for Vector2 or Vector2Int fields.
    /// <para>
    /// The x component represents the minimum value, and the y component represents the maximum value.
    /// </para>
    /// <para>
    /// Supports both integer and float ranges.
    /// </para>
    /// <para>Examples:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <code>[MinMaxRange(0, 100)]</code> → Integer range from 0 to 100
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[MinMaxRange(0f, 1f)]</code> → Float range from 0.0 to 1.0
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public Vector2 damageRange;</code> → Use with Vector2 for floats
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public Vector2Int ageRange;</code> → Use with Vector2Int for integers
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float FMin, FMax;
        public readonly int IMin, IMax;

        /// <param name="min">Minimum value of the range (integer).</param>
        /// <param name="max">Maximum value of the range (integer).</param>
        public MinMaxRangeAttribute(int min, int max)
        {
            IMin = min;
            IMax = max;

            FMin = min;
            FMax = max;
        }

        /// <param name="min">Minimum value of the range (float).</param>
        /// <param name="max">Maximum value of the range (float).</param>
        public MinMaxRangeAttribute(float min, float max)
        {
            FMin = min;
            FMax = max;

            IMin = Mathf.RoundToInt(min);
            IMax = Mathf.RoundToInt(max);
        }
    }
}