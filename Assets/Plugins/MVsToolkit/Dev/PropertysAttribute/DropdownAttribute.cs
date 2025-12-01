using UnityEngine;
using UnityEngine.UI;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Displays a dropdown in the Inspector with given values.
    /// <para>
    /// Values can be hardcoded or referenced from another field.
    /// </para>
    /// <para>
    /// Supported types: string, float, int
    /// </para>
    /// <para>Examples:</para>
    /// <list type="bullet">
    ///   <item><description><code>[Dropdown("AvailableWeapons")]</code> uses a reference field</description></item>
    ///   <item><description><code>[Dropdown("Easy", "Medium", "Hard")]</code> uses hardcoded values</description></item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DropdownAttribute : PropertyAttribute
    {
        public readonly bool isReference;
        public readonly string Path;
        public readonly object[] objects;

        /// <param name="path">Name of the field or property to reference for dropdown values.</param>
        public DropdownAttribute(string path)
        {
            isReference = true;
            Path = path;
        }

        /// <param name="objects">Hardcoded values to display in the dropdown.</param>
        public DropdownAttribute(params object[] objects)
        {
            isReference = false;
            this.objects = objects;
        }
    }
}