using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Hides the field in the Inspector if the specified condition is met.
    /// <para>
    /// Works with bools and enums.
    /// </para>
    /// <para>
    /// Can be used multiple times on the same field.
    /// </para>
    /// <para>Examples:</para>
    /// <list type="bullet">
    ///   <item><description><code>[HideIf("isEnabled", true)]</code> hides if bool is true</description></item>
    ///   <item><description><code>[HideIf("mode", GameMode.Debug)]</code> hides if enum matches</description></item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string ConditionField;
        public readonly object CompareValue;

        /// <param name="conditionField">Name of the field to evaluate.</param>
        /// <param name="compareValue">Value that triggers hiding.</param>
        public HideIfAttribute(string conditionField, object compareValue)
        {
            ConditionField = conditionField;
            CompareValue = compareValue;
        }
    }
}