using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Marks the end of a tab section started by [Tab].
    /// <para>
    /// Use this attribute immediately after the last field you want to include in the tab.
    /// </para>
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <code>[Tab("General")]</code> → Starts General tab
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public string playerName;</code> → Field in General tab
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[CloseTab]</code> → Ends General tab
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[Tab("Advanced")]</code> → Starts Advanced tab
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public bool debugMode;</code> → Field in Advanced tab
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>[CloseTab]</code> → Ends Advanced tab
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    public class CloseTabAttribute : PropertyAttribute { }
}