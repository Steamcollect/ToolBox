using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Marks the end of a foldout section started by [Foldout].
    /// <para>
    /// Use this attribute immediately after the last field you want to include in the foldout.
    /// </para>
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <code>[Foldout("Player Settings")]</code> → Starts foldout
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public int health;</code> → Field inside foldout
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public float speed;</code> → Another field inside foldout
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[CloseFoldout]</code> → Ends foldout
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public string nextField;</code> → Field outside foldout
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    public class CloseFoldoutAttribute : PropertyAttribute { }
}