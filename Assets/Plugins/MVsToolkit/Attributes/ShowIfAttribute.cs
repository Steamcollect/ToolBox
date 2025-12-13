using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Shows the field in the Inspector only if the specified condition is met.
    /// <para>
    /// Works with bools and enums.
    /// </para>
    /// <para>
    /// Can be used multiple times on the same field.
    /// </para>
    /// <para>Examples:</para>
    /// <list type="bullet">
    ///   <item><description><code>[ShowIf("isEnabled", true)]</code> shows if bool is true</description></item>
    ///   <item><description><code>[ShowIf("mode", GameMode.Debug)]</code> shows if enum matches</description></item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string ConditionField;
        public readonly object CompareValue;

        /// <param name="conditionField">Name of the field to evaluate.</param>
        /// <param name="compareValue">Value that triggers visibility.</param>
        public ShowIfAttribute(string conditionField, object compareValue)
        {
            ConditionField = conditionField;
            CompareValue = compareValue;
        }
    }
}