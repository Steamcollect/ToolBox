using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Displays the field and its value on screen during Play Mode.
    /// <para>
    /// Can only be used once per variable.
    /// </para>
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[Watch]</code> shows value during runtime</description></item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class WatchAttribute : PropertyAttribute { }
}