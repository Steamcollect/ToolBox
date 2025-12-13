using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Creates a tab in the Inspector that groups fields until the next Tab.
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item><description><code>[Tab("General")]</code> starts a new tab section</description></item>
    /// </list>
    /// </summary>
    public class TabAttribute : PropertyAttribute
    {
        public string tabName;

        /// <param name="tabName">Label for the tab section.</param>
        public TabAttribute(string tabName)
        {
            this.tabName = tabName;
        }
    }
}